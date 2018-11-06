using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCOM : MonoBehaviour {

    // public
    public GameObject gRigid;

	// Use this for initialization
	void Start () {
        gRigid.GetComponent<Rigidbody>().centerOfMass = this.transform.localPosition;
	}
}
