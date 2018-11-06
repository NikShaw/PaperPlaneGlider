using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // public
    public float fConstForce = 10f;
    public float fContMaxAng = 10.0f;
    public float fContMaxVel = 10.0f;
    public float fContMaxVelMult = 40f;
    public float fRudderMaxAng = 15f;
    public float fTurnAngTarg = 15f;
    public float fWindVolMax = 5f;
    public float fVelVolMax = 40f;
    public float fWind2Intro = 0.3f;
    public Vector2 vContDefAng = new Vector2(10f, 0f);
    public Vector2 vContVelMult = new Vector2(1f, 0.2f);
    public GameObject gCamPos;
    public GameObject gCamRot;
    public GameObject gLeftCont;
    public GameObject gRightCont;
    public GameObject gRudder;

    // private
    private float fUpdraft = 0f;
    private float fPitchAng = 0f;
    private float fWindVol = 0f;
    private float fVelVol = 0f;
    private bool bPitching = false;
    private AudioSource audWind;
    private AudioSource audWind2;
    private Vector3 vWind = Vector3.zero;
    private Vector2 vCurrContRot = Vector2.zero;
    private Vector3 vInitCamPos;
    private Quaternion qInitRot;
    private Rigidbody rgdBdy;
    private List<DragPoint> lstDragPoints = new List<DragPoint>();

	// Use this for initialization
	void Start () {
        rgdBdy = this.GetComponent<Rigidbody>();
        qInitRot = gCamRot.transform.localRotation;
        vInitCamPos = gCamRot.transform.position;
        audWind = this.GetComponents<AudioSource>()[0];
        audWind2 = this.GetComponents<AudioSource>()[1];
        audWind.volume = 0f;
        audWind2.volume = 0f;
        SetWind(Vector3.zero);
    }
	
	// Update is called once per frame
	/*void Update () {
        // UpdateCont();
        fps = (1f / Time.deltaTime);
        if(iArrPos >= (arrFps.Length-1)){
            if(fps < minFps) {
                minFps = fps;
            }
        }

        if (iArrPos < 30) {
            arrFps[iArrPos] = fps;
            iArrPos++;
        } else {
            arrFps = ShuffleArr(arrFps, fps);
        }
    }

    private float[] ShuffleArr(float[] arr, float val) {
        for(int i = 0; i < (arr.Length-1); i++) {
            arr[i] = arr[i + 1];
        }
        arr[arr.Length - 1] = val;
        return arr;
    }?*/
