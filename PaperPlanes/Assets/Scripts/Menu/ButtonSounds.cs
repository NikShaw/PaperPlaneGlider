using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour {

    public static ButtonSounds sounds;
    public float fClickVol = 1f;
    public float fWhooshVol = 0.4f;
    public float fCheckVol = 0.1f;
    public float fDingVol = 0.5f;
    public AudioClip audClick;
    public AudioClip audWhoosh;
    public AudioClip audCheck;
    public AudioClip audDing;

    private AudioSource audSrc;

    private void Awake() {
        if(sounds == null) {
            sounds = this;
        } else {
            DestroyImmediate(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        audSrc = this.GetComponent<AudioSource>();
    }

    public void PlayClick() {
        audSrc.PlayOneShot(audClick, fClickVol);
    }

    public void PlayWhoosh() {
        audSrc.PlayOneShot(audWhoosh, fWhooshVol);
    }

    public void PlayCheck() {
        audSrc.PlayOneShot(audCheck, fCheckVol);
    }

    public void PlayDing() {
        audSrc.PlayOneShot(audDing, fDingVol);
    }
}
