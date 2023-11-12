﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

// public static class ext{
//     public static Transform findRecursive(this Transform t, string n){
//         return null;
//     }

//     public static Transform add(this Transform t, Transform n){
//         return null;
//     }

//     public static Transform[] getChildren(this Transform t){
//         return null;
//     }
// }

public class Holder<T>{
    public readonly string Key;
    System.Func<T> getter;
    System.Action<T> setter;
    
    public Holder(string key, System.Func<T> getter, System.Action<T> setter){
        this.Key = key;
        this.getter = getter;
        this.setter = setter;
    }

    public T Value{
        get{ return getter(); }
        set{ setter(value); PlayerPrefs.Save(); }
    }
}
// TODO: where enum
public class Selection<T> : Holder<string>{
    public readonly string[] candidates;
    public Selection(string key, T defValue): base(
        key,
        () => PlayerPrefs.GetString(key, System.Enum.GetName(typeof(T), defValue)),
        (v) => PlayerPrefs.SetString(key, v)){
        candidates = System.Enum.GetNames(typeof(T));
    }
    public T enumValue{
        get{ return (T)System.Enum.Parse(typeof(T), this.Value, true); }
        set{ this.Value = System.Enum.GetName(typeof(T), value); }
    }
}

public class FloatRange : Holder<float>{
    public readonly float min;
    public readonly float max;
    public FloatRange(string key, float defValue, float min, float max): base(
        key,
        () => PlayerPrefs.GetFloat(key, defValue),
        (v) => PlayerPrefs.SetFloat(key, v)){
        this.min = min;
        this.max = max;
    }
}

public class IntRange : Holder<int>{
    public readonly int min;
    public readonly int max;
    public IntRange(string key, int defValue, int min, int max): base(
        key,
        () => PlayerPrefs.GetInt(key, defValue),
        (v) => PlayerPrefs.SetInt(key, v)){
        this.min = min;
        this.max = max;
    }
}

public class setting_editor: MonoBehaviour
{
    public Transform popup;
    public Button toggle;
    public Button reset;

    public static void clear(){
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    void Awake(){
        toggle.onClick.AddListener(() => {
            popup.gameObject.SetActive(!popup.gameObject.activeSelf);
            reset.gameObject.SetActive(!reset.gameObject.activeSelf);
            Time.timeScale = popup.gameObject.activeSelf ? 0.0f : 1.0f;
        });

        popup.gameObject.SetActive(false);
        reset.gameObject.SetActive(false);

        reset.onClick.AddListener(() => {
            foreach(var children in popup.getChildren().Where(c => c.gameObject.activeSelf))
            {
                GameObject.Destroy(children.gameObject);
            }
            clear();


            // TODO
            // resetSettings();
        });
    }

    public void makeSelector(Holder<string> String, string[] values){
        var selectorTemplate = popup.findRecursive("selector");
        var selector = popup.add(selectorTemplate);
        selector.gameObject.SetActive(true);
        selector.findRecursive("name").GetComponent<Text>().text = String.Key;
        var value = String.Value;
        var dropdown = selector.findRecursive("values").GetComponent<Dropdown>();
        dropdown.options = values.Select(v => new Dropdown.OptionData(v)).ToList();
        dropdown.value = System.Array.IndexOf(values, value);
        dropdown.onValueChanged.AddListener(index => {
            String.Value = values[index];
        });
    }

    
    (Text text, Button inc, InputField input, Button dec) bindNumber(){
        var numberTemplate = popup.findRecursive("number");
        numberTemplate.gameObject.SetActive(false);
        var number = popup.add(numberTemplate);
        number.gameObject.SetActive(true);
        return (
            number.findRecursive("name").GetComponent<Text>(),
            number.findRecursive("inc").GetComponent<Button>(),
            number.findRecursive("input").GetComponent<InputField>(),
            number.findRecursive("dec").GetComponent<Button>()
        );
    }

    public void makeFloatInput(FloatRange Float){
        var bind = bindNumber();
        bind.input.contentType = InputField.ContentType.DecimalNumber;
        bind.text.text = Float.Key;
        System.Action<float> setValue = val => {
            Float.Value = Mathf.Clamp(val, Float.min, Float.max);
            bind.input.text = Float.Value.ToString();
        };
        bind.dec.onClick.AddListener(() => setValue(Float.Value - 0.1f));
        bind.inc.onClick.AddListener(() => setValue(Float.Value + 0.1f));
        bind.input.onValueChanged.AddListener(str => setValue(float.Parse(str)));
        setValue(Float.Value);  
    }

    public void makeIntInput(IntRange Int){
        var bind = bindNumber();
        bind.input.contentType = InputField.ContentType.IntegerNumber;
        bind.text.text = Int.Key;
        System.Action<int> setValue = val => {
            Int.Value = (int)Mathf.Clamp(val, Int.min, Int.max);
            bind.input.text = Int.Value.ToString();
        };
        bind.dec.onClick.AddListener(() => setValue(Int.Value - 1));
        bind.inc.onClick.AddListener(() => setValue(Int.Value + 1));
        bind.input.onValueChanged.AddListener(str => setValue(int.Parse(str)));
        setValue(Int.Value);
    }
}
