using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
//[RequireComponent(typeof(BoxCollider))]
public class ProcWall : MonoBehaviour {

    // public
    public float fRandomExtrudeAmt = 1.0f;
    public float fPointDensity = 1.0f;
    public float fCurveAng = 90f;
    public float fCurveRadius = 1.0f;
    public float fCurveDensity = 0.2f;
    public float fCurveTopLimit = 1.0f;
    public float fCurveTopAmount = 5.0f;
    public bool bGenerate = false;
    public bool bClear = false;
    public bool bSingleMesh = false;
    public GameObject gWallPre;
    public Vector2 vWallSize = Vector2.one;
    public Vector2 vWallAmount = Vector2.one;
    public Vector3 vWallScale = Vector3.one;

    // private
    public List<GameObject> lstWalls = new List<GameObject>();
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (bGenerate) {
            // try {
            if (!bSingleMesh) {
                GenerateWall();
            } else {
                GenerateWallSingle();
            }
            //} catch {
           //     Debug.Log("Failed to generate wall");
           // }
            bGenerate = false;
        }
        if (bClear) {
            ClearWalls();
            bClear = false;
        }
	}

    private void GenerateWallSingle() {
        meshFilter = this.transform.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
       // List<List<float>> lstWallZ = new List<List<float>>();
        Vector2 maxPoints = (vWallSize/fPointDensity) + new Vector2(1, 1);
        maxPoints = new Vector2(Mathf.Round(maxPoints.x), Mathf.Round(maxPoints.y));
        Vector2 vecSquares = maxPoints - new Vector2(1, 1);
        int numSquares = (int)vecSquares.x * (int)vecSquares.y;
        int numIndices = (((int)maxPoints.x - 1) * ((int)maxPoints.y - 1)) * 6;
        int numVerts = (int)maxPoints.x * (int)maxPoints.y;

        Vector3[] arrVerts = new Vector3[numVerts];
        Vector2[] arrUvs = new Vector2[numVerts];
        int[] arrInd = new int[numIndices];

        Debug.Log(maxPoints + " | " + vecSquares + " | " + numSquares + " | " + numVerts + " | " + numIndices);

        List<List<Vector3>> lstVertices = new List<List<Vector3>>();

        int currTri = 0;
        int currVert = 0;

        for (int x1 = 0; x1 < maxPoints.x; x1++) {
            lstVertices.Add(new List<Vector3>());
            for (int y2 = 0; y2 < maxPoints.y; y2++) {
                float randZ = Random.Range(0f, fRandomExtrudeAmt);
                float tmpCurve = fCurveAng;

                if(x1 <= 1){ //|| (x1 >= (maxPoints.x - 2))){
                    randZ = 0f;
                    //tmpCurve = 0f;
                } else if (x1 >= (maxPoints.x - 2)){
                    randZ = 0f;
                }

                float height = (y2 / (maxPoints.y - 1f)) * vWallSize.y;
                float width = (x1 / (maxPoints.x - 1f)) * vWallSize.x;
                float slopeOffset = 0.0f;

                if((fCurveTopLimit < 1.0f) && (fCurveTopLimit > 0.0f)) {
                    if((y2/maxPoints.y) > fCurveTopLimit) {
                        slopeOffset = ((y2/maxPoints.y - (fCurveTopLimit))) / (1f-fCurveTopLimit);
                        slopeOffset = Mathf.Clamp(slopeOffset, 0f, 1f);
                        slopeOffset /= slopeOffset / slopeOffset / slopeOffset;
                        slopeOffset *= fCurveTopAmount;
                    }
                }


                if (y2 == (maxPoints.y - 1)) {
                    height = 0f;
                    slopeOffset = fRandomExtrudeAmt;
                }

                Vector3 pos = new Vector3(width, height, randZ);
                Vector3 pos2 = Vector3.zero;
                if (tmpCurve > 0f) {
                    pos2 = new Vector3((maxPoints.x * vWallSize.x)/width, height, width);
                } else if (tmpCurve < 0f) {
                    pos2 = new Vector3((maxPoints.x * vWallSize.x)/width, height, -width);
                }

                float lerpAmt = ((x1) / (maxPoints.x - 1f)) * Mathf.Abs(tmpCurve);

                if (x1 <= 1) { //|| (x1 >= (maxPoints.x - 2))){
                    lerpAmt = 0f;
                    //tmpCurve = 0f;
                } else if (x1 >= (maxPoints.x - 2)) {
                    lerpAmt = 1f;
                }
                pos = Vector3.Lerp(pos, pos2, lerpAmt);

                arrVerts[((x1 * (int)maxPoints.y)) + y2] = pos;

                arrUvs[((x1 * (int)maxPoints.y)) + y2].x = x1 / (maxPoints.x - 1);
                arrUvs[((x1 * (int)maxPoints.y)) + y2].y = y2 / (maxPoints.y - 1);

                if ((x1 < ((int)maxPoints.x - 1)) && (y2 < ((int)maxPoints.y - 1))) {
                    //Debug.Log(currTri + " | " + triangles.Length + " | " + x + ", " + y + " | " + vNumPoints);
                    arrInd[currTri] = currVert;
                    arrInd[currTri + 1] = currVert + 1;
                    arrInd[currTri + 2] = (int)maxPoints.y + currVert;
                    arrInd[currTri + 3] = (int)maxPoints.y + currVert;
                    arrInd[currTri + 4] = currVert + 1;
                    arrInd[currTri + 5] = (int)maxPoints.y + currVert + 1;
                    currTri += 6;
                }
                currVert++;
            }
        }

        mesh.vertices = arrVerts;
        mesh.uv = arrUvs;
        mesh.triangles = arrInd;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.sharedMesh = mesh;

        BoxCollider box = this.GetComponent<BoxCollider>();
        if (box != null) {
            box.size = new Vector3(vWallSize.x, vWallSize.y, fRandomExtrudeAmt);
            box.center = box.size / 2f;
        }
        MeshCollider meshCol = this.GetComponent<MeshCollider>();
        if (meshCol != null) {
            meshCol.sharedMesh = mesh;
        }
    }

    private void GenerateWall() {
        ClearWalls();
        List<List<float>> lstWallZ = new List<List<float>>();
        Vector2 maxPoints = vWallAmount + new Vector2(1, 1);
        
        for (int x1 = 0; x1 < maxPoints.x; x1++) {
            lstWallZ.Add(new List<float>());
            for (int y2 = 0; y2 < maxPoints.y; y2++) {
                float randZ = Random.Range(0f, fRandomExtrudeAmt);
                lstWallZ[x1].Add(randZ);
            }
        }

        for (int y = 0; y < vWallAmount.y; y++) {
            for (int x = 0; x < vWallAmount.x; x++) {
                List<float> wallZPos = new List<float>();
                /*wallZPos.Add(-1);
                wallZPos.Add(0);
                wallZPos.Add(1);
                wallZPos.Add(2);
                */
                wallZPos.Add(lstWallZ[x][y]);
                wallZPos.Add(lstWallZ[x+1][y+1]);
                wallZPos.Add(lstWallZ[x+1][y]);
                wallZPos.Add(lstWallZ[x][y+1]);

                GameObject tmpWall = Instantiate(gWallPre);
                tmpWall.transform.parent = this.transform;
                tmpWall.transform.localScale = vWallScale;
                Vector3 pos = new Vector3((x * vWallSize.x) * vWallScale.x, (y * vWallSize.y) * vWallScale.y, 0f);
                pos += new Vector3(vWallSize.x/2f, vWallSize.y/2f, 0f);
                tmpWall.transform.localPosition = pos;
                tmpWall.GetComponent<WallObj>().SetVerticesZ(wallZPos);
                lstWalls.Add(tmpWall);
            }
        }
    }

    private void ClearWalls() {
        for(int i = 0; i < lstWalls.Count; i++) {
            DestroyImmediate(lstWalls[i].gameObject);
        }
        lstWalls.Clear();
    }
}
