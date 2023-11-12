using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Cysharp.Threading.Tasks;
using System.Linq;
using System;
using System.Linq.Expressions;
using UnityEngine.UI;
using UnityEngine.Networking;

public static class @enum{
    public static T[] get_values<T>() where T: System.Enum{
        return System.Enum.GetValues(typeof(T)).Cast<T>().ToArray();
    }

    public static string[] get_names<T>() where T: System.Enum{
        return System.Enum.GetNames(typeof(T)).ToArray();
    }
}



public class OnlyFirst: CustomYieldInstruction{
    public override bool keepWaiting{
        get{
            return false;
        }
    }

    int index;

    public OnlyFirst(params IEnumerator[] iterators){

    }

}
public struct FromTo<T>{
    public T From;
    public T To;
    public FromTo(T from, T to){
        From = from;
        To = to;
    }

    // TODO: named tuple?
    public FromTo<K> Map<K>(System.Func<T, K> mapper){
        return new FromTo<K>(mapper(From), mapper(To));
    }
}

[Serializable]
public struct FromToF{
    public float From;
    public float To;
    public FromToF(float from, float to){
        From = from;
        To = to;

    }
    public float half => Mathf.Lerp(From, To, 0.5f);
    public float lerpUnclamped(float t) => Mathf.LerpUnclamped(From, To, t);
    public float diff => To - From;
    public bool includes(float v) => ((From - v) * (To - v)) <= 0.0f;

    public override string ToString()
    {
        return $"from: {From}, to: {To}";
    }

    public static bool distinct(FromToF[] arr){
        for(var i = 1; i < arr.Length; ++i){
            if(arr[i].From <= arr[i - 1].To){
                return false;
            }
        }
        return true;
    }
    public FromToF map(System.Func<float, float> mapper){
        return new FromToF(mapper(From), mapper(To));
    }

    public bool overlap(FromToF other){
        return (From - other.From) * (To - other.To) <= 0.0f;
    }
}

public struct TimelySpend{
    public readonly float fromTime;
    public readonly float from;
    public readonly float duration;
    public readonly float spendPerSec;
    float lastTime;
    public float remain{get; private set;}

    public TimelySpend(float from, float duration){
        fromTime = Time.time;
        this.from = from;
        this.duration = duration;
        lastTime = fromTime;
        spendPerSec = from / duration;
        remain = from;
    }

    public float Spend(){
        var deltaTime = Time.time - lastTime;
        var spent = deltaTime * spendPerSec;
        if(Mathf.Abs(spent) > Mathf.Abs(remain)){
            spent = remain;
        }
        remain -= spent;
        lastTime = Time.time;
        return spent;
    }
}

public struct FromNow
{
    public readonly float TimeFrom;
    public readonly float Duration;
    
    public float Elapsed => (Time.time - TimeFrom);
    public bool Expired => Elapsed >= Duration;
    public float Progress => Elapsed / Duration;

    public FromNow(float duration, float elapsed = 0.0f)
    {   
        TimeFrom = Time.time - elapsed;
        Duration = duration;
    }
}

// TODO: cancelation token?
public class Boxed<T>{
    public T value;

    public Boxed(T _value = default(T)){
        value = _value;
    }

    public static implicit operator T(Boxed<T> boxed) => boxed.value;
};

public static class MathUtil{
    public static float noease(float t){
        return t;
    }
    public static float ease01(float t){
        return Mathf.Cos(Mathf.Lerp(0, Mathf.PI, t)) * -0.5f + 0.5f;
    }

    public static float ease0(float t){
        return t * t;
    }

    public static float ease1(float t){
        return 1.0f - Mathf.Pow(1.0f - t, 2.0f);
    }
    public static float ease0cubic(float t){
        return t * t * t;
    }

    public static float ease1cubic(float t){
        return 1.0f - Mathf.Pow(1.0f - t, 3.0f);
    }

    public static int gcd(int a, int b) {
        while (b != 0) {
            int temp = a % b;
            a = b;
            b = temp;
        }
        return Mathf.Abs(a);
    }

    public static float unlerp(float from, float to, float now){
        return (now - from) / (to - from);
    }

    public static (bool, float, float) solveQuadratic(float a, float b, float c){
        // Debug.Log(a);
        // Debug.Log(b);
        // Debug.Log(c);

        if(a == 0.0f && b != 0.0f){
            return (
                true,
                -c / b,
                -c / b
            );
        }

        if(a == 0.0f && b == 0.0f){
            return (
                false,
                float.NaN,
                float.NaN
            );
        }

        var must_positive = Mathf.Pow(b, 2.0f) - 4.0f * a * c;

        if(must_positive < 0.0f){
            return (
                false,
                float.NaN,
                float.NaN
            );
        }

        return (
            true,
            (-b + Mathf.Sqrt(must_positive)) / (2.0f * a),
            (-b - Mathf.Sqrt(must_positive)) / (2.0f * a)
        );
    }

