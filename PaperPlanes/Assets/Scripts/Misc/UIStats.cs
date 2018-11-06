using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStats : MonoBehaviour {
    
    // Private
    private Text txt;
    private float fps = 0f;
    private float minFps = 1000f;
    private float[] arrFps = new float[60];
    private int iArrPos = 0;
    private bool bUseStats = false;

    // Use this for initialization
    void Start () {
        txt = this.GetComponent<Text>();
        if (!bUseStats) {
            txt.text = "";
        }
	}

    // Update is called once per frame
    void Update() {
        if(bUseStats){
            // UpdateCont();
            fps = (1f / Time.deltaTime);
            //if (iArrPos >= (arrFps.Length - 1)) {
                if (fps < minFps) {
                    minFps = fps;
                }
           // }

            if (iArrPos < 30) {
                arrFps[iArrPos] = fps;
                iArrPos++;
            } else {
                arrFps = ShuffleArr(arrFps, fps);
            }
            UpdateStats();
        }
    }

    private float[] ShuffleArr(float[] arr, float val) {
        for (int i = 0; i < (arr.Length - 1); i++) {
            arr[i] = arr[i + 1];
        }
        arr[arr.Length - 1] = val;
        return arr;
    }

    private void UpdateStats() {

        float fAvgFps = 0f;
        for (int i = 0; i < arrFps.Length; i++) {
            fAvgFps += arrFps[i];
        }
        fAvgFps /= arrFps.Length;

        txt.text = "Current FPS: " + ((int)fps).ToString() + "\n";
        txt.text += "Average FPS: " + ((int)fAvgFps).ToString() + "\n";
        txt.text += "Min FPS: " + ((int)minFps).ToString();
    }
}
