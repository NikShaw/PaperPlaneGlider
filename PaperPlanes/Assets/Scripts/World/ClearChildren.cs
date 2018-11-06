using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ClearChildren : MonoBehaviour {

    public bool bClear = true;

	// Use this for initialization
	void Start () {
        if (Application.isPlaying) {
            //Clear();
        }
	}

    void Update() {
        if(bClear) {
            bClear = false;
            Clear();
        }
    }

    private void Clear() {
        for (int i = this.transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }
}
