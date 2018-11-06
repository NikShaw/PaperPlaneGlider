using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updraft : MonoBehaviour {

    // public
    public int iMaxAnims = 10;
    public float fAnimSpawnTime = 5f;
    public float fAnimFadeTime = 2f;
    public float fEdgeSize = 0.1f;
    public Vector3 vDir = new Vector3(0f, 1f, 0.2f);
    public GameObject gUpAnimPre;
    public GameObject gBox;

    // private
    private int iCurrAnim = 0;
    private Vector2 vDist = new Vector2(250f, 600f);
    private float fMaxAnimSpawnTime = 30f;
    private float fAnimSpawnTimer = 999f;
    public Vector3 vSize = Vector3.zero;
    private List<GameObject> lstUpAnims = new List<GameObject>();

	// Use this for initialization
	void Start () {
        iMaxAnims = (int)(fAnimFadeTime / fAnimSpawnTime);
        vSize = this.GetComponent<BoxCollider>().size/2f;
        InitAnims();
        gBox.transform.localScale = vSize*2f;
    }

    private void InitAnims() {
        for(int i = 0; i < iMaxAnims; i++) {
            GameObject tmpObj = Instantiate(gUpAnimPre);
            tmpObj.transform.parent = this.transform;
            tmpObj.transform.localPosition = Vector3.zero;
            lstUpAnims.Add(tmpObj);
            tmpObj.SetActive(false);
        }
    }

	// Update is called once per frame
	void Update () {
        UpdateAnims();
	}

    private void UpdateAnims() {
        float dist = Vector3.Distance(Camera.main.transform.position, this.transform.position);
        float tmpTimer = fAnimSpawnTime;
        if(dist > vDist.x) {
            tmpTimer = Mathf.Lerp(fAnimSpawnTime, fMaxAnimSpawnTime, (dist - vDist.x)/(vDist.y-vDist.x));
        }
        fAnimSpawnTimer += Time.deltaTime;
        if(fAnimSpawnTimer > tmpTimer) {
            fAnimSpawnTimer = 0f;
            SpawnAnim();
        }
    }

    private void SpawnAnim() {
        int currInc = 0;
       // bool success = false;
        lstUpAnims[iCurrAnim].SetActive(true);
        Vector3 localPos = new Vector3(Random.Range(-vSize.x, vSize.x), Random.Range(-vSize.y, vSize.y), Random.Range(-vSize.z, vSize.z));
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(this.transform.TransformPoint(localPos));

        while (viewportPoint.z <= 0f) {
            localPos = new Vector3(Random.Range(-vSize.x, vSize.x), Random.Range(-vSize.y, vSize.y), Random.Range(-vSize.z, vSize.z));
            viewportPoint = Camera.main.WorldToViewportPoint(this.transform.TransformPoint(localPos));
            currInc++;
            if(currInc >= 3) {
                return;
            }
        }

        lstUpAnims[iCurrAnim].transform.localPosition = localPos;
        lstUpAnims[iCurrAnim].GetComponent<UpdraftAnim>().StartAnim(vDir);
       // if (!lstUpAnims[iCurrAnim].GetComponent<UpdraftAnim>().StartAnim(vDir)){//, this.transform.position - (vDir.normalized * vSize.y), this.transform.position + (vDir.normalized * vSize.y))) {
       // if(currInc >= 3){
       //    return;
       // }
       //   }
        iCurrAnim++;
        if(iCurrAnim >= (iMaxAnims - 1)) {
            iCurrAnim = 0;
        }
    }

    private float GetStrengthAtPos(Vector3 pos) {
        float strength = 0f;

        if (pos.y < 0f) {
            pos.y = 0f;
        }
        pos.x = Mathf.Abs(pos.x);
        pos.y = Mathf.Abs(pos.y);
        pos.z = Mathf.Abs(pos.z);

        float xDist = pos.x;
        float yDist = pos.y;
        float zDist = pos.z;
        float xPerc = xDist / vSize.x;
        float yPerc = yDist / vSize.y;
        float zPerc = zDist / vSize.z;
        xPerc = Mathf.Clamp(xPerc, 0f, 1f);
        yPerc = Mathf.Clamp(yPerc, 0f, 1f);
        zPerc = Mathf.Clamp(zPerc, 0f, 1f);


        Vector3 vStr = Vector3.one;
        vStr.y = 1f-yPerc;

        if(xPerc >= (1f - fEdgeSize)) {
            vStr.x = Mathf.Lerp(1f, 0f, (xPerc - (1f - fEdgeSize))/fEdgeSize);
        } else {
            vStr.x = 1f;
        }
        
        if (zPerc >= (1f - fEdgeSize)) {
            vStr.z = Mathf.Lerp(1f, 0f, (zPerc - (1f - fEdgeSize)) / fEdgeSize);
        } else {
            vStr.z = 1f;
        }


        strength = Mathf.Lerp(0f, 1f, vStr.x * vStr.y * vStr.z);
       // Debug.Log("vstr " + vStr + " pos " + pos + " strength " + strength);

        return strength;
    }

    void OnTriggerStay(Collider col) {
        if(col.tag == "Player") {

            Vector3 vPointDir = this.transform.position - col.transform.position;
            float fXDir = vPointDir.x;
            float fYDir = vPointDir.y;
            float fZDir = vPointDir.z;
            float dist = fYDir;
            if(dist < 0f) {
                dist = 0f;
            }

            float tmpForce = GetStrengthAtPos(col.transform.position - this.transform.position); //Mathf.Lerp(0f, 1f, (dist / vSize.y));

            col.attachedRigidbody.GetComponent<Player>().SetWind(vDir * tmpForce);
        } 
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            col.attachedRigidbody.GetComponent<Player>().SetWind(Vector3.zero);
        }
    }
}
