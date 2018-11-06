using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMan : MonoBehaviour {

    // Public
    public Level mLevel;

	public void StartLevel() {
        mLevel.StartLevel();
    }

    public void ResetTilt() {
        mLevel.ResetTilt();
    }
}
