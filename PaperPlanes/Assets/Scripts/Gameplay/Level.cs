using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    // global
    public static Level level;

    // public
    public string sWorldName = "";
    public float fTimeLimit = -1f;
    public float fWorldUpdraft = 0f;
    public float fStartVel = 10f;
    public bool bSequential = false;
    public bool bFreeRoam = false;
    public GameObject gPlayerPre;
    public GameObject gStartPos;
    public List<GameObject> lstCheckpoints = new List<GameObject>();

    // private
    private int iCurrCheckpoint = 0;
    private int iWorld = -1;
    private int iLevel = -1;
    private float fLevelTimerColChange = 10f;
    public float fLevelTimer = 0f;
    private string sDifficulty = "";
    private bool bStarted = false;
    private bool bPaused = false;
    private bool bFinished = false;
    private bool bCompleted = false;
    private GameObject gPlayer;
    private Player player;
    private CamFollow camFollow;
    private List<Checkpoint> lstCheck = new List<Checkpoint>();

    void Awake() {
        level = this;
    }

	// Use this for initialization
	void Start () {
        if(PlayerPrefs.GetInt(sWorldName) == 1){
            bCompleted = true;
        }
        InitPlayer();
        InitCam();
        InitCheckpoints();
        Time.timeScale = 0f;
        UIManager.manager.SetLevel(this);
        if(!bFreeRoam){
            string[] worldSplit = sWorldName.Split('d');
            string[] levelSplit = worldSplit[worldSplit.Length - 1].Split('-');
            sDifficulty = sWorldName.Split('W')[0];
            int.TryParse(levelSplit[0], out iWorld);
            int.TryParse(levelSplit[1], out iLevel);
            Debug.Log("Difficulty: " + sDifficulty + " | World: " + iWorld.ToString() + " | Level: " + iLevel.ToString());
            if (fTimeLimit > 0f) {
                UIManager.manager.ShowTimer();
                UpdateTime();
            }
        }
    }

    private void UpdateTime() {
        float perc = 0f;
        if((fTimeLimit - fLevelTimer) < fLevelTimerColChange) {
            perc = 1f-((fTimeLimit - fLevelTimer) / fLevelTimerColChange);
        }
        UIManager.manager.UpdateTimer(((int)(fTimeLimit-fLevelTimer)).ToString(), perc);
    }

    private void InitPlayer() {
        gPlayer = Instantiate(gPlayerPre);
        // gPlayer.GetComponent<Rigidbody>().isKinematic = true;
        player = gPlayer.GetComponent<Player>();
        gPlayer.transform.position = gStartPos.transform.position;
        gPlayer.transform.rotation = gStartPos.transform.rotation;
        player.ResetDragPoints();
    }

    private void InitCam() {
        GameObject camPos = player.GetCamPos();
        Camera.main.transform.position = camPos.transform.position;
        Camera.main.transform.rotation = camPos.transform.rotation;
        Camera.main.GetComponent<CamFollow>().SetTarget(camPos);
    }

    private void InitCheckpoints() {
        for(int i = 0; i < lstCheckpoints.Count; i++) {
            lstCheck.Add(lstCheckpoints[i].GetComponent<Checkpoint>());
            lstCheck[i].Init();
        }
        lstCheck[0].SetActive();
        if(lstCheck.Count > 1){
            lstCheck[1].SetNext();
        }
    }

    public void ResetTilt() {
        player.GetComponent<PlayerInput>().ResetTilt();
    }

	// Update is called once per frame
	void Update () {
        if (bStarted && !bFinished && !bFreeRoam) {
            fLevelTimer += Time.deltaTime;
            UpdateCheckpoints();
            if(fTimeLimit > 0f){
                UpdateTime();
            }
            if((fLevelTimer > fTimeLimit) && (fTimeLimit > 0f)) {
                FailLevel();
            }
        }
	}

    private void UpdateCheckpoints() {
        if (lstCheck[iCurrCheckpoint].PlayerHit()) {
            CheckHit();
            if (iCurrCheckpoint < (lstCheck.Count - 2)) {
                lstCheck[iCurrCheckpoint].Disable();
                lstCheck[iCurrCheckpoint + 1].SetActive();
                lstCheck[iCurrCheckpoint + 2].SetNext();
                iCurrCheckpoint++;
            } else if (iCurrCheckpoint < (lstCheck.Count - 1)) {
                lstCheck[iCurrCheckpoint].Disable();
                lstCheck[iCurrCheckpoint + 1].SetActive();
                iCurrCheckpoint++;
            } else {
                lstCheck[iCurrCheckpoint].Disable();
                EndLevel();
            }
        }
    }

    private void CheckHit() {
        if (ButtonSounds.sounds != null) {
            ButtonSounds.sounds.PlayCheck();
        }
    }

    public void Pause() {
        if (!bPaused) {
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
        bPaused = !bPaused;
    }

    public bool IsPaused() {
        return bPaused;
    }

    public bool IsStarted() {
        return bStarted;
    }

    public void FailLevel() {
        bFinished = true;
        Camera.main.GetComponent<CamFollow>().SetTarget(null);
        UIManager.manager.Fail();
        if (bCompleted) {

        }
    }

    public void EndLevel() {
        bFinished = true;
        Camera.main.GetComponent<CamFollow>().SetTarget(null);
        UIManager.manager.Success();
        if(iLevel < 3) {
            UIManager.manager.ShowNextLevel(sDifficulty, iWorld, iLevel+1);
        } else if (iWorld < 6) {
            UIManager.manager.ShowNextLevel(sDifficulty, iWorld+1, 1);
        }
        if (!bCompleted){
            PlayerPrefs.SetInt(sWorldName, 1);
            bCompleted = true;
        }
    }

    public void StartLevel() {
        bStarted = true;
        Time.timeScale = 1f;
        UIManager.manager.DisableStart();
        ResetTilt();
        gPlayer.GetComponent<Rigidbody>().velocity = gStartPos.transform.forward * fStartVel;
    }

    public float GetUpdraft() {
        return fWorldUpdraft;
    }
}
