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

    public ConstrainedMovable fixedNailMovableHandler;
    // Start is called before the first frame update
    void Start()
    {
        fixedNail = gameObject;
        fixedNailMovableHandler = fixedNail.GetComponent<ConstrainedMovable>();
        startPos = fixedNail.transform.position;
        startOri = fixedNail.transform.rotation;
        enclavepos = startPos;
        
        enclavepos.y += -100;

        fixedNail.transform.position = enclavepos;
    }

    // Update is called once per frame
    void Update()
    {
        if (fixedNailMovableHandler.movedDistance >= fixedNailMovableHandler.upperBound)
        {
            SwapNails();
        }
        
    }

    void SwapNails()
    {
        //supress next transcription
        if (MTMobj.GetComponent<TranscriptionMaster>().transcribtionOn)
        {
            if (MTMobj.GetComponent<TranscriptionMaster>().transcribeHands)
            {
                MTMobj.GetComponent<ObjectInteractions>().supressNextHandMotion = true;
            }
        }

        Vector3 pos = fixedNail.transform.position;
        Quaternion ori = fixedNail.transform.rotation;
        fixedNail.GetComponent<ConstrainedMovable>().movedDistance = 0;
        fixedNail.GetComponent<InteractableObject>().gotPositioned = true;
        MoveToEnclave();
            
        inputNail.transform.position = pos;
        inputNail.transform.rotation = ori;
        inputNail.transform.GetChild(2).GetComponent<NailConstrain>().exitTime = Time.realtimeSinceStartup;
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
