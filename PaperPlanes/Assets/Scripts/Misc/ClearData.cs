using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ClearData : MonoBehaviour {

    // Public
    public bool bClear = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (bClear) {
            PlayerPrefs.DeleteAll();
            bClear = false;
        }
	}
}
