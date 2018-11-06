using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {

    // public

    // private
    private GameObject gTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (gTarget != null) {
            this.transform.position = Vector3.Lerp(this.transform.position, gTarget.transform.position, 0.4f);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, gTarget.transform.rotation, 0.2f);
            if(WaterControl.ctrl != null) {
                if(this.transform.position.y < WaterControl.ctrl.GetWaterHeight()) {
                    this.transform.position = new Vector3(this.transform.position.x, WaterControl.ctrl.GetWaterHeight(), this.transform.position.z);
                    this.transform.LookAt(gTarget.transform.position + (gTarget.transform.forward * 5f));
                }
            }
        }
	}

    public void SetTarget(GameObject targ) {
        gTarget = targ;
    }
}
