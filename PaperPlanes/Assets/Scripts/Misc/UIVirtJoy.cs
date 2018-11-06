using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVirtJoy : MonoBehaviour {

    // public
    public float fMaxDist = 1.0f;
    public float fMaxTouch = 3.0f;
    public GameObject gJoy;

    // private
    private int iCurrFinger = -1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetInput(Vector2 inp) {
        gJoy.transform.localPosition = inp *= fMaxDist;
    }

    public Vector2 GetInput() {
        Vector2 inp = Vector2.zero;
        Vector3 vTouch = GetTouch();
        Vector3 vTouchDir = (vTouch -this.transform.position)/1.5f;
        if (vTouchDir.magnitude > fMaxDist) {
            vTouchDir = vTouchDir.normalized * fMaxDist;
            gJoy.transform.localPosition = vTouchDir;
        } else {
            gJoy.transform.localPosition = vTouchDir;
        }
        if (vTouch != Vector3.zero) {
            inp = vTouchDir / fMaxDist;
            if(inp.magnitude > 1f) {
                inp /= inp.magnitude;
            }
        } else {
            gJoy.transform.localPosition = Vector3.zero;
        }

        return inp;
    }

    private Vector3 GetTouch() {
        for(int i = 0; i < Input.touchCount; i++) {
            Touch tmpTouch = Input.GetTouch(i);
            Vector3 vTouch = Camera.main.ScreenToWorldPoint(tmpTouch.position);
            Vector3 vTouchDir = (new Vector3(tmpTouch.position.x, tmpTouch.position.y, 0f) - this.transform.position);
            if (vTouchDir.magnitude < fMaxTouch) {
                iCurrFinger = i;
                return tmpTouch.position;
            }
        }
        return Vector3.zero;
    }

    public void Reset() {
        gJoy.transform.localPosition = Vector3.zero;
    }
}