    public static IEnumerable<(int x, int y)> enumerateRowMajor(int cols, int rows){
        for(var y = 0; y < rows; ++y){
            for(var x = 0; x < cols; ++x){
                yield return (x, y);
            }
        }
    }
}

// public class calmFollower
// {
//     AnimationCurve3D curve = new AnimationCurve3D();
//     public calmFollower(Transform target, float timeDelta){
//         follow(target, timeDelta);
//     }

//     async void follow(Transform target, float timeDelta){
//         var recordedTime = float.MinValue;
//         while(target){
//             if(Time.time > recordedTime + timeDelta){
//                 recordedTime = Time.time;
//                 curve.addKey(Time.time + timeDelta, target.position);
//                 curve.removeKeysBefore(Time.time - timeDelta * 10);
//             }
//             await UniTask.NextFrame(PlayerLoopTiming.LastUpdate);
//         }
//     }

//     public Vector3 Evaluate(){
//         return curve.evaluate(Time.time);
//     }
// }


// public class blender{
//     struct source{
//         public float timeFrom;
//         public float timeTo;
//         public Transform transform;
//     };
    
//     List<source> sources = new List<source>();

//     public blender(Transform target, System.Func<bool> stopped){
//         startBlend(target, stopped);
//     }

//     public void add(Transform transform, float time){
//         sources.Add(new source(){
//             timeFrom = Time.time,
//             timeTo = Time.time + time,
//             transform = transform,
//         });
//     }

//     async void startBlend(Transform target, System.Func<bool> stopped){
//         while(!stopped()){
//             if(sources.Count > 0){
//                 // dispose invisible sources
//                 var lastOpaque = sources.FindLastIndex(0, s => s.timeTo > Time.time);
//                 if(lastOpaque >= 0){
//                     sources.RemoveRange(0, lastOpaque);
//                 }

//                 Debug.Assert(sources.Count > 0);
                
//                 var t = sources[0].transform.position;
//                 var r = sources[0].transform.rotation;

//                 foreach(var source in sources.Skip(1)){
//                     if(source.transform){
//                         var weight = MathUtil.ease01((Time.time - source.timeFrom) / (source.timeTo - source.timeFrom));
//                         t = Vector3.Lerp(t, source.transform.position, weight);
//                         r = Quaternion.Lerp(r, source.transform.rotation, weight);
//                     }
//                 }

//                 if(target){
//                     target.transform.position = t;
//                     target.transform.rotation = r;
//                 }
//             }
//             await UniTask.NextFrame(PlayerLoopTiming.PostLateUpdate);
//         }
//     }
// }

public class TextureUtil{
    // static Texture2D one_pixel(){
    //     var result = new RenderTexture(1, 1, 0, DefaultFormat.)

    // }
}

public class AnimationCurve3D{
    public AnimationCurve x = new AnimationCurve();
    public AnimationCurve y = new AnimationCurve();
    public AnimationCurve z = new AnimationCurve();

    public FromToF timeFromTo {get; private set;} = new FromToF(float.MaxValue, float.MinValue);
    // public readonly FromTo<float> timeFromTo = new FromTo<float>(float.MaxValue, float.MinValue);

    public (float time, Vector3 value, Vector3 intangents, Vector3 outtangents)[] keys{
        get{
            var keys = new{
                x = x.keys,
                y = y.keys,
                z = z.keys,
            };

            return Enumerable.Range(0, keys.x.Length)
                .Select(i => (
                    keys.x[i].time, 
                    new Vector3(keys.x[i].value, keys.y[i].value, keys.z[i].value), 
                    new Vector3(keys.x[i].inTangent, keys.y[i].inTangent, keys.z[i].inTangent),
                    new Vector3(keys.x[i].outTangent, keys.y[i].outTangent, keys.z[i].outTangent)))
                .ToArray();
        }
    }

    public void addKey(float time, Vector3 value){
        x.AddKey(time, value.x);
        y.AddKey(time, value.y);
        z.AddKey(time, value.z);

        // var timeFromTo_ = timeFromTo;
        timeFromTo = new FromToF(Mathf.Min(time, timeFromTo.From), Mathf.Max(time, timeFromTo.To));
    }

    public void addKey(float time, Vector3 value, Vector3 intangents, Vector3 outtangents){
        x.AddKey(new Keyframe(time, value.x, intangents.x, outtangents.x));
        y.AddKey(new Keyframe(time, value.y, intangents.y, outtangents.y));
        z.AddKey(new Keyframe(time, value.z, intangents.z, outtangents.z));

        // var timeFromTo_ = timeFromTo;
        timeFromTo = new FromToF(Mathf.Min(time, timeFromTo.From), Mathf.Max(time, timeFromTo.To));
    }

    public Vector3 evaluate(float time){
        return new Vector3(
            x.Evaluate(time),
            y.Evaluate(time),
            z.Evaluate(time)
        );
    }

    public void removeKeysBefore(float time){
        System.Action<AnimationCurve> remove = curve => {
            while(curve.length > 0 && curve[0].time < time){
                curve.RemoveKey(0);
            }
        };
        remove(x);
        remove(y);
        remove(z);
    }
}

