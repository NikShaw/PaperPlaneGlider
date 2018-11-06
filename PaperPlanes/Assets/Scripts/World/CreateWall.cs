using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CreateWall : MonoBehaviour {
    public List<GameObject> gObject = new List<GameObject>();
    public Vector3 vWallSize;
    public Vector3 vObjSize;
    public bool bLeft = false;
    public bool bBackwardBound = false;
    public bool bBuild = false;
    public bool bRemove = false;
    public bool bCreateCollider = false;
    private bool bHasBuilt = false;
    private List<List<List<GameObject>>> bObjects = new List<List<List<GameObject>>>();
    private Vector3 vBuiltWallSize;
    private Vector3 vBuiltObjSize;
    private BoxCollider boxCollider;
    public string sHotkey = "-";

    void Start() {
        //vObjSize = new Vector2(gObject.GetComponent<BoxCollider>().bounds.size.x, gObject.GetComponent<BoxCollider>().bounds.size.y);
        bBuild = false;
        bRemove = false;
        if (Application.isPlaying) {
           // Remove();
           // Build();
        }
    }

    private void Remove() {
        /*for (int o = 0; o < bObjects.Count; o++) {
            for (int o2 = 0; o2 < bObjects[o].Count; o2++) {
                for (int o3 = 0; o3 < bObjects[o][o2].Count; o3++) {
                    GameObject tmpObj = bObjects[o][o2][o3];
                    DestroyImmediate(tmpObj);
                }
            }
        }*/
        for (int i = this.transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
        if (bCreateCollider) {
            DestroyImmediate(boxCollider);
        }
        bObjects.Clear();
    }

    private void Build() {
        for (int x = 0; x < vWallSize.x; x++) {
            bObjects.Add(new List<List<GameObject>>());
            for (int y = 0; y < vWallSize.y; y++) {
                bObjects[x].Add(new List<GameObject>());
                for (int z = 0; z < vWallSize.z; z++) {
                    GameObject tmpObj;// = Instantiate(gObject);
                   // if(x == 0) {
                        tmpObj = Instantiate(gObject[Random.Range(0, gObject.Count - 1)]);
                   /* } else {
                        Rock rock = bObjects[x - 1][y][z].GetComponent<Rock>();
                        tmpObj = Instantiate(gObject[rock.Connecting()[Random.Range(0, rock.Connecting().Count-1)]]);
                    }*/

                    tmpObj.transform.parent = this.transform;//.FindChild("Objects");
                    Vector3 offset = Vector3.zero;
                    if (bLeft) {
                        offset.x = (vObjSize.x * x);
                        offset.y = (vObjSize.y * y);
                        offset.z = (vObjSize.z * z);
                    } else {
                        offset.x = -(vObjSize.x * x);
                        offset.y = (vObjSize.y * y);
                        offset.z = (vObjSize.z * z);
                    }
                    tmpObj.transform.localPosition = offset;// new Vector3(this.transform.position.x + offset.x, this.transform.position.y + offset.y, this.transform.position.z + offset.z);
                    tmpObj.transform.localScale = Vector3.one;
                    //tmpObj.transform.RotateAround(this.transform.position, this.transform.up, -this.transform.rotation.eulerAngles.y);
                    tmpObj.transform.rotation = this.transform.rotation;
                    bObjects[x][y].Add(tmpObj);
                }
            }
        }
        if(bCreateCollider) {
            boxCollider = this.gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3((vObjSize.x * (vWallSize.x)), (vObjSize.y * (vWallSize.y)), vObjSize.z);
            if(bBackwardBound) {
                boxCollider.center = new Vector3((boxCollider.size.x / 2) - (vObjSize.x / 2f), (boxCollider.size.y / 2f) - (vObjSize.y / 2f), 0.0f);
            } else {
                boxCollider.center = new Vector3((-boxCollider.size.x/2) + (vObjSize.x/2f), (boxCollider.size.y / 2f) - (vObjSize.y / 2f), 0.0f);
            }
        }
        vBuiltWallSize = vWallSize;
        vBuiltObjSize = vObjSize;
    }

    void Update() {
       // if (!Application.isPlaying) {
       if(Input.GetKeyDown(sHotkey)) {
            Remove();
            Build();
        }

            if (bHasBuilt) {
                if(bRemove) {
                    bHasBuilt = false;
                    Remove();
                    bRemove = false;
                }
            } else {
                if(bBuild) {
                    Build();
                    bBuild = false;
                    bHasBuilt = true;
                }
            }
            if(bBuild) {
                if((vBuiltObjSize != vObjSize) || (vBuiltWallSize != vWallSize)) {
                    Remove();
                    Build();
                }
            }
       // }
    }
}
