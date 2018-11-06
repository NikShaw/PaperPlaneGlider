using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdraftAnim : MonoBehaviour {

    // public
    public float fAnimTime = 2f;
    public float fFadeInTime = 0.2f;
    public float fSpeedMult = 4.0f;
    public Vector3 vInitSize = Vector3.one;

    // private
    private float fAnimTimer = 0f;
    private float fFadeInTimer = 0f;
    public float fForce = 0f;
    private float fDist = 0f;
    private float fDist1 = 0f;
    private float fDist2 = 0f;
    private bool bActive = false;
    private Vector3 vUp;
    private Vector3 vMin;
    private Vector3 vMax;
    private SpriteRenderer render;

	// Use this for initialization
	void Start () {
        render = this.GetComponent<SpriteRenderer>();
    }

    public void StartAnim(Vector3 up){//, Vector3 min, Vector3 max) {
        //  if(render == null) {
        //  render = this.GetComponent<SpriteRenderer>();
        //  }
      //  if(this.transform.position != null){
           // Vector3 viewportPoint = Camera.main.WorldToViewportPoint(pos);
          //  if(viewportPoint.z > 0f){
                vUp = up.normalized;
                bActive = true;
                fAnimTimer = 0f;
                fFadeInTimer = 0f;
                fForce = up.magnitude;
            //    vMin = min;
              //  vMax = max;
            //    return true;
          //  }
      //  }
      //  return false;
       // render.color = new Color(render.color.r, render.color.g, render.color.b, 0f);
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (bActive) {
            fAnimTimer += Time.deltaTime;
            if(fAnimTimer > fAnimTime) {
                fAnimTimer = 0f;
                bActive = false;
                this.gameObject.SetActive(false);
            }
            this.transform.LookAt(Camera.main.transform);
            this.transform.up = vUp;
           // fDist1 = (this.transform.position - vMin).magnitude;
           // fDist2 = (this.transform.position - vMax).magnitude;
            //fDist = fDist1 + fDist2;
            //float perc = fDist2/fDist;
            this.transform.position += this.transform.up * fSpeedMult * fForce * Time.deltaTime;
            if (fFadeInTimer < fFadeInTime) {
                fFadeInTimer += Time.deltaTime;
            } else {
                fFadeInTimer = fFadeInTime;
            }

            this.transform.localScale = vInitSize * Mathf.Lerp(fFadeInTimer / fFadeInTime, 0f, fAnimTimer / fAnimTime) * fForce;
            //render.color = new Color(render.color.r, render.color.g, render.color.b, Mathf.Lerp(fFadeInTimer/fFadeInTime, 0f, fAnimTimer/fAnimTime));
        }
	}
}
