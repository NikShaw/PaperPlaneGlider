using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WallObj : MonoBehaviour {

    // public

    // private
    private MeshFilter meshFilt;
    private Mesh mesh;

	// Use this for initialization
	void Start () {
    }

    public void SetVerticesZ(List<float> lstPos) {

        meshFilt = this.transform.GetChild(0).GetComponent<MeshFilter>();
        mesh = meshFilt.mesh;
        Vector3[] verts = mesh.vertices;

        verts[0].z = lstPos[0];
        verts[1].z = lstPos[1];
        verts[verts.Length - 2].z = lstPos[2];
        verts[verts.Length - 1].z = lstPos[3];

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        this.transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
