using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sounder : MonoBehaviour
{
    AudioSource playingBGM;
    
    public void playBGM(AudioClip clip, bool loop = true){
        if(playingBGM != null){
            AudioSource.Destroy(playingBGM);
        }
        playingBGM = gameObject.AddComponent<AudioSource>();
        playingBGM.clip = clip;
        playingBGM.loop = loop;
        playingBGM.spatialBlend = 0.0f;
        playingBGM.Play();
    }

    public void playEffect(AudioClip clip){
        var AS = gameObject.AddComponent<AudioSource>();
        AS.clip = clip;
        AS.spatialBlend = 0.0f;
        AS.Play();
        this.after(clip.length, () => AudioSource.Destroy(AS));
    }
}
