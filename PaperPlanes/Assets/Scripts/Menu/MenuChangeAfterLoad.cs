using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuChangeAfterLoad : MonoBehaviour {

    // Public
    public string sMenu = "";

    // Private
    private bool bChange = false;

	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(this.gameObject);
	}

    IEnumerator LateStart(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        if (bChange) {
            GameObject tmp = GameObject.Find("Menus");
            tmp.GetComponent<MenuManagerGlobal>().ChangeMenu(sMenu);
        }    
    }

    private void OnLevelWasLoaded(int level) {
        if (level == 0) {
            if(sMenu != "") {
                bChange = true;
            }
        }
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        if(scene.buildIndex == 0) {
            StartCoroutine(LateStart(0));
        }
        DestroyImmediate(this.gameObject);
    }
}
