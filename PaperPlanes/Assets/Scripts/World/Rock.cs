using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Rock : MonoBehaviour {

    public int id = 0;
    public List<int> idCanConnectTo = new List<int>();

    void Start() {
        this.GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        //this.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
    }

	public List<int> Connecting() {
        return idCanConnectTo;
    }
}
