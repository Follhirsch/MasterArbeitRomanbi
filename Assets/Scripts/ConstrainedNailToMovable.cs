using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRfreePluginUnity;

public class ConstrainedNailToMovable : MonoBehaviour
{
    public GameObject inputNail;
    public GameObject fixedNail;
    private Vector3 startPos;
    public GameObject MTMobj;

    public ConstrainedMovable fixedNailMovableHandler;
    // Start is called before the first frame update
    void Start()
    {
        fixedNail = gameObject;
        fixedNailMovableHandler = fixedNail.GetComponent<ConstrainedMovable>();
        startPos = fixedNail.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (fixedNailMovableHandler.movedDistance >= fixedNailMovableHandler.upperBound)
        {
            if (MTMobj.GetComponent<TranscriptionMaster>().transcribtionOn)
            {
                if (MTMobj.GetComponent<TranscriptionMaster>().transcribeHands)
                {
                    MTMobj.GetComponent<ObjectInteractions>().supressNextHandMotion = true;
                }
            }//supress next transcription
            
            Vector3 pos = fixedNail.transform.position;
            Quaternion ori = fixedNail.transform.rotation;
            fixedNail.transform.position = startPos;
            fixedNail.GetComponent<ConstrainedMovable>().movedDistance = 0;
            fixedNail.GetComponent<InteractableObject>().gotPositioned = true;
            fixedNail.SetActive(false);
            
            inputNail.SetActive(true);
            inputNail.transform.position = pos;
            inputNail.transform.rotation = ori;
            inputNail.transform.GetChild(2).GetComponent<NailConstrain>().exitTime = Time.realtimeSinceStartup;
            inputNail.GetComponent<InteractableObject>().gotDisengaged = true;
            inputNail.GetComponent<InteractableObject>().disengagingforce = 2;
            //Debug.Log(inputNail.GetComponent<NailConstrain>().exitTime);
        }
        
    }
    
}
