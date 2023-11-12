using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emoji_balloon : MonoBehaviour
{
    public Transform good;
    public Transform bad;

    Vector3 screen_point;
    List<emoji_balloon> siblings = new List<emoji_balloon>();

    public emoji_balloon instanciate_good(){
        var instance = transform.parent.add(this);
        siblings.Add(instance);
        instance.gameObject.SetActive(true);
        instance.good.showExclusve();
        instance.good.getChildren().pickRandom().showExclusve();
        instance.siblings = siblings;
        return instance;
    }

    public emoji_balloon instanciate_bad(){
        var instance = transform.parent.add(this);
        siblings.Add(instance);
        instance.gameObject.SetActive(true);
        instance.bad.showExclusve();
        instance.bad.getChildren().pickRandom().showExclusve();
        instance.siblings = siblings;
        return instance;
    }
    
    public void track(Camera camera, Transform anchor){
        var rt = GetComponent<RectTransform>();
        IEnumerator update(){
            while(anchor != null && anchor.gameObject.activeInHierarchy){
                screen_point = camera.WorldToScreenPoint(anchor.position);
                rt.anchorMax = rt.anchorMin = new Vector2(
                    screen_point.x / camera.pixelWidth,
                    screen_point.y / camera.pixelHeight
                );
                rt.offsetMin = rt.offsetMax = Vector2.zero;

                // TODO: sort once for all siblings
                siblings.Sort((x, y) => (int)(y.screen_point.z - x.screen_point.z));
                for(var i = 0; i < siblings.Count; ++i){
                    siblings[i].transform.SetSiblingIndex(i);
                }

                yield return null;
            }
            GameObject.Destroy(gameObject);
            siblings.Remove(this);
        }
        StartCoroutine(update());

        const float duration = 1.0f / 3.0f;

        IEnumerator scale(){
            var from = Time.time;
            
            while(true){
                var zoom = Mathf.Min(1.0f, (Time.time - from) / duration);
                var heartbeat = Time.time % (duration * 3);
                if(heartbeat < duration * 2){
                    heartbeat = heartbeat % duration;
                    heartbeat = Mathf.Sin(heartbeat / duration * Mathf.PI);
                }
                else{
                    heartbeat = 0.0f;
                }
                heartbeat = Mathf.Lerp(0.8f, 1.0f, heartbeat);
                rt.localScale = Vector3.one * zoom * heartbeat;
                yield return null;
            }
        }

        StartCoroutine(scale());
    }
}