public static class Extensions{
    // public const float standardAnimationDuration = 0.2f;
    public const float standardAnimationDuration = 0.4f;
    public static Transform add(this Transform parent, string name, bool singleton = true){
        if(singleton){
            foreach(var old in parent.getChildren().Where(c => c.name == name)){
                GameObject.DestroyImmediate(old.gameObject);
            }
        }

        var result = new GameObject(name).transform;
        result.SetParent(parent, false);
        return result;
    }

    public static Transform add(this Transform parent, Transform template){
        var result = GameObject.Instantiate(template.gameObject).transform;
        result.SetParent(parent, false);    // localposition + localRotation preserved
        result.localRotation = template.localRotation;
        return result;
    }

    public static T add<T>(this Transform parent, T template, string name = null) where T: Component{
        var result = add(parent, template.transform);
        result.name = name ?? result.name;
        return result.GetComponent<T>();
    }

    public static Transform[] getChildren(this Component parent){
        var trns = parent.transform;
        var result = new Transform[trns.childCount];
        for(var i = 0; i < trns.childCount; ++i){
            result[i] = trns.GetChild(i);
        }
        return result;
    }

    public static Transform findRecursive(this Transform parent, string name){
        for(var i = 0; i < parent.childCount; ++i){
            var child = parent.GetChild(i);
            if(child.name == name){
                return child;
            }
            else{
                var decendant = findRecursive(child, name);
                if(decendant){
                    return decendant;
                }
            }
        }

        return null;
    }

    public static void on(this Component c){
        c.gameObject.SetActive(true);
    }

    public static void off(this Component c){
        c.gameObject.SetActive(false);
    }
    public static void on_off(this Component c, bool on){
        c.gameObject.SetActive(on);
    }

    public static T pickRandom<T>(this T[] candidates){
        return candidates[UnityEngine.Random.Range(0, candidates.Length)];
    }

    public static T[] pickRandom<T>(this T[] candidates, int count){
        var begin = 0;
        var end = candidates.Length;
        var result = new List<T>();
        while(begin < end && count > 0){
            var idx = UnityEngine.Random.Range(begin, end - (count - 1));
            result.Add(candidates[idx]);
            begin = idx + 1;
            count--;
        }
        return result.ToArray();
    }

    public static T[] shuffle<T>(this T[] candidates){
        var result = new List<T>();
        var src = candidates.ToList();
        while(src.Count > 0){
            var idx = UnityEngine.Random.Range(0, src.Count);
            result.Add(src[idx]);
            src.RemoveAt(idx);
        }
        return result.ToArray();
    }

    public static T clone<T>(this T component) where T: Component{
        return GameObject.Instantiate(component.gameObject).GetComponent<T>();
    }

    // public static async UniTask slide(this Transform tr, Vector3 from, Vector3 to, float duration = standardAnimationDuration){
    //     var fromNow = new FromNow(duration);
    //     while(!fromNow.Expired && tr.gameObject != null){
    //         tr.localPosition = Vector3.Lerp(from, to, fromNow.Progress * fromNow.Progress);
    //         await UniTask.NextFrame();
    //     }
    //     tr.localPosition = to;
    // }

    public static IEnumerator progress(float duration, System.Action<float> on_progress01){
        var fromNow = new FromNow(duration);
        while(!fromNow.Expired){
            on_progress01(fromNow.Progress);
            yield return null;
        }
        on_progress01(1.0f);
    }

    public static Coroutine progress(this MonoBehaviour host, float duration, System.Action<float> on_progress01){
        return host.StartCoroutine(progress(duration, on_progress01));
    }

    public static Coroutine after(this MonoBehaviour host, float time, System.Action job){
        IEnumerator iter(){
            yield return new WaitForSeconds(time);
            job();
        }
        return host.StartCoroutine(iter());
    }

    // public static delegate float Del(float f);

    public static IEnumerator slide(this Transform tr, Vector3 from, Vector3 to, System.Func<float, float> easing, float duration = standardAnimationDuration){
        var fromNow = new FromNow(duration);
        while(!fromNow.Expired && tr.gameObject != null){
            tr.localPosition = Vector3.Lerp(from, to, easing(fromNow.Progress));
            yield return null;
        }
        tr.localPosition = to;
    }

    public static IEnumerator rotate_local(this Transform tr, Quaternion from, Quaternion to, System.Func<float, float> easing, float duration = standardAnimationDuration){
        var fromNow = new FromNow(duration);
        while(!fromNow.Expired && tr.gameObject != null){
            tr.localRotation = Quaternion.Lerp(from, to, easing(fromNow.Progress));
            yield return null;
        }
        tr.localRotation = to;
    }

    public static IEnumerator rotate(this Transform tr, Quaternion from, Quaternion to, System.Func<float, float> easing, float duration = standardAnimationDuration){
        var fromNow = new FromNow(duration);
        while(!fromNow.Expired && tr.gameObject != null){
            tr.rotation = Quaternion.Lerp(from, to, easing(fromNow.Progress));
            yield return null;
        }
        tr.localRotation = to;
    }

    

