using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CalibrateBody : MonoBehaviour
{
    public GameObject lFootTracker;
    public GameObject lFoot;
    public GameObject hipBackTracker;
    public GameObject hipBack;
    public GameObject controller;

    private bool lFootCalibrated;
    private bool rFootCalibrated;
    private bool hipBackCalibrated;
    // Start is called before the first frame update
    void Start()
    {
        lFootTracker = gameObject.transform.GetChild(0).gameObject;
        lFoot = lFootTracker.transform.GetChild(0).gameObject;


        hipBackTracker = gameObject.transform.GetChild(2).gameObject;
        hipBack = hipBackTracker.transform.GetChild(0).gameObject;
        controller = gameObject.transform.GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            calibrate();
        }
    }


    void calibrate()
    {
        Vector3 posTracker = lFootTracker.transform.position;
        Quaternion oriTracker = controller.transform.rotation;
        Quaternion orihip = hipBack.transform.rotation;
        Debug.Log(orihip);
        /*Debug.Log(posTracker);
        Vector3 posController = controller.transform.position;
        Debug.Log(posController);
        Vector3 delta = posController - posTracker;
        Debug.Log(delta);
        
        
        lFoot.transform.localPosition = delta;
        */
        //lFoot.transform.localRotation;
        Debug.Log("we made it!");
    }
}
