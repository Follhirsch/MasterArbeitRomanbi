using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRfreePluginUnity;

public class ConstrainedNailScript : MonoBehaviour
{
    public GameObject inputNail;
    public GameObject fixedNail;
    private Vector3 startPos;
    private Quaternion startOri;
    public GameObject MTMobj;
    public Vector3 enclavepos;
    public bool triggerEnabled = true;
    private ConstrainedMovablesCollisionHandler nailHandler;
    private float grabcoefficient;
    private bool hanlerfound = false;

    public ConstrainedMovable fixedNailMovableHandler;
    // Start is called before the first frame update
    void Start()
    {
        hanlerfound = false;
        fixedNail = gameObject;
        fixedNailMovableHandler = fixedNail.GetComponent<ConstrainedMovable>();
        startPos = fixedNail.transform.position;
        startOri = fixedNail.transform.rotation;
        enclavepos = startPos;
        
        enclavepos.y += -100;

        fixedNail.transform.position = enclavepos;
        triggerEnabled = true;
        
        nailHandler = fixedNail.GetComponent<ConstrainedMovablesCollisionHandler>();
        grabcoefficient = nailHandler.EndGrabCoeff;
    }

    

    // Update is called once per frame
    void Update()
    {
        if(!triggerEnabled){return;}
        if (fixedNailMovableHandler.movedDistance >= fixedNailMovableHandler.upperBound)
        {
            //fixedNailMovableHandler.upperBound = fixedNailMovableHandler.upperBound+0.00001f;
            SwapNails();
        }
        
    }

    void SwapNails()
    {
        //supress next transcription
        TranscriptionMaster MTMmaster = FindObjectOfType<TranscriptionMaster>();
        if (MTMmaster.transcribtionOn)
        {
            if (MTMmaster.transcribeHands)
            {
                FindObjectOfType<ObjectInteractions>().supressNextHandMotion = true;
            }
        }

        Vector3 pos = fixedNail.transform.position;
        Quaternion ori = fixedNail.transform.rotation;
        fixedNail.GetComponent<ConstrainedMovable>().movedDistance = 0;
        fixedNail.GetComponent<InteractableObject>().gotPositioned = true;
        
        nailHandler.EndGrabCoeff = 1000f;
        nailHandler.isGrabbed = false;
        
        MoveToEnclave();
            
        inputNail.transform.position = pos;
        inputNail.transform.rotation = ori;
        inputNail.transform.GetChild(2).GetComponent<NailConstrain>().exitTime = Time.realtimeSinceStartup;
        inputNail.GetComponent<InteractableObject>().gotDisengaged = true;
        inputNail.GetComponent<InteractableObject>().disengagingforce = 2;
        nailHandler.EndGrabCoeff = grabcoefficient;
    }

    void MoveToEnclave()
    {
        fixedNail.transform.position = enclavepos;
    }

    public void MoveToHole()
    {
        fixedNail.transform.position = startPos;
        fixedNail.transform.rotation = startOri;
    }

}
