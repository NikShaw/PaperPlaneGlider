using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLevelSelect : MonoBehaviour {

    // Public
    public List<GameObject> lstGStars = new List<GameObject>();

    // Private
    private string sWorldToLoad = "";
    private List<bool> lstStars = new List<bool>();

	// Use this for initialization
	void Start () {
	}

    public void LoadWorld(int worldNum) {
        if (ButtonSounds.sounds != null) {
            ButtonSounds.sounds.PlayClick();
        }
        Debug.Log(sWorldToLoad + "-" + worldNum.ToString());
        SceneManager.LoadScene(sWorldToLoad + "-" + worldNum.ToString());
    }

    public void SetWorld(string world, List<bool> activeStars) {
        sWorldToLoad = world;
        lstStars = activeStars;
        CheckStars();
    }

    private void CheckStars() {
        for(int i = 0; i < lstGStars.Count; i++) {
            if (lstStars[i]) {
                lstGStars[i].SetActive(true);
            } else {
                lstGStars[i].SetActive(false);
            }
        }
    }

    public void Close() {
        this.gameObject.SetActive(false);
    }
}
