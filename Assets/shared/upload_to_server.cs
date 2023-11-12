using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Collections.Generic;
public class upload_to_server{

    [DllImport("__Internal")]
    private static extern void PostLog(string itemCode, string value);

    public static void number(string name, int value){
        Debug.Log($"upload {name}, {value}");

        // enclose to hide exception. uploading can fail whenever.
        try{
            if(Application.platform == RuntimePlatform.WebGLPlayer && Application.internetReachability != NetworkReachability.NotReachable){
                PostLog(name, value.ToString());
            }
        }
        catch(System.Exception e){
            Debug.LogError(e);
        }
    }
}