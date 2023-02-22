using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VRfreePluginUnity;

public class HandleOnHammerScript : MonoBehaviour
{
    public float thresholdAngle = 7.5f;
    // Start is called before the first frame update
    public GameObject hammerheadCollider;
    public GameObject newHammer;
    public GameObject MTMobj;
    GameObject handle;
    GameObject hammer;
    GameObject hammerHead;
    public bool triggerEnabled = true;
    private MovablesCollisionHandler handleHandler;
    private float grabcoefficient;

    void Start()
    {
        handle = gameObject.transform.parent.gameObject;
        hammer = handle.transform.parent.gameObject;
        hammerHead = hammerheadCollider.transform.parent.gameObject;
        handleHandler = handle.GetComponent<MovablesCollisionHandler>();
        grabcoefficient = handleHandler.EndGrabCoeff;
        triggerEnabled = true;
    }
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!triggerEnabled){return;}
        if (!other.CompareTag(hammerheadCollider.tag)){return;}
        Debug.Log("hammer on handle");
        SwapHammers();
    }
    void OnTriggerExit(Collider other)
    {
        if (triggerEnabled){return;}
        if (!other.CompareTag(hammerheadCollider.tag)){return;}
        handleHandler.EndGrabCoeff = grabcoefficient;
        triggerEnabled = true;
    }

    void SwapHammers()
    {
        triggerEnabled = false;
        float dxAngle = handle.transform.localRotation.eulerAngles.x - hammerHead.transform.localRotation.eulerAngles.x;
        float dyAngle = handle.transform.localRotation.eulerAngles.y - hammerHead.transform.localRotation.eulerAngles.y;
        
        if (dxAngle > thresholdAngle || dyAngle >thresholdAngle){return;}
        //supress the next hand moton
        if (MTMobj.GetComponent<TranscriptionMaster>().transcribtionOn)
        {
            if (MTMobj.GetComponent<TranscriptionMaster>().transcribeHands)
            {
                MTMobj.GetComponent<ObjectInteractions>().supressNextHandMotion = true;
            }
        }

        //newHammer.transform.GetChild(0).transform.GetChild(0).GetComponent<InteractableObject>().gotPositioned = true;
        
        //Debug.Log("Hammerhead correctly detected");
        Vector3 enclavePos = new Vector3(0, -0.5f, 0);//newHammer.transform.position;
        Vector3 newPositionHammerHead = hammer.transform.GetChild(0).transform.position;
        Quaternion newRotHammerHead = hammer.transform.GetChild(0).transform.rotation;

        //handle.GetComponent<MovablesCollisionHandler>().enabled = false;
        //hammerHead.GetComponent<MovablesCollisionHandler>().enabled = false;
        handleHandler.EndGrabCoeff = 1000;
        handleHandler.isGrabbed = false;
        handle.transform.localPosition = new Vector3(0, -100, 0);
        hammer.transform.position = enclavePos;
        //handle.GetComponent<MovablesCollisionHandler>().enabled = true;

        //hammer.SetActive(false);
        
        
        newHammer.transform.GetChild(0).transform.position = newPositionHammerHead;
        newHammer.transform.GetChild(0).transform.rotation = newRotHammerHead;

        //newHammer.SetActive(true);
        newHammer.transform.GetChild(0).transform.GetChild(0).GetComponent<InteractableObject>().AddPositioning(1,1);
        //handleHandler.EndGrabCoeff = grabcoefficient;
        //triggerEnabled = true;
    }
    
    
}
