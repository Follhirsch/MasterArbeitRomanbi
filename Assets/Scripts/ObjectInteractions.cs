using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRfreePluginUnity;

public class ObjectInteractions : MonoBehaviour
{
    public List<Objectinteraction> InteractionList = new List<Objectinteraction>();
    public GameObject debugObj;
    public GameObject debugObj2;
    //public GameObject transcriptionDisplay;
    private Dictionary<GameObject, bool> test;
    private TranscriptionMaster transcriptionMaster;
    
    // Start is called before the first frame update
    void Start()
    {
        transcriptionMaster = gameObject.GetComponent<TranscriptionMaster>();
    }
    
    public void addGraspedObject(GameObject graspedObj,bool isRightHand)
    {
        //update InteractableObject
        graspedObj.GetComponent<InteractableObject>().UpdateValues(isRightHand,true);

        //TranscribeMTM
        
        int frame = transcriptionMaster.RecorderObject.GetComponent<RecorderMaster>().frame;
        StartCoroutine(transcriptionMaster.CalculateGraspTransition(isRightHand,graspedObj,frame));
        addItemToList(isRightHand,graspedObj,frame,true);
    }
    
    public void removeGraspedObj(GameObject releasedObj,bool isRightHand)
    {
        // update interactableObject
        releasedObj.GetComponent<InteractableObject>().UpdateValues(isRightHand,false);
        
        
        
        //TranscribeMTM
        
        int frame = transcriptionMaster.RecorderObject.GetComponent<RecorderMaster>().frame;
        StartCoroutine(transcriptionMaster.CalculateReleaseTransition(isRightHand, releasedObj, frame));
        addItemToList(isRightHand,releasedObj,frame,false);
    }
    
    public void addGraspedObjectReplay(GameObject graspedObj,bool isRightHand,int frame)
    {
        StartCoroutine(transcriptionMaster.CalculateGraspTransition(isRightHand,graspedObj,frame));
    }
    
    public void removeGraspedObjReplay(GameObject graspedObj,bool isRightHand, int frame)
    {
        StartCoroutine(transcriptionMaster.CalculateReleaseTransition(isRightHand, graspedObj, frame));
    }
    
    
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            addGraspedObject(debugObj,true);
        }
        if (Input.GetKeyDown("z"))
        {
            addGraspedObject(debugObj2,true);
            //removeGraspedObj(debugObj,true);
        }
        
        
        /* add later for contact grasp

        foreach(GameObject tempObj in currentlyGraspedRH.Keys)
        {
            if (currentlyGraspedRH[tempObj])
            {
                if (tempObj.transform.TryGetComponent<MovablesCollisionHandler>(
                        out MovablesCollisionHandler tempHandler))
                {
                    currentlyGraspedRH[tempObj] = tempHandler.isGrabbed;
                }
                
            }
        }
        */
    }
    
    void addItemToList(bool isRightHandIn, GameObject objIn,int frameIn,bool isGrabingInteractionIn)
    {
        Objectinteraction newInteraction = new Objectinteraction();
        newInteraction.interactedObj = objIn;
        newInteraction.frame = frameIn;
        newInteraction.isRightHand = isRightHandIn;
        newInteraction.isGrabingInteraction = isGrabingInteractionIn;
        
        InteractionList.Add(newInteraction);
    }
    
    public struct Objectinteraction
    {
        public GameObject interactedObj;
        public int frame;
        public bool isGrabingInteraction;
        public bool isRightHand;
    }
    

}
