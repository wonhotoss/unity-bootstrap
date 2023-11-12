using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
using System.Linq;

public class build_mesh
{
    public static Mesh trilist_from_vertices(Vector3[] vertices){
        Mesh mesh = new Mesh();
        var triangles = Enumerable.Range(0, vertices.Length).ToArray();
        mesh.SetVertices(vertices.ToList());
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }

    // TODO: diagram comment?
    public static Vector3[] trilist_triangle_xy(float l, float t, float r){
        return new Vector3[]{
            new Vector3(-l, 0, 0),
            new Vector3(0, t, 0),
            new Vector3(r, 0, 0),
        };
    }

    public static Vector3[] trilist_rect_xy(Vector2 lt, Vector2 size, Vector2 haxis){
        haxis = haxis.normalized * size.x;
        var vaxis = (new Vector2(-haxis.y, haxis.x)).normalized * size.y;
        return new Vector3[]{
            lt,                 // lt
            lt + haxis,         // rt
            lt - vaxis,         // lb

            lt - vaxis,         // lb
            lt + haxis,         // rt
            lt - vaxis + haxis, // rb
        };
    }

    public static Vector3[] trilist_polygon_xy(Vector2 center, Vector2[] points){
        // assume that points clockwise convex
        Debug.Assert(points.Length > 2);
        var result = new List<Vector3>();
        for(var i = 0; i < points.Length; ++i){
            result.Add(center);
            result.Add(points[i]);
            result.Add(points.circular(i + 1));
        }
        return result.ToArray();
    }

    public static Vector3[] trilist_skirt_z(Vector3[] trilist, float depth){
        Debug.Assert(trilist.Length % 3 == 0);
        var skirt = new Vector3[trilist.Length * 6];
        for(var i = 0; i < trilist.Length; i += 3){
            foreach(var edge in new []{(0, 1), (1, 2), (2, 0)}){
                var rt = trilist[i + edge.Item1];
                var lt = trilist[i + edge.Item2];
                var rb = rt + Vector3.forward * depth;
                var lb = lt + Vector3.forward * depth;

                var idx_from = (i + edge.Item1) * 6;
                skirt[idx_from + 0] = lt;
                skirt[idx_from + 1] = rt;
                skirt[idx_from + 2] = lb;

                skirt[idx_from + 3] = rt;
                skirt[idx_from + 4] = rb;
                skirt[idx_from + 5] = lb;
            }
        }

        return skirt;
    }