    public static IEnumerator slide_in(this Transform tr, Vector3 from, float duration = standardAnimationDuration){
        var old = tr.localPosition;
        return tr.slide(from, old, MathUtil.ease1, duration);
    }
    public static IEnumerator slide_in_down(this RectTransform rt, float duration = standardAnimationDuration){
        var t = rt.GetComponentInParent<Canvas>().renderingDisplaySize.y;
        var screen_top_to_tr_bottom = t - rt.get_world_corners()[0].y;
        return slide_in(rt, rt.parent.InverseTransformPoint(rt.position + Vector3.up * screen_top_to_tr_bottom), duration);
    }

    public static IEnumerator slide_in_up(this RectTransform rt, float duration = standardAnimationDuration){
        var screen_bottom_to_tr_top = rt.get_world_corners()[1].y;
        return slide_in(rt, rt.parent.InverseTransformPoint(rt.position + Vector3.down * screen_bottom_to_tr_top), duration);
    }

    public static IEnumerator slide_out_up(this RectTransform rt, float duration = standardAnimationDuration){
        var t = rt.GetComponentInParent<Canvas>().renderingDisplaySize.y;
        var screen_top_to_tr_bottom = t - rt.get_world_corners()[0].y;
        return slide_out(rt, rt.parent.InverseTransformPoint(rt.position + Vector3.up * screen_top_to_tr_bottom), duration);
    }

    public static IEnumerator slide_out_down(this RectTransform rt, float duration = standardAnimationDuration){
        var screen_bottom_to_tr_top = rt.get_world_corners()[1].y;
        return slide_out(rt, rt.parent.InverseTransformPoint(rt.position + Vector3.down * screen_bottom_to_tr_top), duration);
    }

    // lb, lt, rt, rb
    public static Vector3[] get_world_corners(this RectTransform rt){
        var corners = new Vector3[4]; 
        rt.GetWorldCorners(corners);
        return corners;
    }

    public static IEnumerator slide_out(this Transform tr, Vector3 to, float duration = standardAnimationDuration){
        var old = tr.localPosition;
        var sliding = tr.slide(old, to, MathUtil.ease0, duration);

        // maybe unity coroutine not support nested iterator
        while(sliding.MoveNext()){
            yield return sliding.Current;
        }
        
        tr.localPosition = old;
    }

    public static IEnumerator fade(this Image sr, float from, float to, float duration = standardAnimationDuration){
        var fromNow = new FromNow(duration);
        while(!fromNow.Expired && sr.gameObject != null){
            sr.setAlpha(Mathf.Lerp(from, to, fromNow.Progress * fromNow.Progress));
            yield return null;
        }
        sr.setAlpha(to);
    }

    public static IEnumerator fade_in(this Image sr, float duration = standardAnimationDuration){
        return sr.fade(0.0f, sr.color.a, duration);
    }

    public static IEnumerator fade_out(this Image sr, float duration = standardAnimationDuration){
        var alpha = sr.color.a;
        yield return sr.fade(sr.color.a, 0.0f, duration);
        sr.setAlpha(alpha);
    }

