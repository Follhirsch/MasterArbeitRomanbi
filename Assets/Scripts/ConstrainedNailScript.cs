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
    private float exitTime = 0;
    private ConstrainedMovablesCollisionHandler nailHandler;
    private float endGrabcoefficient;
    private float startGrabcoefficient;
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
        endGrabcoefficient = nailHandler.EndGrabCoeff;
        startGrabcoefficient = nailHandler.StartGrabCoeff;
    }

    

    // Update is called once per frame
    void Update()
    {
        if (!triggerEnabled)
        {
            if (Time.realtimeSinceStartup > exitTime + 10)
            {
                fixedNailMovableHandler.movedDistance = 0;
                triggerEnabled = true;
                nailHandler.EndGrabCoeff = endGrabcoefficient;
                nailHandler.StartGrabCoeff = startGrabcoefficient;
            }
            return;
        }
        if (fixedNailMovableHandler.movedDistance >= fixedNailMovableHandler.upperBound)
        {
            exitTime = Time.realtimeSinceStartup;
            Debug.Log(exitTime);
            triggerEnabled = false;
            //fixedNailMovableHandler.upperBound = fixedNailMovableHandler.upperBound+1f;
            SwapNails();
            
        }
        if (Input.GetKeyDown("space")) //replay everything
        {
            fixedNailMovableHandler.movedDistance = 0;
            fixedNailMovableHandler.upperBound = 0.1f;
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
        fixedNailMovableHandler.movedDistance = 0;
        fixedNail.GetComponent<InteractableObject>().gotPositioned = true;
        
        nailHandler.EndGrabCoeff = 10000f;
        nailHandler.StartGrabCoeff = 10000f;
        //nailHandler.isGrabbed = false;

        MoveToEnclave();
        
        inputNail.transform.GetChild(2).GetComponent<NailConstrain>().exitTime = Time.realtimeSinceStartup+1;
        inputNail.transform.position = pos;
        inputNail.transform.rotation = ori;
        inputNail.GetComponent<InteractableObject>().gotDisengaged = true;
        inputNail.GetComponent<InteractableObject>().disengagingforce = 2;
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