    public void test(){
        if(GUILayout.Button("hyperbola")){
            var subdivisions = 20;
            var radius = 1f;
            var height = 1f;
            Mesh mesh = new Mesh();

            var vertices = new Vector3[(subdivisions * 4) * 3];

            var radUnit = Mathf.PI * 2.0f / subdivisions;
            for(var i = 0; i < subdivisions; ++i){
                var rad = new FromTo<float>(radUnit * i, radUnit * (i - 1));
                var x = rad.Map(r => Mathf.Cos(r) * radius);
                var b = -height;
                var t = height;
                var z = rad.Map(r => Mathf.Sin(r) * radius);
                var vertBottom = new FromTo<Vector3>(new Vector3(x.From, b, z.From), new Vector3(x.To, b, z.To));

                // bottom
                var offset = i * 3 * 4;
                vertices[offset + 0] = Vector3.up * b;
                vertices[offset + 1] = vertBottom.To;
                vertices[offset + 2] = vertBottom.From;
                

                // side
                offset += 3;
                vertices[offset + 0] = Vector3.zero;
                vertices[offset + 1] = vertBottom.From;
                vertices[offset + 2] = vertBottom.To;
                

                var vertTop = new FromTo<Vector3>(new Vector3(x.From, t, z.From), new Vector3(x.To, t, z.To));

                // top
                offset += 3;
                vertices[offset + 0] = Vector3.up * t;
                vertices[offset + 1] = vertTop.From;
                vertices[offset + 2] = vertTop.To;

                // side
                offset += 3;
                vertices[offset + 0] = Vector3.zero;
                vertices[offset + 1] = vertTop.To;
                vertices[offset + 2] = vertTop.From;
            }

            var triangles = Enumerable.Range(0, vertices.Length).ToArray();

            mesh.SetVertices(vertices.ToList());
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            // mesh.save_to_asset

            // var path = "Assets/" + "corn" + ".asset";
            // AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path));
            // Debug.Log("Mesh saved: " + path);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();

        }

        if(GUILayout.Button("cone")){
            // var a = new polygon();
            var subdivisions = 20;
            var radius = 1f;
            var height = 2f;
            Mesh mesh = new Mesh();

            var vertices = new Vector3[(subdivisions * 2) * 3];

            var radUnit = Mathf.PI * 2.0f / subdivisions;
            for(var i = 0; i < subdivisions; ++i){
                var rad = new FromTo<float>(radUnit * i, radUnit * (i - 1));
                var x = rad.Map(r => Mathf.Cos(r) * radius);
                var b = -height / 2;
                var t = height / 2;
                var z = rad.Map(r => Mathf.Sin(r) * radius);
                var vertBottom = new FromTo<Vector3>(new Vector3(x.From, b, z.From), new Vector3(x.To, b, z.To));

                // bottom
                var offset = i * 3 * 2;
                vertices[offset + 0] = Vector3.up * b;
                vertices[offset + 1] = vertBottom.To;
                vertices[offset + 2] = vertBottom.From;
                

                // side
                offset += 3;
                vertices[offset + 0] = Vector3.up * t;
                vertices[offset + 1] = vertBottom.From;
                vertices[offset + 2] = vertBottom.To;
                

                // var vertTop = new FromTo<Vector3>(new Vector3(x.From, t, z.From), new Vector3(x.To, t, z.To));

                // // top
                // offset += 3;
                // vertices[offset + 0] = Vector3.up * t;
                // vertices[offset + 1] = vertTop.From;
                // vertices[offset + 2] = vertTop.To;

                // // side
                // offset += 3;
                // vertices[offset + 0] = Vector3.zero;
                // vertices[offset + 1] = vertTop.To;
                // vertices[offset + 2] = vertTop.From;
            }

            var triangles = Enumerable.Range(0, vertices.Length).ToArray();

            mesh.SetVertices(vertices.ToList());
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            // var path = "Assets/" + "cone" + ".asset";
            // AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path));
            // Debug.Log("Mesh saved: " + path);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();

        }

        if(GUILayout.Button("Tetrahedron")){
            var points = new Vector3[4];
            var radius = 1.0f;
            var width = radius * Mathf.Cos(Mathf.Deg2Rad * 30.0f) * 2.0f;
            
            points[0] = Vector3.forward * radius;
            points[1] = Quaternion.AngleAxis(120.0f, Vector3.down) * points[0];
            points[2] = Quaternion.AngleAxis(-120.0f, Vector3.down) * points[0];
            points[3] = Vector3.up * width * Mathf.Sqrt(6.0f) / 3.0f;

            var avg = (points[0] + points[1] + points[2] + points[3]) * 0.25f;

            for(var i = 0 ; i < points.Length; ++i){
                points[i]= points[i] - avg;
            }

            var vertices = new Vector3[12];
            var offset = 0;

            void setVertices(int point0, int point1, int point2){
                vertices[offset + 0] = points[point0];
                vertices[offset + 1] = points[point1];
                vertices[offset + 2] = points[point2];
                offset += 3;
            };

            setVertices(0, 1, 2);
            setVertices(0, 3, 1);
            setVertices(0, 2, 3);
            setVertices(1, 3, 2);

            // vertices = vertices.Take()

            var triangles = Enumerable.Range(0, vertices.Length).ToArray();

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices.ToList());
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            // var path = "Assets/" + "tetrahedron" + ".asset";
            // AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path));
            // Debug.Log("Mesh saved: " + path);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();


        }

        if(GUILayout.Button("cube")){
            var points = new Vector3[8];
            var half = -0.5f;

            var vertices = new Vector3[6 * 2 * 3];
            var offset = 0;

            void plane(Vector3 look, Vector3 up){
                var side = Vector3.Cross(look, up);
                var from = (look + up + side) * half;
                vertices[offset++] = Quaternion.AngleAxis(0.0f, look) * from;
                vertices[offset++] = Quaternion.AngleAxis(-90.0f, look) * from;
                vertices[offset++] = Quaternion.AngleAxis(-180.0f, look) * from;
                vertices[offset++] = Quaternion.AngleAxis(-180.0f, look) * from;
                vertices[offset++] = Quaternion.AngleAxis(-270.0f, look) * from;
                vertices[offset++] = Quaternion.AngleAxis(-360.0f, look) * from;
            };

            plane(Vector3.up, Vector3.back);
            plane(Vector3.right, Vector3.up);
            plane(Vector3.forward, Vector3.up);
            plane(Vector3.left, Vector3.up);
            plane(Vector3.down, Vector3.forward);
            plane(Vector3.back, Vector3.up);

            var triangles = Enumerable.Range(0, vertices.Length).ToArray();

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices.ToList());
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            // var path = "Assets/" + "cube" + ".asset";
            // AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path));
            // Debug.Log("Mesh saved: " + path);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
        }
    }
}
