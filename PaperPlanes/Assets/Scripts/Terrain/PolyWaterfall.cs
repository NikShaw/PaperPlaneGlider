using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class PolyWaterfall : MonoBehaviour {

    // Public
    public bool bStart = false;
    public int iWidth = 3;
    public float fDensity = 15f;
    public List<GameObject> lstPoints = new List<GameObject>();

    // Private
    private MeshFilter meshFilter;

	// Use this for initialization
	void InitStart () {
        meshFilter = this.GetComponent<MeshFilter>();
        GenerateMesh();

    }
	
	// Update is called once per frame
	void Update () {
		if (bStart) {
            bStart = false;
            InitStart();
        }

    }

    private void GenerateMesh(){
        int sizeY = lstPoints.Count;
        int sizeX = iWidth;
        //List<List<Vector3>> tmpPointList = new List<List<Vector3>>();

        Vector3[] vertices = new Vector3[((sizeX * sizeY))];

        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(((sizeX - 1) * (sizeY - 1)) * 2 * 3)];
        int currTri = 0;
        int currVert = 0;

        for (int y = 0; y < sizeY; y++) {
            //tmpPointList.Add(new List<Vector3>());
            for(int x = 0; x < sizeX; x++) {
                Vector3 vPoint = lstPoints[y].transform.localPosition;
                vPoint.x += x * ((sizeX/2f * fDensity));
                Debug.Log("(" + x + ", " + y + ")" + vPoint);

                vertices[currVert] = vPoint;

                if (x % 2 == 0) {
                    uvs[currVert].x = 1;// x / (numPointsX - 0);
                } else {
                    uvs[currVert].x = 0;// x / (numPointsX - 0);
                }
                if (y % 2 == 0) {
                    uvs[currVert].y = 1;// y / (numPointsY - 0);
                } else {
                    uvs[currVert].y = 0;// y / (numPointsY - 0);
                }

                if ((x < (sizeX - 1)) && (y < (sizeY - 1))) {
                    triangles[currTri + 2] = sizeX + currVert;
                    triangles[currTri + 1] = currVert + 1;
                    triangles[currTri + 0] = currVert;

                    triangles[currTri + 5] = sizeX + currVert;
                    triangles[currTri + 4] = sizeX + currVert + 1;
                    triangles[currTri + 3] = currVert + 1;
                    currTri += 6;
                }
                currVert++;
            }
        }

        currVert = 0;
        // Create mesh
       // for (int x = 0; x < triangles.Length; x++) {                                            // Generate mesh
                //vertices[(y * sizeX) + x] = lstPoints[x][y];

                /*if (x % 2 == 0) {
                    uvs[(y * sizeX) + x].x = 1;// x / (numPointsX - 0);
                } else {
                    uvs[(y * sizeX) + x].x = 0;// x / (numPointsX - 0);
                }
                if (y % 2 == 0) {
                    uvs[(y * sizeX) + x].y = 1;// y / (numPointsY - 0);
                } else {
                    uvs[(y * sizeX) + x].y = 0;// y / (numPointsY - 0);
                }*/

                /*  if ((x < (sizeX - 1)) && (y < (sizeY - 1))) {
                      triangles[currTri + 0] = sizeX + currVert;
                      triangles[currTri + 1] = currVert + 1;
                      triangles[currTri + 2] = currVert;
                      triangles[currTri + 3] = sizeX + currVert;
                      triangles[currTri + 4] = sizeX + currVert + 1;
                      triangles[currTri + 5] = currVert + 1;
                      currTri += 6;
                  }
                  currVert++;*/
                //  }

                Mesh tmpMesh = new Mesh();
        tmpMesh.name = this.transform.name + " MESH";
        tmpMesh.vertices = vertices;
        tmpMesh.triangles = triangles;
        tmpMesh.uv = uvs;
        tmpMesh.RecalculateBounds();
        tmpMesh.RecalculateNormals();

        meshFilter.sharedMesh = tmpMesh;

    }
}
