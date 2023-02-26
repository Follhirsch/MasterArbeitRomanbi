using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRfreePluginUnity;

public class NailConstrain : MonoBehaviour
{
    public GameObject TargetCollider;
    public GameObject fixedNail;
    public GameObject MTMobj;
    public GameObject Nail;
    public float reenterTimeDelay = 1f;
    public float exitTime;
    public static bool triggerEnabled = true;
    private MovablesCollisionHandler nailHandler;
    private float grabcoefficient;
    // Start is called before the first frame update
    void Start()
    {
        Nail = gameObject.transform.parent.gameObject;
        TargetCollider = GameObject.Find("TriggerForNail");
        nailHandler = Nail.GetComponent<MovablesCollisionHandler>();
        grabcoefficient = nailHandler.EndGrabCoeff;
        exitTime = Time.realtimeSinceStartup;
        triggerEnabled = true;
    }

    /*void OnEnable()
    {
        exitTime = Time.realtimeSinceStartup;
    }*/

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) //replay everything
        {
            exitTime = Time.realtimeSinceStartup-reenterTimeDelay;
            triggerEnabled = true;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(!triggerEnabled){return;}
        if (exitTime+reenterTimeDelay>Time.realtimeSinceStartup){return;}
        if (!other.CompareTag(TargetCollider.tag)){return;}
        swapNails();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(TargetCollider.tag)){return;}
        nailHandler.EndGrabCoeff = grabcoefficient;
        triggerEnabled = true;
    }

    void swapNails()
    {
        triggerEnabled = false;
        //suppress the next hand moton
        TranscriptionMaster MTMmaster = FindObjectOfType<TranscriptionMaster>();
        if (MTMmaster.transcribtionOn)
        {
            if (MTMmaster.transcribeHands)
            {
                FindObjectOfType<ObjectInteractions>().supressNextHandMotion = true;
            }
        }
        
        //fixedNail.GetComponent<InteractableObject>().gotPositioned = true;
        nailHandler.EndGrabCoeff = 1000f;
        nailHandler.isGrabbed = false;
        Vector3 enclavePos = new Vector3(1, -0.5f, 0);

        
        //GameObject triggerNailObj = Nail.transform.GetChild(2).transform.gameObject;
        //triggerNailObj.SetActive(false);
        exitTime = exitTime * 1000;
        Nail.transform.position = enclavePos;

        ConstrainedNailScript fixNailScript = fixedNail.GetComponent<ConstrainedNailScript>();
        fixNailScript.MoveToHole();
        //fixNailScript.inputNail = Nail;
    }
}
