using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorControls : MonoBehaviour {
    // global
    public static EditorControls controls;
    public enum ePaintModes {Solid, Falloff, Smooth, Flatten, Random};

    // public
    public string sTerrainName = "";
    public float fBrushSize = 4.0f;
    public float fBrushStrength = 1.0f;
    public float fBrushLinearity = 1.0f;
    public float fCameraMoveSpeed = 2.0f;
    public float fDensity = 15.0f;
    public bool bShowUI = true;
    public Vector2 vNumPoints = new Vector2(40, 40);
    public LayerMask layerMask;
    public GameObject gTerrain;
    public GameObject preCursor;

    // private
    private bool bPainting = false;
    public bool bSmoothShading = true;
    public bool bTowardsCam = false;
    public bool bLower = false;
    private bool bAimMode = false;
    public ePaintModes ePaintMode = ePaintModes.Solid;
    private Vector3 vMovement;
    private Vector3 vAim;
    private Vector3 vLastMousePos;
    private PolyTerrain pTerrain;
    public GameObject gSphere;
    private Cursor cursor;



    void Awake() {
        if(controls == null) {
            controls = this;
        } else {
            DestroyImmediate(this.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        pTerrain = gTerrain.GetComponent<PolyTerrain>();
        //if (gSphere == null) {
        gSphere = Instantiate(preCursor);
        gSphere.transform.name = "Cursor";
        gSphere.GetComponent<SphereCollider>().isTrigger = true;
        //gSphere.GetComponent<SphereCollider>().radius = fBrushSize;
        gSphere.transform.localScale = new Vector3(fBrushSize, fBrushSize, fBrushSize);
        cursor = gSphere.AddComponent<Cursor>();
        pTerrain.SetNumPoints(vNumPoints);
        pTerrain.SetDensity(fDensity);
        //}
        //Rigidbody tmpRgd = gSphere.AddComponent<Rigidbody>();
        //tmpRgd.isKinematic = true;
    }

    void OnGUI() {
        if(bShowUI){
            sTerrainName = GUI.TextField(new Rect(10, 10, 200, 20), sTerrainName, 25);
            pTerrain.SetFile(sTerrainName);

            if (GUI.Button(new Rect(10, 40, 50, 20), "Save")) {
                pTerrain.Save();
            }

            float sizeNum = Mathf.Round(fBrushSize*10f)/10f;
            GUI.Box(new Rect(70, 30, 150, 20), "Brush Size: " + sizeNum.ToString());

            fBrushSize = GUI.HorizontalSlider(new Rect(70, 50, 150, 20), fBrushSize, 1f, 240.0f);
            gSphere.transform.localScale = new Vector3(fBrushSize, fBrushSize, fBrushSize);

            float strengthNum = Mathf.Round(fBrushStrength*10f)/10f;
            GUI.Box(new Rect(70, 60, 150, 20), "Brush Strength: " + strengthNum.ToString());
            fBrushStrength = GUI.HorizontalSlider(new Rect(70, 80, 150, 20), fBrushStrength, 0.1f, 1200.0f);

            float linearityNum = Mathf.Round(fBrushLinearity*10f)/10f;
            GUI.Box(new Rect(70, 90, 150, 20), "Brush Non-Linearity: " + linearityNum.ToString());
            fBrushLinearity = GUI.HorizontalSlider(new Rect(70, 110, 150, 20), fBrushLinearity, 0.0f, 2.0f);

            float densityNum = Mathf.Round(fDensity * 10f) / 10f;
            GUI.Box(new Rect(600, 10, 150, 20), "Point Density: " + densityNum.ToString());
            fDensity = GUI.HorizontalSlider(new Rect(600, 30, 150, 20), fDensity, 0.1f, 20.0f);

            float pointsxNum = Mathf.Round(vNumPoints.x * 10f) / 10f;
            float pointsyNum = Mathf.Round(vNumPoints.y * 10f) / 10f;
            GUI.Box(new Rect(600, 60, 150, 20), "Num Points: ");
            float.TryParse(GUI.TextField(new Rect(600, 90, 70, 20), vNumPoints.x.ToString()), out vNumPoints.x);
            float.TryParse(GUI.TextField(new Rect(675, 90, 70, 20), vNumPoints.y.ToString()), out vNumPoints.y);

            if (GUI.Button(new Rect(10, 70, 50, 20), "Load")) {
                pTerrain.Load(true);
            }

            if (GUI.Button(new Rect(10, 100, 50, 20), "Reset")) {
                pTerrain.SetNumPoints(vNumPoints);
                pTerrain.SetDensity(fDensity);
                pTerrain.Reset();
            }

            if (!bTowardsCam) {
                if (GUI.Button(new Rect(10, 130, 120, 20), "Raising Up")) {
                    bTowardsCam = true;
                }
            } else {
                if (GUI.Button(new Rect(10, 130, 120, 20), "Raising To Cam")) {
                    bTowardsCam = false;
                }
            }

           // if (!bSmoothShading) {
               // if (GUI.Button(new Rect(10, 160, 120, 20), "Flat Shading")) {
              //      pTerrain.GenerateTerrain();
              //      bSmoothShading = true;
               // }
          /*  } else {
                if (GUI.Button(new Rect(10, 160, 120, 20), "Smooth Shading")) {
                    pTerrain.GenerateFlatTerrain();
                    bSmoothShading = false;
                }
            }*/

            switch (ePaintMode) {
                case ePaintModes.Falloff:
                    if (GUI.Button(new Rect(10, 190, 120, 20), "Falloff")) {
                        ePaintMode = ePaintModes.Flatten;
                    }
                    break;
                case ePaintModes.Flatten:
                    if (GUI.Button(new Rect(10, 190, 120, 20), "Flatten")) {
                        ePaintMode = ePaintModes.Smooth;
                    }
                    break;
                case ePaintModes.Smooth:
                    if (GUI.Button(new Rect(10, 190, 120, 20), "Smooth")) {
                        ePaintMode = ePaintModes.Solid;
                    }
                    break;
                case ePaintModes.Solid:
                    if (GUI.Button(new Rect(10, 190, 120, 20), "Solid")) {
                        ePaintMode = ePaintModes.Random;
                    }
                    break;
                case ePaintModes.Random:
                    if (GUI.Button(new Rect(10, 190, 120, 20), "Random")) {
                        ePaintMode = ePaintModes.Falloff;
                    }
                    break;
            }
        }
    }

    void Update() {
        bool mouseDown = false;
        if (GUIUtility.hotControl == 0) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000f, layerMask)) {
                gSphere.transform.position = hit.point;
                if (Input.GetMouseButton(0)) {
                    CalcPaint();
                    //pTerrain.GenerateTerrain();
                    pTerrain.UpdateTerrain();
                    bPainting = true;
                    mouseDown = true;
                }
            }
        }
        if(!mouseDown && bPainting) {
            bPainting = false;
            pTerrain.FinishUpdate();
        }
        
        if (Input.GetMouseButtonUp(0)) {
            if (!bSmoothShading) {
            //    pTerrain.GenerateFlatTerrain();
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            bAimMode = true;
        } else if (Input.GetMouseButtonUp(1)) {
            bAimMode = false;
        }

        vLastMousePos = Input.mousePosition;

        if (Input.GetAxis("Lower") > 0.01f) {
            bLower = true;
        } else {
            bLower = false;
        }
    }

    private void CalcPaint() {

        switch (ePaintMode) {
            case ePaintModes.Solid:
                if (bTowardsCam) {
                    Vector3 movement = (Camera.main.transform.position - cursor.transform.position).normalized * fBrushStrength * Time.deltaTime;
                    if (bLower) {
                        movement *= -1f;
                    }
                    cursor.MoveObjects(movement);
                } else {
                    Vector3 movement = new Vector3(0f, 1f, 0f) * fBrushStrength * Time.deltaTime;
                    if (bLower) {
                        movement *= -1f;
                    }
                    cursor.MoveObjects(movement);
                }
                break;
            case ePaintModes.Falloff:
                if (bTowardsCam) {
                    Vector3 movement = (Camera.main.transform.position - cursor.transform.position).normalized * fBrushStrength * Time.deltaTime;
                    if (bLower) {
                        movement *= -1f;
                    }
                    cursor.MoveObjects(movement, cursor.transform.position, fBrushLinearity, fBrushSize/2f);
                } else {
                    Vector3 movement = new Vector3(0f, 1f, 0f) * fBrushStrength * Time.deltaTime;
                    if (bLower) {
                        movement *= -1f;
                    }
                    cursor.MoveObjects(movement, cursor.transform.position, fBrushLinearity, fBrushSize/2f);
                }
                break;
            case ePaintModes.Flatten:
                break;
            case ePaintModes.Smooth:
                cursor.SmoothObjects(fBrushStrength / 30f);
                break;
            case ePaintModes.Random:
                cursor.RandomObjects(fBrushStrength / 20f);
                break;
        }
    }

    public Vector3 GetMovement() {
        Vector3 move = Vector3.zero;
        move.x = Input.GetAxis("Move Horizontal");
        move.y = Input.GetAxis("Up");
        move.z = -Input.GetAxis("Move Vertical");
        return move.normalized * fCameraMoveSpeed;
    }

    public Vector3 GetLook() {
        Vector2 look = Vector2.zero;
        look.x = -Input.GetAxis("Rotate Camera Y");
        look.y = Input.GetAxis("Rotate Camera X");
        return look*2f;
    }

    public bool GetAimMode() {
        return bAimMode;
    }
}
