using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public interface IRevision{
    int revision{get;}
}

public interface IKV<T>{
    public string K{get;}
    public T V{get; set;}
}

public interface IRANGE<T>{
    public T min{get;}
    public T max{get;}
}

// TODO: load playerprefs on first get or set
public class PPBase<T> : IRevision, IKV<T>{
    public string K{ get; private set;}
    T def;
    Action<string, T> setter;
    Func<string, T, T> getter;
    T cache;
    
    public PPBase(string _key, Action<string, T> _setter, Func<string, T, T> _getter, T _def){
        K = _key;
        def = _def;
        setter = _setter;
        getter = _getter;
        cache = getter(K, def);
    }

    public int revision{get; private set;} = 0;

    public T V{
        get => cache;
        set{
            cache = value;
            setter(K, value);
            revision = revision + 1;
        }
    }

    public void delete(){
        PlayerPrefs.DeleteKey(K);
    }
}

public class PPN : PPBase<int>, IRANGE<int>{
    public int min{get; private set;}
    public int max{get; private set;}
    public PPN(string key, int min = int.MinValue, int max = int.MaxValue): base(key, PlayerPrefs.SetInt, PlayerPrefs.GetInt, 0) { 
        this.min = min;
        this.max = max;
    }
}

public class PPS : PPBase<string>{
    public PPS(string key, string def = ""): base(key, PlayerPrefs.SetString, PlayerPrefs.GetString, "") { 
    }
}

public class PPE<T> : PPBase<T> where T : Enum{
    public PPE(string key, T def = default(T)): base(key, setter, getter, def) { 
    }

    static T getter(string key, T def){
        return (T)Enum.Parse(typeof(T), PlayerPrefs.GetString(key, Enum.GetName(typeof(T), def)));
    }
    
    static void setter(string key, T value){
        PlayerPrefs.SetString(key, Enum.GetName(typeof(T), value));
    }
}

public class PPF : PPBase<float>, IRANGE<float>{
    public float min{get; private set;}
    public float max{get; private set;}
    public PPF(string key, float def = 0.0f, float min = float.MinValue, float max = float.MaxValue): base(key, PlayerPrefs.SetFloat, PlayerPrefs.GetFloat, def) { 
        this.min = min;
        this.max = max;
    }
}

public class PP01 : PPBase<bool>{
    public PP01(string key): base(
        key, 
        (k, b) => PlayerPrefs.SetInt(k, b ? 0 : 1), 
        (k, d) => PlayerPrefs.GetInt(k, d ? 0 : 1) == 0,
        true
    ) { 
    }
}

public class PPJSON<T> : PPBase<T>{
    public string serialized{get; private set;}
    public PPJSON(string key, T def): base(key, setter, getter, def){ 
    }

    static T getter(string key, T def){
        return JsonUtility.FromJson<W>(PlayerPrefs.GetString(key, JsonUtility.ToJson(new W(){v = def}, true))).v;
    }
    
    static void setter(string key, T value){
        PlayerPrefs.SetString(key, JsonUtility.ToJson(new W(){v = value}, true));
    }

    [Serializable]
    private class W{
        public T v;
    }
}

public class PPColor : PPBase<Color>{

    public PPColor(string key): this(key, Color.white){
    } 

    public PPColor(string key, Color @default): base(
        key, 
        (k, c) => PlayerPrefs.SetString(k, c.to_json()), 
        (k, d) => new Color().from_json(PlayerPrefs.GetString(k, d.to_json())),
        @default
    ) { 
    }
}

public static class PPExtensions{
    public static string to_json(this Color color){
        return JsonUtility.ToJson(color);
    }

    public static Color from_json(this Color color, string json){
        var parsed = JsonUtility.FromJson<Color>(json);
        color.r = parsed.r;
        color.g = parsed.g;
        color.b = parsed.b;
        color.a = parsed.a;
        return color;
    }

    // normally at OnEnable()
    public static void subscribe<T>(this MonoBehaviour host, PPBase<T> data, Action<T> update){        
        var revision = data.revision;
        update(data.V);

        IEnumerator co(){
            while(true){
                yield return new WaitUntil(() => revision != data.revision);
                revision = data.revision;
                update(data.V);
            }
        }
        host.StartCoroutine(co());
    }

    public static void subscribe(this MonoBehaviour host, Action update, params IRevision[] datas){        
        var revisions = datas.Select(d => d.revision).ToArray();
        update();

        IEnumerator co(){
            while(true){
                yield return new WaitUntil(() => datas.Where((d, i) => d.revision != revisions[i]).Any());
                revisions = datas.Select(d => d.revision).ToArray();
                update();
            }
        }
        host.StartCoroutine(co());
    }
}