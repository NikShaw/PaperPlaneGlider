using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustBillboard : MonoBehaviour {

    void OnWillRenderObject() {
        if(Camera.current != null) {
            this.transform.LookAt(Camera.current.transform);
        } else {
            this.transform.LookAt(Camera.main.transform);
        }
    }
}
