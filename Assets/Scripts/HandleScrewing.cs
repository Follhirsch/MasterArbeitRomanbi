using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRfreePluginUnity;

public class HandleScrewing : MonoBehaviour
{
    private float angle;
    private float maxAngle;
    public float screwingAngle = 180f;
    private float minAngle = 0;
    public float screwdistance;
    public GameObject newHammer;
    GameObject hammerHead;

    private bool triggerOn = true;
    // Start is called before the first frame update
    void Start()
    {
        angle = 0f;
        maxAngle = gameObject.GetComponent<KnobCollisionHandler>().upperBound;
        screwdistance = transform.localPosition.z;
        hammerHead = gameObject.transform.parent.transform.gameObject;
        triggerOn = true;
        KnobCollisionHandler knobHandler = gameObject.GetComponent<KnobCollisionHandler>();
    }
    private void OnEnable()
    {
        KnobCollisionHandler knobHandler = gameObject.GetComponent<KnobCollisionHandler>();
        knobHandler.upperBound = knobHandler.movedDistance + screwingAngle;
        knobHandler.lowerBound = knobHandler.movedDistance;
        maxAngle = knobHandler.upperBound;
        minAngle = knobHandler.lowerBound;
        screwingAngle = 180f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            SwapHammers();
        }
        if (Input.GetKeyDown("h"))
        {
            Debug.Log(hammerHead.transform.position);
        }
        /*if (Input.GetKeyDown("space"))
        {
            gameObject.GetComponent<KnobCollisionHandler>().totalMoveDist = 0f;
        }*/        
        KnobCollisionHandler knobHandler = gameObject.GetComponent<KnobCollisionHandler>();
        
        
        angle = knobHandler.totalMoveDist-minAngle;
        Vector3 localpositionvector = transform.localPosition;
        if (angle >= screwingAngle)
        {
            knobHandler.upperBound = maxAngle+1;
            screwingAngle = 200;
            SwapHammers();
        }
        localpositionvector.z = screwdistance*(1-angle/(screwingAngle));
        transform.localPosition = localpositionvector;
    }

   

    void SwapHammers()
    {
        TranscriptionMaster MTMmaster = FindObjectOfType<TranscriptionMaster>();
        RecorderMaster recMaster = FindObjectOfType<RecorderMaster>();
        MTMmaster.MTMTranscription.Add(new ApplyPressure(true,recMaster.frame));
        if (MTMmaster.transcribtionOn)
        {
            if (MTMmaster.transcribeHands)
            {
                FindObjectOfType<ObjectInteractions>().supressNextHandMotion = true;
            }
        }

        Vector3 enclavePos = new Vector3(0.2f,-0.5f,0);
        
        Vector3 newPositionHammer = hammerHead.transform.position;
        Quaternion newRotHammer = hammerHead.transform.rotation;
        
        hammerHead.transform.position = enclavePos;
        hammerHead.transform.GetChild(0).transform.position = enclavePos;
        
        newHammer.transform.rotation = newRotHammer;
        newHammer.transform.position = newPositionHammer;
    }
}