/*
    void OnGUI() {
        GUIStyle gStyle = new GUIStyle();
        gStyle.fontSize = 48;

        float fAvgFps = 0f;
        for(int i = 0; i < arrFps.Length; i++) {
            fAvgFps += arrFps[i];
        }
        fAvgFps /= arrFps.Length;


        GUI.Label(new Rect(10, 90, 70, 20), "Average: " + fAvgFps.ToString(), gStyle);
        GUI.Label(new Rect(10, 20, 70, 20), "Current: " + fps.ToString(), gStyle);
        GUI.Label(new Rect(10, 160, 70, 20), "Min: " + minFps.ToString(), gStyle);
    }*/

    void FixedUpdate() {
        rgdBdy.AddForce(this.transform.forward * fConstForce);
        UpdateCamRot();
        UpdateVolume();
    }

    private void UpdateVolume() {
        fVelVol = Mathf.Lerp(0f, 0.5f, rgdBdy.velocity.magnitude/fVelVolMax);
        fWindVol = Mathf.Lerp(0f, 0.5f, (vWind.magnitude + fUpdraft)/fWindVolMax);
        audWind.volume = fWindVol + fVelVol;
        audWind2.volume = Mathf.Lerp(0f, 1f, (audWind.volume - fWind2Intro) / (1f - fWind2Intro));
    }

    public void UpdateCamRot() {
        if (rgdBdy.velocity.magnitude > 1f) {
            gCamRot.transform.LookAt(this.transform.position + rgdBdy.velocity);
        } else {
            gCamRot.transform.LookAt(this.transform.position + this.transform.forward);
        }
    }

    public void UpdateCam(Vector2 rot) {
        gCamRot.transform.localRotation *= Quaternion.Euler(new Vector3(rot.y*0f, rot.x, 0f));
    }

    public void ResetCam() {
        gCamRot.transform.LookAt(this.transform.position + this.transform.forward, this.transform.up);
    }

    public void UpdateLimitedCont(Vector2 move) {
        if(rgdBdy != null){
            float contAng = Mathf.Lerp(vContDefAng.x, vContDefAng.y, rgdBdy.velocity.magnitude / fContMaxVel);
            float planeAng = this.transform.right.y * 180f;
            float pitchAng = this.transform.forward.y * 180f;
            float planeAngTarg = fTurnAngTarg * Mathf.Clamp(move.x, -1f, 1f) * Mathf.Lerp(vContVelMult.x, vContVelMult.y, rgdBdy.velocity.magnitude / fContMaxVelMult);
            float rudderTurn = 0f;

            Vector2 contMove = new Vector2(contAng, contAng);
            Vector2 tmpMove = Vector2.zero;
        
            if (move.y != 0f) {
                bPitching = true;
                fPitchAng = pitchAng + (move.y * 90f);
            } else if (bPitching) {
                fPitchAng = pitchAng;
                bPitching = false;
            }

            if (planeAngTarg > planeAng) {
                float perc = (planeAngTarg - planeAng)/fTurnAngTarg;
                tmpMove.x += Mathf.Lerp(0f, 1f, perc);
                tmpMove.y -= tmpMove.x;
            } else if (planeAngTarg < planeAng) {
                float perc = (planeAng - planeAngTarg) / fTurnAngTarg;
                tmpMove.x += Mathf.Lerp(0f, -1f, perc);
                tmpMove.y -= tmpMove.x;
                //move.y += perc;
            }

            rudderTurn = planeAng / fTurnAngTarg;

           // if(!bPitching) {
            if (fPitchAng > pitchAng) {
                float perc = (fPitchAng - pitchAng) / fTurnAngTarg;
                move.y = Mathf.Lerp(0f, 1f, perc);
                //tmpMove.y -= tmpMove.x;
            } else if (fPitchAng < pitchAng) {
                float perc = (pitchAng - fPitchAng) / fTurnAngTarg;
                move.y = Mathf.Lerp(0f, -1f, perc);
                //move.y += perc;
            }
            move.y += Mathf.Abs(this.transform.right.y) * 0.75f;
            //  }
            tmpMove.x += move.y;
            tmpMove.y += move.y;

            tmpMove *= fContMaxAng;
            rudderTurn *= fRudderMaxAng;
            tmpMove *= Mathf.Lerp(vContVelMult.x, vContVelMult.y, rgdBdy.velocity.magnitude / fContMaxVelMult);
            rudderTurn *= Mathf.Lerp(vContVelMult.x, vContVelMult.y, rgdBdy.velocity.magnitude / fContMaxVelMult);
            contMove += tmpMove;
        

            //Debug.Log(planeAng + " | " + planeAngTarg + " | " + tmpMove + " | " + contMove + " | " + rudderTurn);
            gLeftCont.transform.localRotation = Quaternion.Euler(new Vector3(contMove.x, 0f, 0f));
            gRightCont.transform.localRotation = Quaternion.Euler(new Vector3(contMove.y, 0f, 0f));
            gRudder.transform.localRotation = Quaternion.Euler(new Vector3(0f, rudderTurn, 0f));
        }
    }

    public void ResetDragPoints() {
        for(int i = 0; i < lstDragPoints.Count; i++) {
            lstDragPoints[i].SkipNext();
        }
    }

    public void UpdateCont(Vector2 move) {
        if(rgdBdy != null){
            float contAng = Mathf.Lerp(vContDefAng.x, vContDefAng.y, rgdBdy.velocity.magnitude / fContMaxVel);
            Vector2 contMove = new Vector2(contAng, contAng);
            Vector2 tmpMove = Vector2.zero;
            tmpMove.x += move.y;
            tmpMove.y += move.y;
            tmpMove.x += move.x;
            tmpMove.y -= move.x;

            tmpMove.x = Mathf.Clamp(tmpMove.x, -1f, 1f);
            tmpMove.y = Mathf.Clamp(tmpMove.y, -1f, 1f);
            tmpMove *= fContMaxAng;
            tmpMove *= Mathf.Lerp(vContVelMult.x, vContVelMult.y, rgdBdy.velocity.magnitude / fContMaxVelMult);
            contMove += tmpMove;
        }

       // gLeftCont.transform.localRotation = Quaternion.Euler(new Vector3(contMove.x, 0f, 0f));
       // gRightCont.transform.localRotation = Quaternion.Euler(new Vector3(contMove.y, 0f, 0f));
    }

    public void AddDrag(DragPoint drag) {
        lstDragPoints.Add(drag);
    }

    public void SetWind(Vector3 wind) {
        if (Level.level != null) {
            fUpdraft = Level.level.GetUpdraft();
        }
        vWind = wind;
        for(int i = 0; i < lstDragPoints.Count; i++) {
            lstDragPoints[i].SetWind(wind + (new Vector3(0f, fUpdraft, 0f)));
        }
    }

    public GameObject GetCamPos() {
        return gCamPos;
    }
}
