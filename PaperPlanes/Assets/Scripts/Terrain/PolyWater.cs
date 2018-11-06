using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class PolyWater : PolyTerrain {



    protected override void SetChunk(PolyChunk chunk, List<List<Vector3>> chunkList, Vector2 min, Vector2 max, Vector3 pos) {
        chunk.Init(chunkList, min, max, pos, true);
    }
    /*  // Public
      public bool bStart = false;
      public bool bClear = false;
      public bool bStarted = false;
      public float fPointDensity = 10f;
      public int iSplitAxisCount = 10;
      public Vector2 vNumPoints = new Vector2(2f, 2f);

      // Private
      private List<List<Vector3>> lstPoints = new List<List<Vector3>>();
      private List<GameObject> lstMeshObj = new List<GameObject>();
      private MeshFilter meshFilter;
      private MeshRenderer meshRenderer;
      private Material matWater;

      // Use this for initialization
      void Start() {
          meshFilter = this.GetComponent<MeshFilter>();
          meshRenderer = this.GetComponent<MeshRenderer>();
          matWater = meshRenderer.sharedMaterial;
          InitStart();
      }

      void InitStart () {
          meshFilter = this.GetComponent<MeshFilter>();
          meshRenderer = this.GetComponent<MeshRenderer>();
          matWater = meshRenderer.sharedMaterial;
          GeneratePoints();
          GenerateSplitMesh();
          bStarted = true;
      }

      private void GeneratePoints() {
          lstPoints.Clear();
          if ((vNumPoints.x > 0) && (vNumPoints.y > 0) && (fPointDensity > 0f)) {
              for (int x = 0; x < vNumPoints.x; x++) {
                  lstPoints.Add(new List<Vector3>());
                  for (int y = 0; y < vNumPoints.y; y++) {
                      Vector3 pos = new Vector3((x - (vNumPoints.x / 2)) * fPointDensity, 0f, (y - (vNumPoints.y / 2)) * fPointDensity);
                      lstPoints[x].Add(pos);
                  }
              }
          }
      }

      void Update() {
          if (bStart) {
              InitStart();
              bStart = false;
          }
      }

      public void GenerateSplitMesh() {
          while (lstMeshObj.Count > 0) {
              GameObject obj = lstMeshObj[0];
              lstMeshObj.RemoveAt(0);
              DestroyImmediate(obj);
          }
        //  lstMeshFilt.Clear();
         // lstMeshRend.Clear();

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
                  //Debug.Log("Range - " + x + " | " + y + " " + range + " | " + xMult + ", " + yMult);
              }
              //prevx = xMult * x;
              //prevy = 0f;
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
                  numPointsX = 1 + maxX - minX;
                  numPointsY = 1 + maxY - minY;

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
                      //    vertices[(y * numPointsX) + x] = lstPoints[x + (minX)/*((int)lstSquares[sqX][sqY].x)*///][y + (minY) /*((int)lstSquares[sqX][sqY].y)*/];
                                                                                                                  //tmpChunkList[x].Add(lstPoints[x + (minX)][y + (minY)]);


    //Debug.Log(x + ", " + y + " | " + vertices[(y * numPointsX) + x]);

    /* if (x % 2 == 0) {
         uvs[(y * numPointsX) + x].x = 1;// x / (numPointsX - 0);
     } else {
         uvs[(y * numPointsX) + x].x = 0;// x / (numPointsX - 0);
     }
     if (y % 2 == 0) {
         uvs[(y * numPointsX) + x].y = 1;// y / (numPointsY - 0);
     } else {
         uvs[(y * numPointsX) + x].y = 0;// y / (numPointsY - 0);
     }*/
    /* uvs[(y * numPointsX) + x].x = x / (numPointsX - 10);
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

//tmpMesh = GenerateFlatTerrain(tmpMesh);
//tmpFilt.sharedMesh = tmpMesh;

tmpFilt.sharedMesh = tmpMesh;
tmpCol.sharedMesh = tmpMesh;
// chunk.Init(tmpChunkList, new Vector2(minX, minY), new Vector2(maxX, maxY));


//  lstMeshFilt.Add(tmpFilt);
// lstMeshRend.Add(tmpRend);
lstMeshObj.Add(gMeshObj);
}

}
}*/
}