    public static void setAlpha(this Image image, float alpha){
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public static Material instanciateMaterial(this Renderer renderer){
        renderer.material = Material.Instantiate(renderer.material);
        return renderer.material;
    }

    // public static async UniTask fadeOut(this SpriteRenderer sr, float duration = standardAnimationDuration){
    //     await fade(sr, sr.color.a, 0.0f, duration);
    // }

    // public static async UniTask slideIn(this Transform tr, Vector3 from, float duration = standardAnimationDuration){
    //     var old = tr.localPosition;
    //     await tr.slide(from, old, duration);
    // }

    // public static async UniTask slideOut(this Transform tr, Vector3 to, float duration = standardAnimationDuration){
    //     var old = tr.localPosition;
    //     await tr.slide(old, to, duration);
    //     tr.localPosition = old;
    // }

    public static IEnumerator zoom(this Transform tr, Vector3 from, Vector3 to, System.Func<float, float> easing, float duration = standardAnimationDuration){
        var fromNow = new FromNow(duration);
        while(!fromNow.Expired && tr.gameObject != null){
            tr.localScale = Vector3.Lerp(from, to, easing(fromNow.Progress));
            yield return null;
        }
        tr.localScale = to;
    }

    public static IEnumerator zoomIn(this Transform tr, System.Func<float, float> easing, float duration = standardAnimationDuration){
        return tr.zoom(Vector3.zero, tr.localScale, easing, duration);
    }
    public static IEnumerator zoomOut(this Transform tr, System.Func<float, float> easing, float duration = standardAnimationDuration){
        var old = tr.localScale;
        var zooming = tr.zoom(old, Vector3.zero, easing, duration);
        // maybe unity coroutine not support nested iterator
        while(zooming.MoveNext()){
            yield return zooming.Current;
        }
        tr.localScale = old;
    }

    public static void showExclusve(this Transform tr){
        foreach(var child in tr.parent.getChildren()){
            child.gameObject.SetActive(child == tr);
        }
    }

    public static void keepExclusve(this Transform tr){
        foreach(var child in tr.parent.getChildren().Except(tr)){
            GameObject.Destroy(child.gameObject);
        }
    }

    public static T circular<T>(this T[] arr, int at){
        // TODO: mod?
        while(at < 0){
            at = arr.Length + at;
        }

        return arr[at % arr.Length];
    }

    public static T[] rotate<T>(this T[] arr, int start_at){
        return arr.Select((a, i) => arr.circular(i + start_at)).ToArray();
    }

    public static bool Contains<T>(this IEnumerable<T> self, IEnumerable<T> search){
        return search.All(s => self.Contains(s));
    }

    public static Vector2 Average(this IEnumerable<Vector2> arr){
        return new Vector2(arr.Select(v => v.x).Average(), arr.Select(v => v.y).Average());
    }

    public static Vector3 Average(this IEnumerable<Vector3> arr){
        return new Vector3(arr.Select(v => v.x).Average(), arr.Select(v => v.y).Average(), arr.Select(v => v.z).Average());
    }

    public static T host3DtrackerInstance<T>(this MonoBehaviour host, T template, Camera camera, Transform anchor) where T : MonoBehaviour{
        var instance = template.transform.parent.add(template);
        instance.gameObject.SetActive(true);
        var tracker = instance.gameObject.AddComponent<UITracker3D>();
        tracker.cam = camera;
        tracker.anchor = anchor;

        IEnumerator update(){
            var rt = instance.GetComponent<RectTransform>();
            while(instance && anchor && camera){
                var screen_point = camera.WorldToScreenPoint(anchor.position);
                rt.anchorMax = rt.anchorMin = new Vector2(
                    screen_point.x / camera.pixelWidth,
                    screen_point.y / camera.pixelHeight
                );
                rt.offsetMin = rt.offsetMax = Vector2.zero;
                yield return null;
            }
        }

        host.StartCoroutine(update());
        
        return instance;
    }

    public static IEnumerator blink(this Graphic graphic, Color color){
        var old_color = graphic.color;
        yield return graphic.progress(1f, (p01) => {
            graphic.color = Mathf.CeilToInt(p01 * 10) % 2 > 0 ? color : old_color;
        });
        graphic.color = old_color;
    }

    public static void playAudio(this MonoBehaviour host, AudioClip clip){
        var audiosource = host.gameObject.AddComponent<AudioSource>();
        audiosource.spatialBlend = 0.0f;
        audiosource.clip = clip;
        audiosource.Play();
        host.after(clip.length, () => AudioSource.Destroy(audiosource));
    }

    public static void updateLayoutRecursively(this Transform leaf){
        // var vein = new List<Transform>();

        

        while(leaf != null){
            // vein.Add(leaf);
            // leaf = leaf.parent;

            var lg = leaf.GetComponent<LayoutGroup>();
            if(lg != null){
                LayoutRebuilder.ForceRebuildLayoutImmediate(lg.GetComponent<RectTransform>());
            }
            leaf = leaf.parent;
        }

        // var inactives = vein.Where(t => !t.gameObject.activeSelf).ToArray();
        // foreach(var t in inactives){
        //     t.gameObject.SetActive(true);
        // }

        // foreach(var t in vein){
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(t.GetComponent<RectTransform>());
        // }

        // foreach(var t in inactives){
        //     t.gameObject.SetActive(false);
        // }
    }

    // public static void updateMarkLayoutRebuild(this RectTransform leaf){
    //     while(leaf != null){
    //         LayoutRebuilder.MarkLayoutForRebuild(leaf);
    //         leaf = leaf.parent != null ? leaf.parent.GetComponent<RectTransform>() : null;
    //     }
    // }

    // public static blender startBlend(this Transform tr, System.Func<bool> stopped){
    //     return new blender(tr, stopped);
    // }

    
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T what){
        return enumerable.Concat(new T[]{what});
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T what){
        return enumerable.Except(new T[]{what});
    }
    public static Stack<T> ToStack<T>(this IEnumerable<T> enumerable){
        var result = new Stack<T>();
        foreach(var elem in enumerable){
            result.Push(elem);
        }
        return result;
    }
    public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable){
        var result = new Queue<T>();
        foreach(var elem in enumerable){
            result.Enqueue(elem);
        }
        return result;
    }
    public static Vector2 replaceX(this Vector2 src, float x){
        return new Vector2(x, src.y);
    }
    public static Vector2 replaceY(this Vector2 src, float y){
        return new Vector2(src.x, y);
    }
    public static Vector3 replaceX(this Vector3 src, float x){
        return new Vector3(x, src.y, src.z);
    }
    public static Vector3 replaceY(this Vector3 src, float y){
        return new Vector3(src.x, y, src.z);
    }
    public static Vector3 replaceZ(this Vector3 src, float z){
        return new Vector3(src.x, src.y, z);
    }

    public static Vector3 scaleX(this Vector3 src, float x){
        return new Vector3(src.x * x, src.y, src.z);
    }
    public static Vector3 scaleY(this Vector3 src, float y){
        return new Vector3(src.x, src.y * y, src.z);
    }
    public static Vector3 scaleZ(this Vector3 src, float z){
        return new Vector3(src.x, src.y, src.z * z);
    }

    public static void log<T>(this IEnumerable<T> enumerable, string header){
        Debug.Log(to_log(enumerable, header));
    }
    public static string to_log<T>(this IEnumerable<T> enumerable, string header){
        var i = 0;
        var result = "";
        foreach(var e in enumerable)
        {
            result += $"{header} {i++} : {e.ToString()}\n";
        }
        return result;
    }

    public static string ordinal(this int n){
        return 
            n == 1 ? "1st" :
            n == 2 ? "2nd" :
            n == 3 ? "3rd" : $"{n}th";
    }

    public static RenderTexture TakePicture(this RectTransform rt){
        var rect = rt.rect;

        // make dummy canvas with camera
        var canvas = new GameObject("dummy").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        var camera = (new GameObject("camera")).AddComponent<Camera>();
        camera.transform.SetParent(canvas.transform, false);
        camera.orthographic = true;
        camera.nearClipPlane = 0.0f;    // negative more?
        canvas.worldCamera = camera;

        // migrate
        var clone = GameObject.Instantiate(rt.gameObject).GetComponent<RectTransform>();
        clone.transform.SetParent(canvas.transform, false);
        clone.transform.localPosition = Vector3.zero;
        
        clone.anchorMin = new Vector2(0.5f, 0.5f);
        clone.anchorMax = new Vector2(0.5f, 0.5f);
        clone.anchoredPosition = Vector2.zero;
        clone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
        clone.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);

        // render
        var result = new RenderTexture((int)rect.width, (int)rect.height, 32);
        camera.targetTexture = result;
        camera.Render();
        camera.targetTexture = null;

        // cleanup
        GameObject.Destroy(canvas.gameObject);

        return result;
    }

    public static IEnumerator parallel(this MonoBehaviour host, params IEnumerator[] iterators){
        var coroutines = iterators.Select(it => host.StartCoroutine(it)).ToArray();
        foreach(var c in coroutines){
            yield return c;
        }
    }

    public static IEnumerator sequential(this MonoBehaviour host, params IEnumerator[] iterators){
        foreach(var iter in iterators){
            yield return iter;
        }
    }

    public static Collider get_pointing(Collider[] colliders){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        return colliders.Where(c => c != null).FirstOrDefault(c => c.Raycast(ray, out hit, 100));
    }

    public static Boxed<Collider> wait_mousedown(this MonoBehaviour host, Collider[] colliders){
        var boxed = new Boxed<Collider>();
        IEnumerator iter(){
            yield return new WaitUntil(() => !Input.GetMouseButton(0));
            while(boxed.value == null){
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject());
                boxed.value = get_pointing(colliders);
            }
        }
        host.StartCoroutine(iter());
        return boxed;
    }

    public static Boxed<Collider> wait_mouseup(this MonoBehaviour host, Collider[] colliders){
        var boxed = new Boxed<Collider>();
        IEnumerator iter(){
            yield return new WaitUntil(() => Input.GetMouseButton(0));
            while(boxed.value == null){
                yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
                boxed.value = get_pointing(colliders);
            }
        }
        host.StartCoroutine(iter());
        return boxed;
    }

    public static Boxed<Collider> wait_mouseclick(this MonoBehaviour host, params Collider[] colliders){
        var boxed = new Boxed<Collider>();
        IEnumerator iter(){
            while(boxed.value == null){
                var down = wait_mousedown(host, colliders);
                var up = wait_mouseup(host, colliders);
                yield return new WaitUntil(() => down.value != null && up.value != null);
                if(down.value == up.value){
                    boxed.value = down.value;
                }
            }
        }
        host.StartCoroutine(iter());
        return boxed;
    }
    
    public static Boxed<bool> clicked(this Button button){
        var clicked = new Boxed<bool>(false);
        UnityEngine.Events.UnityAction listener = null;
        listener = () => {
            clicked.value = true;

            //
            // var audioclips = GameObject.FindObjectOfType<audioclips>();
            // if(audioclips && audioclips.button){
            //     audioclips.playEffect(audioclips.button);
            // }
            //

            button.onClick.RemoveListener(listener);
        };
        button.onClick.AddListener(listener);
        return clicked;
    }

    public static IEnumerator waitClick(this Button button){
        var clicked = button.clicked();
        yield return new WaitUntil(() => clicked.value);
    }

    public static IEnumerator fadein(this AudioSource AS, float duration = standardAnimationDuration){
        var volume = AS.volume;
        AS.Play();
        AS.volume = 0.0f;
        var fromnow = new FromNow(duration);
        while(!fromnow.Expired){
            AS.volume = MathUtil.ease01(fromnow.Progress) * volume;
            yield return null;
        }
        AS.volume = volume;
    }

    public static IEnumerator fadeout(this AudioSource AS, float duration = standardAnimationDuration){
        var volume = AS.volume;
        var fromnow = new FromNow(duration);
        while(!fromnow.Expired){
            AS.volume = (1.0f - MathUtil.ease01(fromnow.Progress)) * volume;
            yield return null;
        }
        AS.Stop();
        AS.volume = volume;
    }

