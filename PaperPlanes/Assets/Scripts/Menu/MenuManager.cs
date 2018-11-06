using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    // Global
    public enum eMenuStates {Main, FreeFly, Settings, Challenge};

    // Public
    public GameObject gChallenge;
    public GameObject gFreeFly;
    public GameObject gMain;
    public GameObject gSettings;
    public int iPrevStates = 1;

    // Private
    private eMenuStates eState;
    private eMenuStates ePrevState;

	// Use this for initialization
	void Start () {
        eState = eMenuStates.Main;
        DisableMenus();
        gMain.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeState(string state) {
    //  if(state != eState){
        ePrevState = eState;
        DisableMenus();
        switch (state) {
            case "Main":
                eState = eMenuStates.Main;
                gMain.SetActive(true);
                break;
            case "FreeFly":
                eState = eMenuStates.FreeFly;
                gFreeFly.SetActive(true);
                break;
            case "Challenge":
                eState = eMenuStates.Challenge;
                gChallenge.SetActive(true);
                break;
            case "Settings":
                eState = eMenuStates.Settings;
                gSettings.SetActive(true);
                break;
        }
    }

    public void Quit() {
        Application.Quit();
    }

   /* public void GoBack() {
        ChangeState(ePrevState);
    }*/

    private void DisableMenus() {
        gMain.SetActive(false);
        gChallenge.SetActive(false);
        gFreeFly.SetActive(false);
        gSettings.SetActive(false);
    }
}
