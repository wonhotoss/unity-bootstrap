using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hint_balloon : MonoBehaviour
{
    public class hint_data{
        public RectTransform anchor;
        public string[] messages;

        public static hint_data create(RectTransform anchor, params string[] messages){
            return new hint_data(){
                anchor = anchor,
                messages = messages,
            };
        }
    }

    // TODO: make not nullable struct, and declare no hint value
    public hint_data hint{get; set;} = null;

    public IEnumerator track(){
        while(true){
            if(hint != null && hint.anchor != null){
                transform.position = hint.anchor.position; 
            }
            yield return null;
        }
    }
    public IEnumerator update(){
        var text = transform.GetComponentInChildren<Text>();
        var layoutRoot = text.transform.parent.GetComponent<RectTransform>();
        while(true){
            // this coroutine starts before component stars, ensure deavtivates.
            while(hint == null || !enabled){
                transform.gameObject.SetActive(false);
                yield return null;
            }
            
            transform.gameObject.SetActive(true);
            
            var current_hint = hint;
            var new_hint = Linq.Func(() => current_hint != hint || !enabled);

            while(!new_hint()){
                for(var i = 0; i < current_hint.messages.Length && !new_hint(); ++i){
                    var sentence = current_hint.messages[i];
                    for(var j = 0; j < sentence.Length && !new_hint(); ++j){
                        text.text = sentence.Substring(0, j + 1);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
                        yield return new WaitForSeconds(0.1f);
                    }

                    var S1 = new FromNow(1.0f);
                    while(!S1.Expired && !new_hint()){
                        yield return null;
                    }
                }

                var S3 = new FromNow(3.0f);
                while(!S3.Expired && !new_hint()){
                    yield return null;
                }
            }
        }
    }
}