#if UNITY_EDITOR
    public static void save_to_asset(this UnityEngine.Object o, string name, bool replace = false){
        var path = "Assets/" + name + ".asset";
        if(replace){
            UnityEditor.AssetDatabase.DeleteAsset(path);
        }
        
        UnityEditor.AssetDatabase.CreateAsset(o, UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path));
        Debug.Log("asset saved: " + path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
#endif

    public static Vector2 xy(this Vector3 xyz){
        return new Vector2(xyz.x, xyz.y);
    }

    public static Vector2 yz(this Vector3 xyz){
        return new Vector2(xyz.y, xyz.z);
    }

    public static Vector2 xz(this Vector3 xyz){
        return new Vector2(xyz.x, xyz.z);
    }

    public static Vector3 z0(this Vector2 xy){
        return z(xy, 0);
    }

    public static Vector3 z(this Vector2 xy, float z){
        return new Vector3(xy.x, xy.y, z);
    }

    public static Vector3Int z0(this Vector2Int xy){
        return z(xy, 0);
    }

    public static Vector3Int z(this Vector2Int xy, int z){
        return new Vector3Int(xy.x, xy.y, z);
    }

    public static void track3D(this RectTransform rt, Camera cam, Component anchor){
        var screen_point = cam.WorldToScreenPoint(anchor.transform.position);
        // Debug.Log(screen_point);
        // Debug.Log(cam.pixelWidth);
        // Debug.Log(cam.pixelHeight);
        // Debug.Log(Screen.width);
        // Debug.Log(Screen.height);
        
        var sizeDelta = rt.sizeDelta;

        // assume that fullscreencanvas + direct child
        rt.anchorMax = rt.anchorMin = new Vector2(
            // screen_point.x / cam.pixelWidth,
            // screen_point.y / cam.pixelHeight,
            screen_point.x / Screen.width,
            screen_point.y / Screen.height
        );
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        rt.sizeDelta = sizeDelta;
    }

    public static RectTransform rt(this UnityEngine.EventSystems.UIBehaviour ui){
        return ui.GetComponent<RectTransform>();
    }

    public static IEnumerable<T> Enumerate<T> (this (T, T) value){
        yield return value.Item1;
        yield return value.Item2;
    }

    public static IEnumerable<T> Enumerate<T> (this (T, T, T) value){
        return (value.Item1, value.Item2).Enumerate().Concat(value.Item3);
    }

    public static IEnumerable<T> Enumerate<T> (this (T, T, T, T) value){
        return (value.Item1, value.Item2, value.Item3).Enumerate().Concat(value.Item4);
    }

    // public static void EnsureLayout(this Canvas canvas){


    // }

    // static void _ensureLayout(Transform tr){
    //     foreach(var c in tr.getChildren()){
    //         _ensureLayout(c);
    //     }
    //     for(var l in tr.GetComponents<LayoutGroup>()){
    //         LayoutRebuilder.ForceRebuildLayoutImmediate()
    //     }
    // }
    public static Button make_button(this RectTransform rt){
        var b = rt.gameObject.GetComponent<Button>();
        if(b == null){
            b = rt.gameObject.AddComponent<Button>();
        }

        if(rt.GetComponent<Graphic>() == null){
            rt.gameObject.AddComponent<RaycastTarget>();
        }

        // if(rt.GetComponent<CanvasRenderer>() == null){
        //     rt.gameObject.AddComponent<CanvasRenderer>();
        // }
        return b;
    }

    public static (Coroutine coroutine, Boxed<bool> terminated) start_watchable_coroutine(this MonoBehaviour host, IEnumerator iter){
        var terminated = new Boxed<bool>(false);
        IEnumerator wrapper(){
            yield return iter;
            terminated.value = true;
        }
        return (host.StartCoroutine(wrapper()), terminated);
    }
}

