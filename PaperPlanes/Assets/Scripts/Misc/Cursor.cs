using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {

    public List<GameObject> lstCollide = new List<GameObject>();

    public void MoveObjects(Vector3 amt) {
        amt *= Time.deltaTime;
        for (int i = 0; i < lstCollide.Count; i++) {
            if (lstCollide[i] != null) {
                lstCollide[i].transform.position += amt;
            } else {
                lstCollide.RemoveAt(i);
            }
        }
    }

    public void SmoothObjects(float amt) {
        amt *= Time.deltaTime;

        float avgHeight = 0f;
        int numCounted = 0;

        for (int i = 0; i < lstCollide.Count; i++) {
            if(lstCollide[i] != null) {
                avgHeight += lstCollide[i].transform.position.y;
                numCounted++;
            }
        }

        avgHeight /= numCounted;
        
        for (int i = 0; i < lstCollide.Count; i++) {
            if (lstCollide[i] != null) {
                float diff = lstCollide[i].transform.position.y - avgHeight;
                diff = Mathf.Clamp(diff, -1f, 1f);
                lstCollide[i].transform.position -= new Vector3(0f, diff * amt, 0f);
            }
        }
    }

    public void RandomObjects(float amt) {
        amt *= Time.deltaTime;
        for (int i = 0; i < lstCollide.Count; i++) {
            if (lstCollide[i] != null) {
                lstCollide[i].transform.position += new Vector3(0f, Random.Range(-1f, 1f) * amt, 0f);
            }
        }
    }

    public void MoveObjects(Vector3 amt, Vector3 cursorPos, float linear, float maxDist) {
        amt *= Time.deltaTime;
        for (int i = 0; i < lstCollide.Count; i++) {

            if (lstCollide[i] != null) {
                float mult = 1.0f;
                float dist = Vector3.Distance(lstCollide[i].transform.position, cursorPos);
                mult = 1f-Mathf.Clamp(dist / maxDist, 0f, 1f);


                mult = mult * mult * (3f - 2f * mult);

                if (linear < 1) {
                    mult = Mathf.Lerp(1f, 1f - Mathf.Clamp(dist / maxDist, 0f, 1f), linear);
                } else if (linear > 1) {
                    mult = Mathf.Lerp(1f - Mathf.Clamp(dist / maxDist, 0f, 1f), mult, linear-1f);
                }
                
                lstCollide[i].transform.position += amt * mult;
            } else {
                lstCollide.RemoveAt(i);
            }
        }
    }

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Point") {
            lstCollide.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Point") {
            if (lstCollide.Contains(col.gameObject)) {
                lstCollide.Remove(col.gameObject);
            }
        }
    }
}
