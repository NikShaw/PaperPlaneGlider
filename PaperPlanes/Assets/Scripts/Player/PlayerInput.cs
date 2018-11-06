using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    // public
    public enum eControlStates {VirtJoy, Cont, Tilt, Keyboard, Def}
    public int iSmoothness = 20;
    public float fTiltFactor = 10.0f;
    public float fRotSpeed = 1.0f;
    public float fTiltDecay = 0.9f;
    public bool bResetTilt = false;

    // private
    public eControlStates eControlState = eControlStates.Keyboard;
    private bool bGyroSupported = false;
    private bool bSetUIMan = false;
    private Vector2 vPrevAccelChange = Vector2.zero;
    private Player player;
    private List<Vector2> lstPrevAccelSmooth = new List<Vector2>();

    // Use this for initialization
    void Awake () {
#if UNITY_EDITOR
        eControlState = eControlStates.Cont;
#elif UNITY_ANDROID
        eControlState = eControlStates.VirtJoy;
#else
        eControlState = eControlStates.Cont;
#endif
        player = this.GetComponent<Player>();

        if (SystemInfo.supportsGyroscope) {
            Input.gyro.enabled = true;
            bGyroSupported = true;
        }
        for (int i = 0; i < iSmoothness; i++) {
            lstPrevAccelSmooth.Add(Vector3.zero);
        }
        if (UIManager.manager != null) {
            UIManager.manager.SetPlayerInput(this);
        } else {
            bSetUIMan = true;
        }
    }

    public void ChangeControl(eControlStates state) {
        eControlState = state;
    }

    public eControlStates GetControl() {
        return eControlState;
    }

    // Update is called once per frame
    void Update () {
        if (bSetUIMan) {
            if (UIManager.manager != null) {
                UIManager.manager.SetPlayerInput(this);
                bSetUIMan = false;
            }
        }
        switch (eControlState) {
            case eControlStates.VirtJoy:
                UpdateVirtJoy();
              //  player.UpdateCamRot();
                break;
            case eControlStates.Cont:
                UpdateCont();
                //RotateCam();
                break;
            case eControlStates.Tilt:
                UpdateTilt();
               // player.UpdateCamRot();
                break;
            case eControlStates.Keyboard:
                UpdateKeyboard();
                //RotateCam();
                break;
        }
	}

    private void RotateCam() {
        Vector2 tmpRot = Vector2.zero;
        tmpRot.x = Input.GetAxis("Rotate Camera X");
        tmpRot.y = Input.GetAxis("Rotate Camera Y");
        player.UpdateCam(tmpRot);
        if (Input.GetButtonDown("Reset Camera")) {
            player.ResetCam();
        }
    }

    private void UpdateVirtJoy() {
        Vector2 tmpMove = Vector2.zero;
        if(UIManager.manager != null){
            tmpMove = -UIManager.manager.GetVirtJoy();
        }
        player.UpdateCont(tmpMove);
        player.UpdateLimitedCont(tmpMove);
    }

    private void UpdateCont() {
        Vector2 tmpMove = Vector2.zero;
        tmpMove.x = -Input.GetAxis("Move Horizontal");
        tmpMove.y = Input.GetAxis("Move Vertical");

        if (tmpMove.magnitude > 1f) {
            tmpMove/=tmpMove.magnitude;
        }
        player.UpdateCont(tmpMove);
        player.UpdateLimitedCont(tmpMove);
    }

    public void ResetTilt() {
        vPrevAccelChange = Vector2.zero;
    }

    private void UpdateTilt() {
        Vector2 tmpMove = Vector2.zero;

        if (bGyroSupported) {
            vPrevAccelChange.x += Input.gyro.rotationRateUnbiased.z;
            vPrevAccelChange.y += Input.gyro.rotationRateUnbiased.x;
            //Debug.Log(Input.gyro.rotationRateUnbiased);
            //transform.rotation = Input.gyro.attitude;
            //vPrevAccelChange.y += Input.gyro.rotationRateUnbiased.x/ fTiltFactor;
            if (bResetTilt) {
                ResetTilt();
                bResetTilt = false;
            }
            tmpMove = vPrevAccelChange / fTiltFactor;
            vPrevAccelChange *= fTiltDecay;
        } else {
            //player.Rotate(new Vector3(-Input.acceleration.x, -Input.acceleration.y, 0f));
            Vector2 vec = new Vector2(((Input.acceleration.x * -2f) ) * fRotSpeed,( (Input.acceleration.y * -2f)) * fRotSpeed);
            for (int i = 0; i < iSmoothness - 1; i++) {
                lstPrevAccelSmooth[i] = lstPrevAccelSmooth[i + 1];
            }
            lstPrevAccelSmooth[iSmoothness - 1] = vec;
            Vector2 tmpvec = Vector2.zero;
            for (int i = 0; i < iSmoothness; i++) {
                tmpvec += lstPrevAccelSmooth[i];
            }
            tmpvec /= iSmoothness;
            tmpMove = tmpvec;
        }
        if (tmpMove.magnitude > 1f) {
            tmpMove.Normalize();
        }
        if (UIManager.manager != null) {
            UIManager.manager.SetVirtJoy(-tmpMove);
        }
        player.UpdateCont(tmpMove);
        player.UpdateLimitedCont(tmpMove);
    }

    private void UpdateKeyboard() {
        Vector2 tmpMove = Vector2.zero;
        if (Input.GetKey("w")) {
            tmpMove.y -= 1f;
        }
        if (Input.GetKey("s")) {
            tmpMove.y += 1f;
        }
        if (Input.GetKey("a")) {
            tmpMove.x += 1f;
        }
        if (Input.GetKey("d")) {
            tmpMove.x -= 1f;
        }
        player.UpdateCont(tmpMove.normalized);
        player.UpdateLimitedCont(tmpMove);
    }
}