[RequireComponent(typeof(CanvasRenderer))]
public class RaycastTarget : Graphic 
{
    public override void SetMaterialDirty() { return; }
    public override void SetVerticesDirty() { return; }
}

public static class editor{
    // TODO: show anything in modal dialog box. enable inspect and editing
    public static void Modal(params UnityEngine.Object[] anything){

    }
}


// http://tomasp.net/blog/dynamic-linq-queries.aspx/
public static class Linq {
    // Returns the given anonymous method as a lambda expression
    public static Expression<Func<T, R>> Expr<T, R>(Expression<Func<T, R>> f) {
        return f;
    }

    // Returns the given anonymous function as a Func delegate
    public static Func<T, R> Func<T, R>(Func<T, R> f) {
        return f;
    }

    public static Func<R> Func<R>(Func<R> f) {
        return f;
    }

    public static Action Action(Action f) {
        return f;
    }
}

public class TextDownloader{
    const string directory = "TextDownloaderPersistentCache";
    public readonly string url;
    public readonly Coroutine downloading;
    public string result{get; private set;}
    public string encodedURL{get; private set;}
    public TextDownloader(MonoBehaviour host, string url){
        this.url = url;
        downloading = host.StartCoroutine(download());
    }
    public IEnumerator download(){
        result = "<UNKNOWN>";

        // persistent cache first
        // https://stackoverflow.com/questions/26353710/how-to-achieve-base64-url-safe-encoding-in-c
        byte[] encData_byte = System.Text.Encoding.UTF8.GetBytes(url);
        encodedURL = Convert.ToBase64String(encData_byte).TrimEnd().Replace('+', '-').Replace('/', '_');
        var textAsset = Resources.Load($"{directory}/{encodedURL}") as TextAsset;
        if(textAsset != null){
            result = textAsset.text;
            Debug.Log($"{url} from persistent cache");
            Debug.Log(result);
        }
        
        // try dynamic cache
        result = PlayerPrefs.GetString(encodedURL, result);
        Debug.Log($"{url} from dynamic cache");
        Debug.Log(result);

        if(Application.internetReachability != NetworkReachability.NotReachable){
            // try download and update dynamic cache
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        // Debug.Log("Received: " + webRequest.downloadHandler.text);
                        result = webRequest.downloadHandler.text;
                        Debug.Log($"{url} from remote");
                        Debug.Log(result);
                        PlayerPrefs.SetString(encodedURL, result);
                        PlayerPrefs.Save();
                        break;
                }
            }
        }
    }

    #if UNITY_EDITOR
    public void cache(){
        if(!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources")){
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
        }

        if(!UnityEditor.AssetDatabase.IsValidFolder($"Assets/Resources/{directory}")){
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources", directory);
        }

        var textAsset = new TextAsset(result);
        UnityEditor.AssetDatabase.CreateAsset(textAsset, UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"Assets/Resources/{directory}/{encodedURL}.asset"));
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
    #endif
}

