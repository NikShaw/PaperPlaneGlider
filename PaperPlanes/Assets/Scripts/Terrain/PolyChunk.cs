using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class PolyChunk : MonoBehaviour {

    // Public
    private List<float> lstStageDistLow =           new List<float>() { 170f, 300f, 500f, 800f, 10000f };
    private List<float> lstStageDistMed =           new List<float>() { 300f, 450f, 800f, 1000f, 10000f };
    private List<float> lstStageDistHigh =          new List<float>() { 600f, 700f, 1200f, 1400f, 10000f };
    private List<float> lstStageDistVeryHigh =      new List<float>() { 1000f, 1100f, 1800f, 2000f, 10000f };
    private List<float> lstStageWaterDistLow =      new List<float>() { 250f, 400f, 520f, 700f, 10000f };
    private List<float> lstStageWaterDistMed =      new List<float>() { 400f, 600f, 800f, 1000f, 10000f };
    private List<float> lstStageWaterDistHigh =     new List<float>() { 800f, 1000f, 1300f, 1500f, 10000f };
    private List<float> lstStageWaterDistVeryHigh = new List<float>() { 1000f, 1100f, 1800f, 2000f, 10000f };
    private int iMeshStages = 5;
    public int iSetStage = 0;
    public float fCamDist = 0f;
    public float fCamForwDist = 150f;
    public Vector2 vStart = Vector2.zero;
    public Vector2 vEnd = Vector2.zero;

    // Private
    private bool bVisible = true;
    private bool bWater = false;
    public int iCurrStage = 0;
    private int iQuality = 0;
    private int iLoadedQuality = -1;
    private float fRefreshTimer = 0f;
    private float fRefreshTime = 2f;
    private float fRefreshDistMax = 1500f;
    private Vector2 vRefreshTime = new Vector2(1f, 16f);
    public Vector3 vMid = Vector3.zero;
    public List<List<Vector3>> lstPoints = new List<List<Vector3>>();
    public List<Mesh> lstMeshStages = new List<Mesh>();
    private List<float> lstStageDist = new List<float>();
    private bool bLoaded = false;
    MeshFilter mFilter;
    private readonly int x;

    void Start() {
        CheckQuality();
        fCamForwDist = lstStageDist[0] * 0.75f;
       // fRefreshDistMax = 2000f;// lstStageDist[3];
        fRefreshTimer = Random.Range(0f, fRefreshTime);
        GameInit();
    }

    private void CheckQuality() {
        int tmpQuality = iQuality;
        if (Settings.settings != null) {
            tmpQuality = Settings.settings.GetTerrainDist();
        }
        if(tmpQuality == iLoadedQuality) {
            return;
        }
        if (tmpQuality == 0) {
            if (bWater) {
                lstStageDist = lstStageWaterDistLow;
            } else {
                lstStageDist = lstStageDistLow;
            }
            iLoadedQuality = 0;
        } else if (tmpQuality == 1) {
            if (bWater) {
                lstStageDist = lstStageWaterDistMed;
            } else {
                lstStageDist = lstStageDistMed;
            }
            iLoadedQuality = 1;
        } else if (tmpQuality == 2) {
            if (bWater) {
                lstStageDist = lstStageWaterDistHigh;
            } else {
                lstStageDist = lstStageDistHigh;
            }
            iLoadedQuality = 2;
        } else if (tmpQuality == 3) {
            if (bWater) {
                lstStageDist = lstStageWaterDistVeryHigh;
            } else {
                lstStageDist = lstStageDistVeryHigh;
            }
            iLoadedQuality = 3;
        }
        fCamForwDist = lstStageDist[0] * 0.75f;
    }

    private void GameInit() {
        lstPoints = this.transform.parent.GetComponent<PolyTerrain>().GetPointsAt(vStart, vEnd);
        mFilter = this.GetComponent<MeshFilter>();
        lstMeshStages.Add(mFilter.sharedMesh);
        for (int i = 1; i < iMeshStages; i++) {
            lstMeshStages.Add(SimplifyMesh((i+1)*2));
        }
        bLoaded = true;
    }

    // Use this for initialization
    public void Init(List<List<Vector3>> points, Vector2 start, Vector2 end, Vector3 pos, bool water) {
        bWater = water;
        vStart = start;
        vEnd = end;
        lstPoints = points;
        mFilter = this.GetComponent<MeshFilter>();
        vMid = pos + mFilter.sharedMesh.vertices[mFilter.sharedMesh.vertices.Length / 2];
        lstMeshStages.Add(mFilter.sharedMesh);
        for (int i = 1; i < iMeshStages; i++) {
            lstMeshStages.Add(SimplifyMesh((i+1)*2));
        }
        if (bWater) {
            this.GetComponent<MeshCollider>().enabled = false;
        }
    }

    private Mesh SimplifyMesh(int amt) {
        Mesh tmpMesh = new Mesh();
        tmpMesh.name = this.transform.name + " MESH STAGE " + amt;

        int numPointsX = lstPoints.Count / amt;
        int numPointsY = lstPoints[0].Count / amt;

        Vector3[] vertices = new Vector3[((numPointsX * numPointsY)) + 0];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(((numPointsX - 1) * (numPointsY - 1)) * 2 * 3) + 0];
        int currTri = 0;
        int currVert = 0;
        // int currX = 0;
        //  int currY = 0;

        // Create mesh
        for (int currX = 0; currX < numPointsX; currX++) {                                            // Generate mesh
            for (int currY = 0; currY < numPointsY; currY++) {
                if ((currX >= numPointsX) || (currY >= numPointsY)) {
                    break;
                }
                int tmpPosX = currX * amt;
                int tmpPosY = currY * amt;
                if (currX == numPointsX - 1) {
                    tmpPosX = lstPoints.Count - 1;
                }
                if(currY == numPointsY - 1) {
                    tmpPosY = lstPoints[currX].Count - 1;
                }

                vertices[(currY * numPointsX) + currX] = lstPoints[tmpPosX][tmpPosY];
                uvs[(currY * numPointsX) + currX].x = currX / (numPointsX - 0);
                uvs[(currY * numPointsX) + currX].y = currY / (numPointsY - 0);

                if ((currX < (numPointsX - 1)) && (currY < (numPointsY - 1))) {
                    triangles[currTri + 0] = numPointsX + currVert;
                    triangles[currTri + 1] = currVert + 1;
                    triangles[currTri + 2] = currVert;
                    triangles[currTri + 3] = numPointsX + currVert;
                    triangles[currTri + 4] = numPointsX + currVert + 1;
                    triangles[currTri + 5] = currVert + 1;
                    currTri += 6;
                }
                // currY++;
                currVert++;
                // }
            }
            // currX++;
        }

        tmpMesh.vertices = vertices;
        tmpMesh.triangles = triangles;
        tmpMesh.uv = uvs;
        tmpMesh.RecalculateBounds();
        tmpMesh.RecalculateNormals();

        return tmpMesh;
    }

    // Update is called once per frame
    void Update() {
        if(bLoaded){
           // CheckVisible();
            if(fRefreshTimer >= fRefreshTime) {
                CheckQuality();
                if (bVisible){
                    CheckCamDist();
                }
                CheckStage();
                fRefreshTimer -= fRefreshTime;
                if (fRefreshTimer < 0f) fRefreshTimer = 0f;
            }
            fRefreshTimer += Time.deltaTime;
        }
    }

    private void CheckStage() {
        if (iCurrStage != iSetStage) {
            if(iSetStage < 0) {
                mFilter.sharedMesh = null;
                iCurrStage = iSetStage;
            } else if (iSetStage < iMeshStages) {
                if (lstMeshStages.Count > iSetStage) {
                    mFilter.sharedMesh = lstMeshStages[iSetStage];
                }
                iCurrStage = iSetStage;
            } else {
                iSetStage = iCurrStage;
            }
        }
    }

    private void CheckCamDist() {
        float dist = Vector3.Distance(vMid, Camera.main.transform.position);
        fCamDist = Vector3.Distance(vMid, (Camera.main.transform.position) + (Camera.main.transform.forward * fCamForwDist));
        for (int i = 0; i < iMeshStages; i++) {
            if (fCamDist < lstStageDist[i]) {
                iSetStage = i;
                break;
            } else if (i == (iMeshStages - 1)) {
                iSetStage = i;
            }
        }
        fRefreshTime = Mathf.Lerp(vRefreshTime.x, vRefreshTime.y, dist / fRefreshDistMax);
    }

    private void CheckVisible() {
        bool visible = false;
        //Vector3[] vPos = new Vector3[8];
        /*vPos[0] = lstPoints[0][0];
        vPos[1] = lstPoints[0][lstPoints[0].Count - 1];
        vPos[2] = lstPoints[0][(lstPoints[0].Count - 1)/2];
        vPos[3] = lstPoints[(lstPoints.Count-1)/2][0];
        vPos[4] = lstPoints[lstPoints.Count-1][0];
        vPos[5] = lstPoints[lstPoints.Count - 1][(lstPoints[lstPoints.Count - 1].Count - 1)/2];
        vPos[6] = lstPoints[lstPoints.Count - 1][lstPoints[lstPoints.Count - 1].Count - 1];
        vPos[7] = lstPoints[(lstPoints.Count - 1)/2][lstPoints[lstPoints.Count - 1].Count - 1];*/

       // for (int i = 0; i < vPos.Length; i++) {
            if (CheckViewpoint(vMid)) {
                visible = true;
            }
      //  }

        if(visible != bVisible){
            bVisible = visible;
            if (!bVisible) {
                iSetStage = -1;
            } else {
                CheckCamDist();
            }
            CheckStage();
        }
    }

    private bool CheckViewpoint(Vector3 pos) {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(pos += (Camera.main.transform.forward * 300f));

        if((((viewPos.x < -4) || (viewPos.x > 5)) || ((viewPos.y < -4) || (viewPos.y > 5))) || (viewPos.z < -5)) {
            return false;
        }

        return true;
    }
}
