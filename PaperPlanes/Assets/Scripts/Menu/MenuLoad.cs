using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoad : MonoBehaviour {

    public void LoadLevel(string level) {
        if (ButtonSounds.sounds != null) {
            ButtonSounds.sounds.PlayClick();
        }
        SceneManager.LoadScene(level);
    }
}
