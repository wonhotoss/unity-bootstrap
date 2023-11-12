using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_fullscreen : MonoBehaviour{
    RectTransform last_top;
    RectTransform last_bottom;
    public IEnumerator replace_top(RectTransform top){
        if(last_top != null){
            Debug.Assert(last_top.gameObject.activeInHierarchy);
            yield return last_top.slide_out_up();
            last_top.off();
            last_top = null;
        }

        if(top != null){
            last_top = top;
            Debug.Assert(!last_top.gameObject.activeSelf);
            last_top.on();
            yield return last_top.slide_in_down();
        }
    }

    public IEnumerator replace_bottom(RectTransform bottom){
        if(last_bottom != null){
            Debug.Assert(last_bottom.gameObject.activeInHierarchy);
            yield return last_bottom.slide_out_down();
            last_bottom.off();
            last_bottom = null;
        }

        if(bottom != null){
            last_bottom = bottom;
            Debug.Assert(!last_bottom.gameObject.activeSelf);
            last_bottom.on();
            yield return last_bottom.slide_in_up();
        }
    }
}
