using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    // global
    public static UIManager manager;

    // public
    public GameObject gVirtJoyUI;
    public GameObject gContUI;
    public GameObject gTiltUI;
    public GameObject gKeyUI;
    public GameObject gStart;
    public GameObject gPauseMenu;
    public GameObject gEndMenu;
    public GameObject gFailMenu;
    public GameObject gNextMenu;
    public GameObject gRetryMenu;
    public GameObject gTimerMenu;
    public Color cTimerDef = Color.white;
    public Color cTimerEnd = Color.red;

    // private
    private string sWorldToLoad = "";
    private bool bTimer = false;
    private bool bUseTilt = false;
    private PlayerInput pInp;
    private UIVirtJoy uiJoy;
    private Level mLevel;
    private PlayerInput.eControlStates eControlState = PlayerInput.eControlStates.Def;

    void Awake() {
        Debug.Log("Awake");
        if (manager == null) {
            manager = this;
        } else {
            Debug.Log("Delete");
            DestroyImmediate(this.gameObject);
        }
    }

    /*void OnGUI() {
        GUI.Label(new Rect(10, 10, 100, 20), eControlState.ToString());
    }*/

    public void SetPlayerInput(PlayerInput inp) {
        pInp = inp;
        PlayerInput.eControlStates state = pInp.GetControl();
        /*if(eControlState == PlayerInput.eControlStates.Def) {
            ChangeControl(pInp.GetControl());
        } else {
            ChangeControl(eControlState);
        }*/
        if ((state == PlayerInput.eControlStates.Tilt) || (state == PlayerInput.eControlStates.VirtJoy)) {
            if(PlayerPrefs.HasKey("UseTilt")){
                if (PlayerPrefs.GetInt("UseTilt") == 1) {
                    ChangeControl(PlayerInput.eControlStates.Tilt);
                } else {
                    ChangeControl(PlayerInput.eControlStates.VirtJoy);
                } 
            } else {
                ChangeControl(state);
            }
        } else {
            ChangeControl(state);
        }
    }

	// Use this for initialization
	void Start () {
        uiJoy = gVirtJoyUI.GetComponent<UIVirtJoy>();
        //DisableContMenus();
        DisableMenus();
    }
	
    public void SetLevel(Level l) {
        mLevel = l;
    }

    public void Fail() {
        DisableMenus();
        DisableContMenus();
        gFailMenu.SetActive(true);
        gRetryMenu.SetActive(true);
    }

    public void Success() {
        DisableMenus();
        DisableContMenus();
        gEndMenu.SetActive(true);
        gRetryMenu.SetActive(true);
        if (ButtonSounds.sounds != null) {
            ButtonSounds.sounds.PlayDing();
        }
    }

    public void Pause() {
        ButtonClick();
        if(mLevel.IsStarted()) {
            mLevel.Pause();
            if (mLevel.IsPaused()) {
                DisableMenus();
                gPauseMenu.SetActive(true);
                gRetryMenu.SetActive(true);
            } else {
                DisableMenus();
            }
        }
    }

    public void ButtonClick() {
        if(ButtonSounds.sounds != null) {
            ButtonSounds.sounds.PlayClick();
        }
    }

    public void DisableStart() {
        gStart.SetActive(false);
    }

    public Vector2 GetVirtJoy() {
        return uiJoy.GetInput();
    }

    public void SetVirtJoy(Vector2 inp) {
        uiJoy.SetInput(inp);
    }

    public void ReturnToMenu() {
        ButtonClick();
        SceneManager.LoadScene(0);
    }

    public void ShowNextLevel(string difficulty, int world, int level) {
        sWorldToLoad = difficulty + "World" + world.ToString() + "-" + level.ToString();
        gNextMenu.SetActive(true);
    }

    public void ShowTimer() {
        gTimerMenu.SetActive(true);
        bTimer = true;
    }

    public void UpdateTimer(string timer, float col) {
        gTimerMenu.GetComponent<Text>().text = timer;
        gTimerMenu.GetComponent<Text>().color = Color.Lerp(cTimerDef, cTimerEnd, col);
        if(gTimerMenu.activeSelf == false) {
            gTimerMenu.SetActive(true);
        }
    }

    public void NextLevel() {
        ButtonClick();
        SceneManager.LoadScene(sWorldToLoad);
    }

    public void Retry() {
        ButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public PlayerInput.eControlStates GetControl() {
        return eControlState;
    }

    public void ChangeControl(PlayerInput.eControlStates state) {
        DisableContMenus();
        eControlState = state;
        switch (state) {
            case PlayerInput.eControlStates.Cont:
                gContUI.SetActive(true);
                break;
            case PlayerInput.eControlStates.Keyboard:
                gKeyUI.SetActive(true);
                break;
            case PlayerInput.eControlStates.Tilt:
                gTiltUI.SetActive(true);
                gVirtJoyUI.SetActive(true);
                bUseTilt = true;
                PlayerPrefs.SetInt("UseTilt", 1);
                break;
            case PlayerInput.eControlStates.VirtJoy:
                gVirtJoyUI.SetActive(true);
                bUseTilt = false;
                PlayerPrefs.SetInt("UseTilt", 0);
                break;
        }
        pInp.ChangeControl(eControlState);
    }

    /*public PlayerInput.eControlStates GetControl() {
        return 
    }*/

    private void DisableMenus() {
        gPauseMenu.SetActive(false);
        gEndMenu.SetActive(false);
        gFailMenu.SetActive(false);
        gNextMenu.SetActive(false);
        gRetryMenu.SetActive(false);
        gTimerMenu.SetActive(false);
    }

    private void DisableContMenus() {
        gVirtJoyUI.SetActive(false);
        gContUI.SetActive(false);
        gTiltUI.SetActive(false);
        gKeyUI.SetActive(false);
    }
}
