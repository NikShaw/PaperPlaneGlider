using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCont : MonoBehaviour {

    // Public
    public GameObject gTilt;
    public GameObject gVJoy;

    // Private
    private bool bStarted = false;

	// Use this for initialization
	void Start () {
        Disable();
        LateStart(1f);
	}

    IEnumerator LateStart(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        bStarted = false;
    }

    void LateUpdate() {
        if (!bStarted) {
            bStarted = true;
            CheckInp();
        }
    }

    private void CheckInp() {
        if(UIManager.manager != null){
        PlayerInput.eControlStates eState;
        eState = UIManager.manager.GetControl();
        if(eState == PlayerInput.eControlStates.Tilt) {
            gVJoy.SetActive(true);
        } else if (eState == PlayerInput.eControlStates.VirtJoy) {
            gTilt.SetActive(true);
        } else if (eState == PlayerInput.eControlStates.Def) {
            gVJoy.SetActive(true);
        }
        Debug.Log(eState.ToString());
        }
    }

    private void Disable() {
        gTilt.SetActive(false);
        gVJoy.SetActive(false);
    }

    public void SetTilt() {
        Disable();
        gVJoy.SetActive(true);
        UIManager.manager.ChangeControl(PlayerInput.eControlStates.Tilt);
    }

    public void SetVJoy() {
        Disable();
        gTilt.SetActive(true);
        UIManager.manager.ChangeControl(PlayerInput.eControlStates.VirtJoy);
    }
}
