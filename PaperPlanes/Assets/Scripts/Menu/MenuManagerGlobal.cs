using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerGlobal : MonoBehaviour {

    // Public
    public List<GameObject> lstMenus = new List<GameObject>();

    // Private
    private int iCurrMenu = -1;

	// Use this for initialization
	void Start () {
        ChangeMenu(0);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void ButtonClick() {
        if (ButtonSounds.sounds != null) {
            ButtonSounds.sounds.PlayClick();
        }
    }

    public void ChangeMenu(int i) {
        ButtonClick();
        if((i != iCurrMenu) && (i < lstMenus.Count)) {
            iCurrMenu = i;
            DisableMenus();
            lstMenus[i].SetActive(true);
        }
    }

    public void ChangeMenu(string n) {
        ButtonClick();
        if(lstMenus[iCurrMenu].name != n) {
            for (int i = 0; i < lstMenus.Count; i++) {
                if(lstMenus[i].name == n) {
                    iCurrMenu = i;
                    DisableMenus();
                    lstMenus[i].SetActive(true);
                    break;
                }
            }
        }
    }

    private void DisableMenus() {
        for(int i = 0; i < lstMenus.Count; i++) {
            lstMenus[i].SetActive(false);
        }
    }
}