using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRfreePluginUnity;

public class testHandCollisionrecording : MonoBehaviour
{
    public GameObject hands;
    public GameObject rightHand;
    public GameObject leftHand;
    public Quaternion oldrot;
    public float angle;

    // Start is called before the first frame update
    void Start()
    {
        //rightHand = hands.transform.GetChild(0).GetChild(4).GetChild(1).gameObject;
        //leftHand = hands.transform.GetChild(1).GetChild(4).GetChild(1).gameObject;
        //oldrot = rightHand.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown("w"))
        {
            Quaternion newrot = rightHand.transform.rotation;
            Debug.Log(Vector3.Angle(oldrot * Vector3.up, newrot * Vector3.up));
            oldrot = newrot;
        }*/

    }
}
