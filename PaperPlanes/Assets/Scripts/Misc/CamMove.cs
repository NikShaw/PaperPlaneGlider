using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour {

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        MoveCam();
        RotateCam();
    }

    private void RotateCam() {
        if (EditorControls.controls.GetAimMode()) {
            Vector2 mouseMove = EditorControls.controls.GetLook();
            this.transform.localEulerAngles += new Vector3(-mouseMove.x, mouseMove.y);
        }
    }

    private void MoveCam() {
        Vector3 movement = EditorControls.controls.GetMovement();
        if (movement != Vector3.zero) {
            Vector3 forw = this.transform.position + this.transform.forward;
            Vector3 up = this.transform.position + this.transform.up;
            if (!EditorControls.controls.GetAimMode()) {
                forw = new Vector3(forw.x, this.transform.position.y, forw.z);
                up = new Vector3(this.transform.position.x, up.y, this.transform.position.z);
            }
            Vector3 vec = forw - this.transform.position;
            this.transform.position += vec.normalized * Time.deltaTime * movement.z;
            vec = up - this.transform.position;
            this.transform.position += vec.normalized * Time.deltaTime * movement.y;
            this.transform.position += this.transform.right * Time.deltaTime * movement.x;
        }
    }
}
