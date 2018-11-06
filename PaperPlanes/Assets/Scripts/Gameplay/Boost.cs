using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour {

    // public
    public float fBoostForce = 20.0f;
    public float fChargeTime = 5f;
    public GameObject gActive;
    public GameObject gInactive;

    // private
    private float fChargeTimer = 0f;
    private bool bActive = true;

    // Use this for initialization
    void Start () {
        ShowActive(true);
	}
	
	// Update is called once per frame
	void Update () {
        if (!bActive) {
            fChargeTimer += Time.deltaTime;
            if(fChargeTimer > fChargeTime) {
                fChargeTimer = 0f;
                bActive = true;
                ShowActive(true);
            }
        }	
	}

    void OnTriggerEnter(Collider col) {
        if (bActive) {
            if (col.tag == "Player") {
                col.attachedRigidbody.AddForce(col.transform.forward * fBoostForce, ForceMode.Impulse);
                bActive = false;
                ShowActive(false);
                if(ButtonSounds.sounds != null) {
                    ButtonSounds.sounds.PlayWhoosh();
                }
            }
        }
        //Debug.Log(col.tag);
    }

    private void ShowActive(bool a) {
        if (a) {
            gActive.SetActive(true);
           // gInactive.SetActive(false);
        } else {
            gActive.SetActive(false);
          //  gInactive.SetActive(true);
        }
    }
}