public class curve_pos_lookat_scale{
    public AnimationCurve3D pos = new AnimationCurve3D();
    public AnimationCurve3D lookat = new AnimationCurve3D();
    public AnimationCurve3D scale = new AnimationCurve3D();

    public curve_pos_lookat_scale(Transform tr){
        for(var i = 0; i < tr.childCount; ++i){
            var c = tr.GetChild(i);
            var time = 1.0f * i / (tr.childCount - 1);
            pos.addKey(time, c.position);
            lookat.addKey(time, c.rotation * Vector3.forward);
            scale.addKey(time, c.localScale);
        }
    }

    public curve_pos_lookat_scale(params (Vector3 p, Quaternion r, Vector3 s)[] prs_arr){
        for(var i = 0; i < prs_arr.Length; ++i){
            var time = 1.0f * i / (prs_arr.Length - 1);
            var prs = prs_arr[i];
            pos.addKey(time, prs.p);
            lookat.addKey(time, prs.r * Vector3.forward);
            scale.addKey(time, prs.s);
        }
    }

    public IEnumerator progress(Transform tr, float duration){
        return progress(tr, duration, new FromToF(0, 1));
    }

    public IEnumerator progress(Transform tr, float duration, FromToF range01){
        return Extensions.progress(duration, progress => {
            progress = range01.lerpUnclamped(progress);
            sample(tr, progress);
        });
    }

    public void sample(Transform tr, float progress){
        tr.position = pos.evaluate(progress);
        tr.rotation = Quaternion.LookRotation(lookat.evaluate(progress), Vector3.up);
        tr.localScale = scale.evaluate(progress);
    }
}

public class UITracker3D: MonoBehaviour{
    public Transform anchor;
    public Camera cam;
    public RectTransform rt;

    void Start(){
        rt = GetComponent<RectTransform>();

    }
    
    void Update(){
        if(anchor != null){
            rt.track3D(cam, anchor);
        }
        else{
            GameObject.Destroy(gameObject);
        }
    }
}

public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField, HideInInspector]
	private List<TKey> keyData = new List<TKey>();
	
	[SerializeField, HideInInspector]
	private List<TValue> valueData = new List<TValue>();

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
		this.Clear();
		for (int i = 0; i < this.keyData.Count && i < this.valueData.Count; i++)
		{
			this[this.keyData[i]] = this.valueData[i];
		}
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
		this.keyData.Clear();
		this.valueData.Clear();

		foreach (var item in this)
		{
			this.keyData.Add(item.Key);
			this.valueData.Add(item.Value);
		}
    }
}

public class selection: CustomYieldInstruction{
    Dictionary<Button, Boxed<bool>> btn2clicked;
    public selection(params Button[] buttons){
        btn2clicked = buttons.ToDictionary(b => b, b => b.clicked());
    }

    public Button selected => btn2clicked.First(kv => kv.Value).Key;

    public override bool keepWaiting{get{
        return btn2clicked.Values.All(v => !v);
    }}
}