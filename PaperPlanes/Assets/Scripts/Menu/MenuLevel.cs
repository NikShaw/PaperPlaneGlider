using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLevel : MonoBehaviour {

    // Public
    public string sWorldName = "";
    public string sDifficulty = "";
    public int iWorldNum = 0;
    public int iStarsNeeded = 2;
    public GameObject gLevelSelect;
    public GameObject gLocked;
    public List<GameObject> lstStars = new List<GameObject>();

    // Private
    private bool bUnlocked = false;
    private List<bool> lstStarGot = new List<bool>();

    // Use this for initialization
    void Start () {
        lstStarGot.Clear();
        for(int i = 0; i < lstStars.Count; i++) {
            lstStarGot.Add(false);
        }
        DisableStars();
        CheckStars();
	}

    private void CheckStars() {
        if(iWorldNum > 1) {
            int numStars = 0;
            int tmpInt1 = PlayerPrefs.GetInt(sDifficulty + "World" + (iWorldNum - 1).ToString() + "-1");
            int tmpInt2 = PlayerPrefs.GetInt(sDifficulty + "World" + (iWorldNum - 1).ToString() + "-2");
            int tmpInt3 = PlayerPrefs.GetInt(sDifficulty + "World" + (iWorldNum - 1).ToString() + "-3");

            if (tmpInt1 == 1) {
                numStars++;
            }
            if (tmpInt2 == 1) {
                numStars++;
            }
            if (tmpInt3 == 1) {
                numStars++;
            }

            if (numStars >= iStarsNeeded) {
                bUnlocked = true;
                gLocked.SetActive(false);
            } else {
                bUnlocked = false;
                gLocked.SetActive(true);
            }
        } else {
            bUnlocked = true;
            gLocked.SetActive(false);
        }


        if(sWorldName != "") {
            for(int i = 0; i < lstStarGot.Count; i++) {
                int tmpInt = PlayerPrefs.GetInt(sDifficulty + "World" + iWorldNum.ToString() + "-" + (i+1).ToString());
                if(tmpInt == 0) {
                    lstStarGot[i] = false;
                } else {
                    if(bUnlocked){
                        lstStarGot[i] = true;
                        lstStars[i].SetActive(true);
                    } else {
                        lstStarGot[i] = false;
                        lstStars[i].SetActive(false);
                    }
                }
            }
        }
    }

    public void ActivateLevel() {
        if(bUnlocked){
            if (ButtonSounds.sounds != null) {
                ButtonSounds.sounds.PlayClick();
            }
            gLevelSelect.SetActive(true);
            gLevelSelect.GetComponent<MenuLevelSelect>().SetWorld(sDifficulty + sWorldName, lstStarGot);
        }
    }

    private void DisableStars() {
        for(int i = 0; i < lstStars.Count; i++) {
            lstStars[i].SetActive(false);
        }
    }
}
