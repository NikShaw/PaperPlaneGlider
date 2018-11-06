using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    // public
    public GameObject gActive;
    public GameObject gInactive;
    public GameObject gNext;

    // private
    private bool bActive = false;
    private bool bNext = false;
    private bool bPlayerHit = false;

	// Use this for initialization
	public void Init() {
        DisableObjs();
        gInactive.SetActive(true);
        bPlayerHit = false;
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void DisableObjs() {
        gActive.SetActive(false);
        gInactive.SetActive(false);
        gNext.SetActive(false);
    }

    public void SetNext() {
        bNext = true;
        DisableObjs();
        gNext.SetActive(true);
    }

    public void SetActive() {
        bActive = true;
        bNext = false;
        DisableObjs();
        gActive.SetActive(true);
        gNext.SetActive(true);
    }

    public void Disable() {
        bPlayerHit = false;
        DisableObjs();
    }

    public bool PlayerHit() {
        return bPlayerHit;
    }

    void OnTriggerEnter(Collider col) {
        if (bActive) {
            if (col.tag == "Player") {
                bPlayerHit = true;
            }
        }
        //Debug.Log("hit " + col.tag);
    }
}
