using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPoint : MonoBehaviour {
    // Drag Force = 1 / 2 p v^2 C A

    // public
    public GameObject gRgd;
    public Rigidbody rgdBdy;
    public float fArea = 10f;
    public float fCoeff = 1f;
    public float fDragForce = 0f;
    public float fMaxForce = 20f;
    public LayerMask mskWater;
    public bool bDoubleSided = false;
    public bool bWater = true;
    public bool bAir = false;
    public bool bPrintVel = false;
    public bool bSetPlayer = true;

    // private
    public Vector3 localvelllll;
    public Vector3 vVel = Vector3.zero;
    public Vector3 vPrevPos = Vector3.zero;
    public Vector3 vWind = Vector3.zero;
    public float fCurrentAngle = 0f;
    private float fMaxDrag = 10000f;
    private Player player;
    private bool bSkipNext = false;

    public object UtilityScript {
        get;
        private set;
    }

    // Use this for initialization
    void Awake() {
        rgdBdy = gRgd.GetComponent<Rigidbody>();
        vPrevPos = this.transform.position;
        if (bSetPlayer) {
            player = rgdBdy.GetComponent<Player>();
            player.AddDrag(this);
        }
    }

    // Update is called once per frame
    void Update() {
        float time = 0;
        time = Time.deltaTime;
        if (bPrintVel) {
            Debug.Log(rgdBdy.velocity + " | " + vVel);
        }
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3.up * fDragForce) / 10000f, Color.red);
        Debug.DrawLine(this.transform.position, this.transform.position + (vVel), Color.white);
        Debug.DrawLine(this.transform.position, this.transform.position + (this.transform.forward * 50f), Color.green);
    }

    public void SkipNext() {
        bSkipNext = true;
    }

    void OnEnable() {
        vVel = Vector3.zero;
        vPrevPos = this.transform.position;
    }

    void FixedUpdate() {
        float time = 0;
        time = Time.fixedDeltaTime;
        CalcLocalVel(time);
        CalcDragForce(time);
        if(!bSkipNext){
            ApplyForce(time);
        } else {
            bSkipNext = false;
        }
    }

    private void ApplyForce(float t) {
        rgdBdy.AddForceAtPosition(fDragForce * -this.transform.forward, this.transform.position, ForceMode.Impulse);
    }

    private void CalcLocalVel(float t) {
        vVel = (this.transform.position - vPrevPos);
        vPrevPos = this.transform.position;
        //  vVel *= t * 60f;
        if (vVel.magnitude > 2000) {
            vVel = Vector3.zero;
        }
        vVel /= t;
        vVel -= vWind;
       // vWind = Vector3.zero;
    }

    public void SetWind(Vector3 wind) {
        vWind = wind;
    }

    public void CalcDragForce(float t) {
        //Vector3 OceanDir = Ocean.ocean.GetOceanDirection();
        // OceanDir.z = OceanDir.y;
        //OceanDir.y = 0f;
        // fCurrentAngle = UtilityScript.SignedAngleBetween(this.transform.forward, OceanDir, Vector3.up);// Vector3.Angle(this.transform.forward, Ocean.ocean.GetOceanDirection());

        Vector3 localvel = this.transform.InverseTransformDirection(vVel);
        // if((fCurrentAngle > -90f) && (fCurrentAngle < 90f)) {
        //      localvel += this.transform.InverseTransformDirection(OceanDir);
        // }


        localvelllll = localvel;
        float p = 0f;
        // float height = 0f;
        // height = Ocean.ocean.GetHeightAt(new Vector2(this.transform.position.x, this.transform.position.z));
        //if((Physics.Raycast(this.transform.position, Vector3.up, 100f, mskWater)) && bWater) {
        //  if ((this.transform.position.y < height) && bWater) {
        //    p = 1027f;
        //} else if (bAir){
        p = 1.293f;
        // } else {
        ////     p = 0f;
        // }
        if ((localvel.z > 0f) || bDoubleSided) {
            fDragForce = (p / 2f) * (Mathf.Pow(localvel.z, 2f)) * fCoeff * fArea;
            if (fDragForce < 0) {
                fDragForce = 0;// -fDragForce;
            }
        } else {
            fDragForce = 0f;
        }
        if(fDragForce > fMaxForce) {
            fDragForce = fMaxForce;
        }
    }
}
