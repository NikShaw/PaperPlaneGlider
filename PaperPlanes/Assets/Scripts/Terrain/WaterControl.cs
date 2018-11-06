using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterControl : MonoBehaviour {

    // Global
    public static WaterControl ctrl;

    // Public
    public Material matWater;
    public float fWaterHeight = 0.5f;
    public float fWaterSpeed = 1.0f;
    public float fWaveLength = 1.0f;
    public float fWaterLevel = 0f;
    public bool bGlobalWater = false;
    public Vector2 vWaterPos = new Vector2(0f, 0f);
    public Vector2 vWaterDir = new Vector2(0f, 1f);

    // Private
    private float fSetWaterHeight = 0f;
    private float fSetWaterSpeed = 0f;
    private float fSetWaveLength = 0f;
    private Vector2 vSetWaterPos = Vector2.zero;
    private Vector2 vSetWaterDir = Vector2.zero;

    // Use this for initialization
    void Awake () {
		if(ctrl == null) {
            ctrl = this;
        } else {
            DestroyImmediate(this.gameObject);
        }
	}

    public float GetWaterHeight() {
        if(bGlobalWater){
            return fWaterLevel + fWaterHeight;
        } else {
            return -1000f;
        }
    }
	
	// Update is called once per frame
    void Update () {
        if (fSetWaterHeight != fWaterHeight) {
            matWater.SetFloat("_WaveHeight", fWaterHeight);
            fSetWaterHeight = fWaterHeight;
        }
        if (fSetWaterSpeed != fWaterSpeed) {
            matWater.SetFloat("_WaveSpeed", fWaterSpeed);
            fSetWaterSpeed = fWaterSpeed;
        }
        if (fSetWaveLength != fWaveLength) {
            matWater.SetFloat("_WaveLength", fWaveLength);
            fSetWaveLength = fWaveLength;
        }
        if (vSetWaterDir != vWaterDir) {
            matWater.SetVector("_WaveDir", vWaterDir);
            vSetWaterDir = vWaterDir;
        }
        if (vSetWaterPos != vWaterPos) {
            matWater.SetVector("_WavePos", vWaterPos);
            vSetWaterPos = vWaterPos;
        }
        vWaterPos += vWaterDir * fWaterSpeed * Time.deltaTime;
    }
}
