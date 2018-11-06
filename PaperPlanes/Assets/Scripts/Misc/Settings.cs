using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

    // Global
    public static Settings settings;

    // Public
    public bool bUSeFPSTarget = true;
    public int iFPSSmoothFrames = 15;
    public float fFPSTarget = 50f;
    public float fFPSUpperTarget = 57f;

    // Private
    public int iTerrainDistance = 3;
    private float fShadowDistance = 400f;
    public float fAvgFPS = 0f;
    private List<float> lstFPSAvg = new List<float>();

	void Awake () {
        if(settings == null){
            settings = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            DestroyImmediate(this.gameObject);
        }
	}

    public int GetTerrainDist() {
        return iTerrainDistance;
    }

    void Update() {
        CalcFPS();
        if (bUSeFPSTarget) {
            if(lstFPSAvg.Count >= iFPSSmoothFrames) {
                if(fAvgFPS < fFPSTarget) {
                    LowerSettings();
                } else if (fAvgFPS > fFPSUpperTarget) {
                    RaiseSettings();
                }
            }
        }
    }

    private void LowerSettings() {
        if(iTerrainDistance > 0) {
            iTerrainDistance--;
        }
    }

    private void RaiseSettings() {
        if(iTerrainDistance < 3) {
            iTerrainDistance++;
        }
    }

    private void CalcFPS() {
        if(lstFPSAvg.Count < iFPSSmoothFrames) {
            lstFPSAvg.Add(1f / Time.deltaTime);
        } else {
            float avg = 0f;
            for(int i = lstFPSAvg.Count-1; i > 0; i--) {
                lstFPSAvg[i] = lstFPSAvg[i - 1];
                avg += lstFPSAvg[i];
            }
            lstFPSAvg[0] = 1f / Time.deltaTime;
            avg += lstFPSAvg[0];
            fAvgFPS = avg / lstFPSAvg.Count;
        }
    }
}
