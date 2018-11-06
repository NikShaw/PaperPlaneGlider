using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class PolyTerrain : MonoBehaviour {

    // public
    public Vector2 vNumPoints;
    public float fPointDensity;
    public string sTerrainName;
    private string sSavePath = "/Resources/";
    public bool bGeneratePoints;
    public bool bGenerateTerrain;
    public bool bEditor = true;
    public bool bClear = false;
    public bool bSave = false;
    public bool bLoad = false;
    public bool bClearMesh = false;
    public bool bInitialised = false;
    public bool bSplit = false;
    public bool bStart = false;
    public int iSplitAxisCount = 10;
    public Vector2 vEdgeOffset = new Vector2(25f, -20f);
    public GameObject prePoint;

    // private
    protected List<GameObject> lstMeshObj = new List<GameObject>();
    protected List<MeshRenderer> lstMeshRend = new List<MeshRenderer>();
    protected List<MeshFilter> lstMeshFilt = new List<MeshFilter>();
    protected List<List<GameObject>> lstPoints = new List<List<GameObject>>();
    protected List<List<Vector3>> lstVPoints = new List<List<Vector3>>();
    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;
    protected MeshRenderer meshRenderer;

    void Start() {
        if (bEditor) {
            InitStart();
        } else {
            //GeneratePoints();
           // Load();
        }
    }

    // Use this for initialization
    void InitStart () {
        Clear();
        meshFilter = this.GetComponent<MeshFilter>();
        meshCollider = this.GetComponent<MeshCollider>();
        meshRenderer = this.GetComponent<MeshRenderer>();
        sSavePath = Application.dataPath + sSavePath;
        bInitialised = true;
        GeneratePoints();
        if(sTerrainName != ""){
            Load(true);
        }
        if (bSplit) {
            GenerateSplitTerrain();
        }   else {
            GenerateTerrain();
        }
    }

    // Update is called once per frame
    void Update() {
        if (bStart) {
            InitStart();
            bStart = false;
        }
        if (bClear) {
            Clear();
            bClear = false;
        }
        if (bSave) {
            Save();
            bSave = false;
        }
        if (bLoad) {
            Load(true);
            bLoad = false;
        }
        if (bClearMesh) {
            ClearMesh();
            bClearMesh = false;
        }
        if (bGeneratePoints) {
            try {
                GeneratePoints();
            } catch {
                Debug.Log("Failed to points");
            }
            bGeneratePoints = false;
        }
        if (bGenerateTerrain) {
            try {
                GenerateFlatTerrain();
            } catch {
                Debug.Log("Failed to terrain");
            }
            bGenerateTerrain = false;
        }
    }

    public List<List<Vector3>> GetPointsAt(Vector2 start, Vector2 end) {
        
        if(lstVPoints.Count == 0) {
            GeneratePoints();
            Load(false);
        }

        List<List<Vector3>> lstVec3 = new List<List<Vector3>>();
        int xAdd = 0;

        for(int x = (int)start.x; x < (int)end.x; x++) {
            lstVec3.Add(new List<Vector3>());
            for(int y = (int)start.y; y < (int)end.y; y++) {
               // Debug.Log(x + "/" + lstVPoints.Count + " | " + y);// + "/" + lstVPoints[x].Count);
                lstVec3[xAdd].Add(lstVPoints[x][y]);
            }
            xAdd++;
        }

        return lstVec3;
    }

    public void GenerateSplitTerrain() {
        while(lstMeshObj.Count > 0) {
            GameObject obj = lstMeshObj[0];
            lstMeshObj.RemoveAt(0);
            DestroyImmediate(obj);
        }
        lstMeshFilt.Clear();
        lstMeshRend.Clear();

        // Set up ranges for each square
        List<List<Vector4>> lstSquares = new List<List<Vector4>>();
        // float prevx = 0f;
        // float prevy = 0f;
        int tmpNumPointsX = (int)vNumPoints.x;
        int tmpNumPointsY = (int)vNumPoints.y;
        int xMult = tmpNumPointsX / iSplitAxisCount;
        int yMult = tmpNumPointsY / iSplitAxisCount;
        for (int x = 0; x < iSplitAxisCount; x++) {
            lstSquares.Add(new List<Vector4>());
            //Debug.Log("Range - " + lstSquares[x][0]);
            for (int y = 0; y < iSplitAxisCount; y++) {
                Vector2 xRange = new Vector2(0f, 0f);
                Vector2 yRange = new Vector2(0f, 0f);
                xRange.x = xMult * x;
                if (x >= (iSplitAxisCount - 1)) {
                    //  Debug.Log(x + ", " + y + " X");
                    xRange.y = tmpNumPointsX - 1;
                    xRange.x -= 1;
                } else {
                    xRange.y = xMult * (x + 1);
                //prevx = xRange.y;
                }

                yRange.x = yMult * y;
                if (y >= (iSplitAxisCount - 1)) {
                    // Debug.Log(x + ", " + y + " Y");
                    yRange.y = tmpNumPointsY - 1;
                    yRange.x -= 1;
                } else {
                    yRange.y = yMult * (y + 1);
                    //prevy = yRange.y;
                }

                lstSquares[x].Add(new Vector4((int)xRange.x, (int)xRange.y, (int)yRange.x, (int)yRange.y));
            }
        }
        // Create meshes for each square
        for (int sqX = 0; sqX < iSplitAxisCount; sqX++) {
            for (int sqY = 0; sqY < iSplitAxisCount; sqY++) {

                int maxX = (int)lstSquares[sqX][sqY].y;
                int minX = (int)lstSquares[sqX][sqY].x;
                int maxY = (int)lstSquares[sqX][sqY].w;
                int minY = (int)lstSquares[sqX][sqY].z;

                int numPointsX = 1 + maxX - minX;
                int numPointsY = 1 + maxY - minY;

                Vector3[] vertices = new Vector3[((numPointsX * numPointsY)) + 0];
                Vector2[] uvs = new Vector2[vertices.Length];
                int[] triangles = new int[(((numPointsX - 1) * (numPointsY - 1)) * 2 * 3) + 0];
                List<List<Vector3>> tmpChunkList = new List<List<Vector3>>();
                int currTri = 0;
                int currVert = 0;

                GameObject gMeshObj = new GameObject();
                PolyChunk chunk = gMeshObj.AddComponent<PolyChunk>();
                gMeshObj.transform.parent = this.transform;
                gMeshObj.transform.localPosition = Vector3.zero;
                gMeshObj.transform.localScale = Vector3.one;
                gMeshObj.transform.rotation = this.transform.rotation;
                gMeshObj.transform.name = "Poly Terrain " + lstMeshObj.Count.ToString();
                gMeshObj.layer = this.gameObject.layer;
                MeshFilter tmpFilt = gMeshObj.AddComponent<MeshFilter>();
                MeshRenderer tmpRend = gMeshObj.AddComponent<MeshRenderer>();
                MeshCollider tmpCol = gMeshObj.AddComponent<MeshCollider>();
                tmpRend.sharedMaterial = meshRenderer.sharedMaterial;

                Mesh tmpMesh = new Mesh();
                tmpMesh.name = gMeshObj.transform.name + " MESH";

                // Create mesh
                for (int x = 0; x < numPointsX; x++) {                                            // Generate mesh
                    tmpChunkList.Add(new List<Vector3>());
                    for (int y = 0; y < numPointsY; y++) {
                        if (bEditor) {
                            vertices[(y * numPointsX) + x] = lstPoints[x + minX][y + minY].transform.localPosition;
                        } else {
                            vertices[(y * numPointsX) + x] = lstVPoints[x + (minX)][y + (minY)];
                            tmpChunkList[x].Add(lstVPoints[x + (minX)][y + (minY)]);
                        }

                        uvs[(y * numPointsX) + x].x = x / (numPointsX - 10);
                        uvs[(y * numPointsX) + x].y = y / (numPointsY - 10);

                        if ((x < (numPointsX - 1)) && (y < (numPointsY - 1))) {
                            triangles[currTri + 0] = numPointsX + currVert;
                            triangles[currTri + 1] = currVert + 1;
                            triangles[currTri + 2] = currVert;
                            triangles[currTri + 3] = numPointsX + currVert;
                            triangles[currTri + 4] = numPointsX + currVert + 1;
                            triangles[currTri + 5] = currVert + 1;
                            currTri += 6;
                        }
                        currVert++;
                    }
                }


                tmpMesh.vertices = vertices;
                tmpMesh.triangles = triangles;
                tmpMesh.uv = uvs;
                tmpMesh.RecalculateBounds();
                tmpMesh.RecalculateNormals();

                tmpFilt.sharedMesh = tmpMesh;
                tmpCol.sharedMesh = tmpMesh;
               // chunk.Init(tmpChunkList, new Vector2(minX, minY), new Vector2(maxX, maxY), this.transform.position, false);
                SetChunk(chunk, tmpChunkList, new Vector2(minX, minY), new Vector2(maxX, maxY), this.transform.position);
                
                lstMeshObj.Add(gMeshObj);
            }
        
        }
    }

    protected virtual void SetChunk(PolyChunk chunk, List<List<Vector3>> chunkList, Vector2 min, Vector2 max, Vector3 pos) {
        chunk.Init(chunkList, min, max, pos, false);
    }

    public Mesh GenerateFlatTerrain(Mesh mesh) {
        Mesh tmpMesh = new Mesh();
        int countX = 0;
        int countY = 0;
        if (bEditor) {
            countY = lstPoints.Count;
            countX = lstPoints[0].Count;
        } else {
            countY = lstVPoints.Count;
            countX = lstVPoints[0].Count;
        }

        if ((countX > 0) && (countY > 0)) {
            GenerateTerrain();

            tmpMesh = mesh;


            Vector3[] oldVerts = tmpMesh.vertices;
            Vector2[] oldUvs = tmpMesh.uv;
            int[] triangles = tmpMesh.triangles;
            Vector3[] vertices = new Vector3[triangles.Length];
            Vector2[] uvs = new Vector2[vertices.Length];

            Debug.Log(triangles.Length);

            for (int i = 0; i < triangles.Length; i++) {
                vertices[i] = oldVerts[triangles[i]];
                uvs[i] = oldUvs[triangles[i]];
                triangles[i] = i;
            }

            tmpMesh.vertices = vertices;
            tmpMesh.triangles = triangles;
            tmpMesh.uv = uvs;
            tmpMesh.RecalculateBounds();
            tmpMesh.RecalculateNormals();
        }

        return tmpMesh;
    }

    public void GenerateFlatTerrain() {
        Debug.Log("generate");

        int countX = 0;
        int countY = 0;
        if (bEditor) {
            countY = lstPoints.Count;
            countX = lstPoints[0].Count;
        } else {
            countY = lstVPoints.Count;
            countX = lstVPoints[0].Count;
        }

        if ((countX > 0) && (countY > 0)) {
            GenerateTerrain();

            Mesh tmpMesh = meshFilter.sharedMesh;


            Vector3[] oldVerts = tmpMesh.vertices;
            Vector2[] oldUvs = tmpMesh.uv;
            int[] triangles = tmpMesh.triangles;
            Vector3[] vertices = new Vector3[triangles.Length];
            Vector2[] uvs = new Vector2[vertices.Length];

            Debug.Log(triangles.Length);

            for (int i = 0; i < triangles.Length; i++) {
                vertices[i] = oldVerts[triangles[i]];
                uvs[i] = oldUvs[triangles[i]];
                triangles[i] = i;
            }

            tmpMesh.vertices = vertices;
            tmpMesh.triangles = triangles;
            tmpMesh.uv = uvs;
            tmpMesh.RecalculateBounds();
            tmpMesh.RecalculateNormals();

            meshFilter.sharedMesh = tmpMesh;

            meshFilter.mesh = tmpMesh;
            meshCollider.sharedMesh = tmpMesh;

        }
    }

    public void UpdateTerrain() {
        int countX = 0;
        int countY = 0;
        if (bEditor) {                                                                          // Use gameobjects if editor
            countY = lstPoints.Count;
            countX = lstPoints[0].Count;
        } else {
            countY = lstVPoints.Count;
            countX = lstVPoints[0].Count;
        }

        if ((countX > 0) && (countY > 0)) {
            Mesh tmpMesh = meshFilter.sharedMesh;//new Mesh();

            Vector3[] vertices = new Vector3[(((int)vNumPoints.x * (int)vNumPoints.y)) /*+ 12*/];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[((((int)vNumPoints.x - 1) * ((int)vNumPoints.y - 1)) * 2 * 3) /*+ 48*/];
            int currTri = 0;
            int currVert = 0;

            for (int x = 0; x < vNumPoints.x; x++) {                                            // Generate mesh
                for (int y = 0; y < vNumPoints.y; y++) {
                    if (bEditor) {
                        vertices[(y * (int)vNumPoints.x) + x] = lstPoints[x][y].transform.localPosition;
                    } else {
                        vertices[(y * (int)vNumPoints.x) + x] = lstVPoints[x][y];
                    }

                    uvs[(y * (int)vNumPoints.x) + x].x = x / (vNumPoints.x - 1);
                    uvs[(y * (int)vNumPoints.x) + x].y = y / (vNumPoints.y - 1);

                    if ((x < ((int)vNumPoints.x - 1)) && (y < ((int)vNumPoints.y - 1))) {
                        triangles[currTri + 0] = (int)vNumPoints.x + currVert;
                        triangles[currTri + 1] = currVert + 1;
                        triangles[currTri + 2] = currVert;
                        triangles[currTri + 3] = (int)vNumPoints.x + currVert;
                        triangles[currTri + 4] = (int)vNumPoints.x + currVert + 1;
                        triangles[currTri + 5] = currVert + 1;
                        currTri += 6;
                    }
                    currVert++;
                }
            }

            tmpMesh.vertices = vertices;
            tmpMesh.triangles = triangles;
            tmpMesh.uv = uvs;
           // tmpMesh.RecalculateNormals();
           // tmpMesh.RecalculateBounds();
            tmpMesh.name = "PolyTerrain";
           // meshFilter.mesh = tmpMesh;
           // meshCollider.sharedMesh = tmpMesh;
        }
    }

    public void FinishUpdate() {
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
        //= tmpMesh;
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public void GenerateTerrain() {

        int countX = 0;
        int countY = 0;
        if (bEditor) {                                                                          // Use gameobjects if editor
            countY = lstPoints.Count;
            countX = lstPoints[0].Count;
        } else {
            countY = lstVPoints.Count;
            countX = lstVPoints[0].Count;
        }

        if ((countX > 0) && (countY> 0)) {
            Mesh tmpMesh = new Mesh();

            Vector3[] vertices = new Vector3[(((int)vNumPoints.x * (int)vNumPoints.y)) /*+ 12*/];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[((((int)vNumPoints.x - 1) * ((int)vNumPoints.y - 1)) * 2 * 3) /*+ 48*/];
            int currTri = 0;
            int currVert = 0;

            for (int x = 0; x < vNumPoints.x; x++) {                                            // Generate mesh
                for (int y = 0; y < vNumPoints.y; y++) {
                    if (bEditor) {
                        vertices[(y * (int)vNumPoints.x) + x] = lstPoints[x][y].transform.localPosition;
                    } else {
                        vertices[(y * (int)vNumPoints.x) + x] = lstVPoints[x][y];
                    }

                    uvs[(y * (int)vNumPoints.x) + x].x = x / (vNumPoints.x-1);
                    uvs[(y * (int)vNumPoints.x) + x].y = y / (vNumPoints.y-1);

                    if ((x < ((int)vNumPoints.x - 1)) && (y < ((int)vNumPoints.y - 1))) {
                        triangles[currTri + 0] = (int)vNumPoints.x + currVert;
                        triangles[currTri + 1] = currVert + 1;
                        triangles[currTri + 2] = currVert;
                        triangles[currTri + 3] = (int)vNumPoints.x + currVert;
                        triangles[currTri + 4] = (int)vNumPoints.x + currVert + 1;
                        triangles[currTri + 5] = currVert + 1;  
                        currTri += 6;
                    }
                    currVert++;
                }
            }

            tmpMesh.vertices = vertices;
            tmpMesh.triangles = triangles;
            tmpMesh.uv = uvs;
            tmpMesh.RecalculateNormals();
            tmpMesh.RecalculateBounds();
            tmpMesh.name = "PolyTerrain";
            meshFilter.mesh = tmpMesh;
            meshCollider.sharedMesh = tmpMesh;
        }
    }

    public void GeneratePoints() {
        //Clear();
        if ((vNumPoints.x > 0) && (vNumPoints.y > 0) && (fPointDensity > 0f)) {
            for(int x = 0; x < vNumPoints.x; x++) {
                if (bEditor) {
                    lstPoints.Add(new List<GameObject>());
                } else {
                    lstVPoints.Add(new List<Vector3>());
                }
                for (int y = 0; y < vNumPoints.y; y++) {
                    if (bEditor) {
                        GameObject tmpObj = Instantiate(prePoint);
                        Vector3 pos = new Vector3((x - (vNumPoints.x / 2)) * fPointDensity, 0f, (y - (vNumPoints.y / 2)) * fPointDensity);
                        tmpObj.transform.parent = this.transform;
                        tmpObj.transform.name = "Terrain point :" + x + ", " + y;
                        if (x == 0) {
                            //pos.x -= vEdgeOffset.x * fPointDensity;
                            //pos.y += vEdgeOffset.y * fPointDensity;
                            if (tmpObj.GetComponent<SphereCollider>() != null) {
                                tmpObj.GetComponent<SphereCollider>().enabled = false;
                            }
                        } else if (x == (vNumPoints.x - 1)) {
                            //pos.x += vEdgeOffset.x * fPointDensity;
                            //pos.y += vEdgeOffset.y * fPointDensity;
                            if (tmpObj.GetComponent<SphereCollider>() != null) {
                                tmpObj.GetComponent<SphereCollider>().enabled = false;
                            }
                        }/* else if ((x == 1) || (x == (vNumPoints.x - 2))){
                            if (tmpObj.GetComponent<SphereCollider>() != null) {
                                tmpObj.GetComponent<SphereCollider>().enabled = false;
                            }
                        }*/
                        if (y == 0) {
                            //pos.z -= vEdgeOffset.x * fPointDensity;
                            //pos.y += vEdgeOffset.y * fPointDensity;
                            if (tmpObj.GetComponent<SphereCollider>() != null) {
                                tmpObj.GetComponent<SphereCollider>().enabled = false;
                            }
                        } else if (y == (vNumPoints.y - 1)) {
                            //pos.z += vEdgeOffset.x * fPointDensity;
                            //pos.y += vEdgeOffset.y * fPointDensity;
                            if (tmpObj.GetComponent<SphereCollider>() != null) {
                                tmpObj.GetComponent<SphereCollider>().enabled = false;
                            }
                        } /*else if ((y == 1) || (y == (vNumPoints.y - 2))) {
                            if (tmpObj.GetComponent<SphereCollider>() != null) {
                                tmpObj.GetComponent<SphereCollider>().enabled = false;
                            }
                        }*/
                        tmpObj.transform.position = pos;
                        lstPoints[x].Add(tmpObj);
                    } else {
                        Vector3 pos = new Vector3((x - (vNumPoints.x / 2)) * fPointDensity, 0f, (y - (vNumPoints.y / 2)) * fPointDensity);
                       /* if (x == 0) {
                            pos.x -= vEdgeOffset.x * fPointDensity;
                            pos.y += vEdgeOffset.y * fPointDensity;
                        } else if (x == (vNumPoints.x - 1)) {
                            pos.x += vEdgeOffset.x * fPointDensity;
                            pos.y += vEdgeOffset.y * fPointDensity;
                        } 
                        if (y == 0) {
                            pos.z -= vEdgeOffset.y * fPointDensity;
                            pos.y += vEdgeOffset.y * fPointDensity;
                        } else if (y == (vNumPoints.y - 1)) {
                            pos.z += vEdgeOffset.y * fPointDensity;
                            pos.y += vEdgeOffset.y * fPointDensity;
                        } */
                        lstVPoints[x].Add(pos);
                    }
                }
            }
        }
    }

    public void SetNumPoints(Vector2 numPoints) {
        vNumPoints = numPoints;
    }

    public void SetDensity(float d) {
        fPointDensity = d;
    }

    public void SetFile(string file) {
        sTerrainName = file;
    }

    public void Save() {
        int countX = 0;

        if (bEditor) {
            countX = lstPoints.Count;
        } else {
            countX = lstVPoints.Count;
        }

        if (countX > 0) {
            string output = "";
            output += vNumPoints.x + "," + vNumPoints.y + "|" + fPointDensity + "|";

            for (int x = 0; x < countX; x++) {

                int countY = 0;
                if (bEditor) {
                    countY = lstPoints[x].Count;
                } else {
                    countY = lstVPoints[x].Count;
                }

                for (int y = 0; y < countY; y++) {
                    Vector3 pos = Vector3.zero;
                    if (bEditor) {
                        pos = lstPoints[x][y].transform.position;
                    } else {
                        pos = lstVPoints[x][y];
                    }
                    output += pos.x + "," + pos.y + "," + pos.z;
                    //if (x != (lstPoints.Count - 1)) {
                    //    output += "*";
                    // } else 
                    if (y != (countY - 1)) {
                        output += "*";
                    }
                }
                if (x != (countX - 1)) {
                    output += "^";
                }
            }

            StreamWriter sr = File.CreateText(sSavePath+sTerrainName+".txt");
            sr.WriteLine(output);
            sr.Close();
        }
    }

    public void Load(bool clear) {
        if(sTerrainName == "") {
            return;
        }
        TextAsset txtAsset = Resources.Load(sTerrainName) as TextAsset;
        //string[] txtLines = txtAsset.text.Split("/n"[0]);
        //int iCurrLine = 0;
        // if (File.Exists(sSavePath+sTerrainName)) {
        //  StreamReader sr = File.OpenText(sSavePath+sTerrainName);
        if(txtAsset == null) {
            Debug.Log("Failed to load " + sTerrainName);
            return;
        }
        string line = txtAsset.text; //sr.ReadLine();
            //sr.Close();

            float fnumx = 1f;
            float fnumy = 1f;
            float fdensity = 1f;

            string[] strSect = line.Split('|');
            string numx = strSect[0].Split(',')[0].ToString();
            string numy = strSect[0].Split(',')[1].ToString();
            string dense = strSect[1].ToString();
            string[] lines = strSect[2].Split('^');

            if (float.TryParse(numx, out fnumx) && float.TryParse(numy, out fnumy) && float.TryParse(dense, out fdensity)) {
                Debug.Log("Loading: " + fnumx + ", " + fnumy + " | " + fdensity);
            if(clear){
                Clear();
            }
            } else {
                Debug.Log("Parsing lines failed");
                return;
            }

            vNumPoints = new Vector2(fnumx, fnumy);
            fPointDensity = fdensity;

            for(int y = 0; y < lines.Length; y++) {
                string[] vals = lines[y].Split('*');
                if(y == 0) {
                    for(int val = 0; val < vals.Length; val++) {
                        if (bEditor) {
                            lstPoints.Add(new List<GameObject>());
                        } else {
                            lstVPoints.Add(new List<Vector3>());
                        }
                    }
                }
                for(int x = 0; x < vals.Length; x++) {
                    string[] indv = vals[x].Split(',');
                    string xstr = indv[0];
                    string ystr = indv[1];
                    string zstr = indv[2];
                    float xVal, yVal, zVal = 0f;

                    if (float.TryParse(xstr, out xVal) && float.TryParse(ystr, out yVal) && float.TryParse(zstr, out zVal)) {
                        if (bEditor) {
                            GameObject tmpObj = Instantiate(prePoint);
                            tmpObj.transform.parent = this.transform;
                            tmpObj.transform.localPosition = new Vector3(xVal, yVal, zVal);
                            tmpObj.transform.name = "Terrain point :" + x + ", " + y;
                            lstPoints[(vals.Length - 1) - x].Add(tmpObj);
                        } else {
                            Vector3 pos = new Vector3(xVal, yVal, zVal);
                            lstVPoints[((vals.Length - 1) - x)].Add(pos);
                        }
                    } else {
                        Debug.Log("Parsing point failed " + xstr + " | " + ystr + " | " + zstr);
                        return;
                    }
                }
            }

        //}
    }

    public void Reset() {
        Clear();
        ClearMesh();
        GeneratePoints();
        GenerateTerrain();
    }

    private void ClearMesh() {
        if(meshFilter != null){
            meshFilter.mesh = null;
        }
    }

    private void Clear() {
        while(this.transform.childCount > 0) {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }
        lstPoints.Clear();
        lstVPoints.Clear();
    }
}
