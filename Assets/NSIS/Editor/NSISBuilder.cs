using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;

public class NSISBuilder : EditorWindow
{
    [MenuItem("Window/NSIS")]
    public static void ShowExample()
    {
        var wnd = GetWindow<NSISBuilder>();
        wnd.titleContent = new GUIContent("NSIS");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy
        Label label = new Label("Hello World!");
        root.Add(label);

        // Create button
        Button button = new Button();
        button.name = "button";
        button.text = "Button";
        root.Add(button);      
        button.clicked += () => {
            Debug.Log("clicked!!");
            make_script();

        };

        // Create toggle
        Toggle toggle = new Toggle();
        toggle.name = "toggle";
        toggle.label = "Toggle";
        root.Add(toggle);
    }

    void make_script(){
        var NSIS_directory = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "NSISPortable");

        // select build directory
        var build_directory = EditorUtility.OpenFolderPanel(
            "Select directory contains build",
            Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "build"),
            "windows"
        );

        if(string.IsNullOrEmpty(build_directory)){
            Debug.LogError("build_directory not set");
            return;
        }

        Debug.Log(build_directory);

        // collect files
        var directories = new Dictionary<string, List<string>>();

        void collect_files_recursive(string dir){ 
            directories[dir] = new List<string>();
            directories[dir].AddRange(Directory.GetFiles(dir));
            foreach(var sd in Directory.GetDirectories(dir)){
                collect_files_recursive(sd);
            }
        }

        collect_files_recursive(build_directory);
        foreach(var d in directories.Keys){
            Debug.Log(directories[d].report(d));
        }      

        // load script template  
        var script_template = File.ReadAllText(Path.Combine(NSIS_directory, "script_template.nsi"));
        Debug.Log(script_template);        

        // select license location
        var license_path = EditorUtility.OpenFilePanel(
            "Select license file(txt)",
            NSIS_directory,
            "txt"
        );
        if(string.IsNullOrEmpty(license_path)){
            Debug.LogError("license_path not set");
            return;
        }

        // select script location
        var save_script_path = EditorUtility.SaveFilePanel(
            "Select path to save script",
            Directory.GetParent(build_directory).ToString(),
            "NSIS_script",
            "nsi"
        );
        if(string.IsNullOrEmpty(save_script_path)){
            Debug.LogError("save_script_path not set");
            return;
        }
        var save_script_directory = Directory.GetParent(save_script_path).ToString();

        // replace file location with relative ones
        // var rel_directories = directories
        //     .Select(kv => (k: kv.Key, v: kv.Value.Select(path => Path.GetRelativePath(save_script_path, path)).ToList()))
        //     .ToDictionary(kv => kv.k, kv => kv.v);

        // foreach(var d in rel_directories.Keys){
        //     Debug.Log(rel_directories[d].report(d));
        // }

        // foreach(var d in rel_directories.Keys){
        //     var install_directory = Path.GetRelativePath(build_directory, d);
        //     Debug.Log(rel_directories[d].report(install_directory));
        // }

        // replace placeholders
        var product_name = "sample";
        var company_name = "samplecompany";
        var version="0.0.1";
        var website="www.naver.com";
        var license = Path.GetRelativePath(save_script_directory, license_path);

        var header = 
            $"!define PRODUCT_NAME \"{product_name}\"\n" +
            $"!define PRODUCT_VERSION \"{version}\"\n" +
            $"!define PRODUCT_PUBLISHER \"{company_name}\"\n" +
            $"!define PRODUCT_WEB_SITE \"{website}\"\n" +
            $"!define LICENSE_PATH \"{license}\"\n";

        var install = "";
        foreach(var d in directories.Keys){
            var files = directories[d];
            if(files.Count > 0){
                install += $"SetOutPath \"$INSTDIR\\{Path.GetRelativePath(build_directory, d)}\"" + "\n";
                foreach(var f in files){
                    install += $"File \"{Path.GetRelativePath(save_script_directory, f)}\"" + "\n";
                }
            }
        }

        var uninstall = "";
        foreach(var d in directories.Keys){
            foreach(var f in directories[d]){
                uninstall += $"Delete \"$INSTDIR\\{Path.GetRelativePath(build_directory, f)}\"" + "\n";
            }
        }

        foreach(var d in directories.Keys){
            if(directories[d].Count > 0){
                uninstall += $"RMDir \"$INSTDIR\\{Path.GetRelativePath(build_directory, d)}\"" + "\n";
            }
        }

        // save script
        var script = script_template
            .Replace(";HEADER_PLACEHOLDER", header)
            .Replace(";INSTALL_PLACEHOLDER", install)
            .Replace(";UNINSTALL_PLACEHOLDER", uninstall);

        // TODO: UTF-8 with BOM
        File.WriteAllText(save_script_path, script);
        
    }

    void run_script(){

    }
}

