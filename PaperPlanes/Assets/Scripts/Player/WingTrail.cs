using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingTrail : MonoBehaviour {

    // public
    public Vector2 vMaxVel = new Vector2(5f, 20f);
    public Vector2 vStartWidth = Vector2.one;

    // private
    private Vector3 vPrevPos = Vector3.zero;
    public float fVel = 0f;
    private TrailRenderer trail;

	// Use this for initialization
	void Start () {
        trail = this.GetComponent<TrailRenderer>();
        vPrevPos = this.transform.position;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        fVel = (this.transform.position - vPrevPos).magnitude/Time.fixedDeltaTime;
        vPrevPos = this.transform.position;
        if (fVel > vMaxVel.x) {
            trail.widthMultiplier = Mathf.Lerp(vStartWidth.x, vStartWidth.y, (fVel - vMaxVel.x) / (vMaxVel.y - vMaxVel.x));
        } else {
            trail.widthMultiplier = 0f;
        }
	}
}
