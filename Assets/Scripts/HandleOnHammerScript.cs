using System;
using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        handle = gameObject.transform.parent.gameObject;
        hammer = handle.transform.parent.gameObject;
        hammerHead = hammerheadCollider.transform.parent.gameObject;
    }
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(hammerheadCollider.tag)){return;}

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

        newHammer.transform.GetChild(0).transform.GetChild(0).GetComponent<InteractableObject>().gotPositioned = true;
        
        //Debug.Log("Hammerhead correctly detected");
        Vector3 enclavePos = new Vector3(0, -0.5f, 0);//newHammer.transform.position;
        Vector3 newPositionHammerHead = hammer.transform.GetChild(0).transform.position;
        Quaternion newRotHammerHead = hammer.transform.GetChild(0).transform.rotation;

        handle.GetComponent<MovablesCollisionHandler>().enabled = false;
        hammerHead.GetComponent<MovablesCollisionHandler>().enabled = false;
        
        hammer.SetActive(false);
        
        
        hammer.transform.position = enclavePos;
        
        newHammer.transform.GetChild(0).transform.position = newPositionHammerHead;
        newHammer.transform.GetChild(0).transform.rotation = newRotHammerHead;

        newHammer.SetActive(true);
        newHammer.GetComponent<InteractableObject>().AddPositioning(1,1);
    }
    
    
}
