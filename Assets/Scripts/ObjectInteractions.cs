using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VRfreePluginUnity;

public class ObjectInteractions : MonoBehaviour
{
    public List<Interaction> InteractionList = new List<Interaction>();

    public bool supressNextHandMotion = false;
    public bool suppressGroupedObjectMotions;
    //public GameObject transcriptionDisplay;
    private Dictionary<GameObject, bool> test;
    private TranscriptionMaster transcriptionMaster;
    private GameObject groupSuppressionObj;
    StreamWriter csvWriter;
    private List<GameObject> recordedObjects;

    // Start is called before the first frame update
    void Start()
    {
        transcriptionMaster = gameObject.GetComponent<TranscriptionMaster>();
        
    }
    
    public void addGraspedObject(GameObject graspedObj,bool isRightHand)
    {
        
        //update InteractableObject
        graspedObj.GetComponent<InteractableObject>().UpdateValues(isRightHand,true);
        
        if (graspedObj.GetComponent<InteractableObject>().isInGroup)
        {
            if (suppressGroupedObjectMotions) {return;}
            suppressGroupedObjectMotions = true;
            groupSuppressionObj = graspedObj;
        }
        
        //TranscribeMTM
        if (supressNextHandMotion){return;}
        graspedObj.GetComponent<InteractableObject>().RemoveDisengaging();
        int frame = transcriptionMaster.RecorderObject.GetComponent<RecorderMaster>().frame;
        StartCoroutine(transcriptionMaster.CalculateGraspTransition(isRightHand,graspedObj,frame));
        AddInteractionToList(frame,isRightHand,graspedObj,true);
    }
    
    public void removeGraspedObj(GameObject releasedObj,bool isRightHand)
    {
        
        // update interactableObject
        releasedObj.GetComponent<InteractableObject>().UpdateValues(isRightHand,false);
        
        if (suppressGroupedObjectMotions)
        {
            if (releasedObj.name == groupSuppressionObj.name) { suppressGroupedObjectMotions = false;}
            else if (releasedObj.GetComponent<InteractableObject>().isInGroup){return;}
        }
        
        
        
        //TranscribeMTM
        if (supressNextHandMotion)
        {
            supressNextHandMotion = false;
            return;
        }
        
        int frame = transcriptionMaster.RecorderObject.GetComponent<RecorderMaster>().frame;
        StartCoroutine(transcriptionMaster.CalculateReleaseTransition(isRightHand, releasedObj, frame));
        AddInteractionToList(frame,isRightHand,releasedObj,false);
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
        if (Input.GetKeyDown("x"))
        {
            AddInteractionToList(2, true,GameObject.Find("CrankHandle"), true);
            Debug.Log(InteractionList.Count);
        }
        if (Input.GetKeyDown("y")) 
        {
            for (int i = 0; i < InteractionList.Count; i++)
            {
                Debug.Log(InteractionList[i].frame);
            }
            
        }
    }
    
    void AddInteractionToList(int frameIn,bool isRightHandIn, GameObject objIn,bool IsGraspIn)
    {
        Interaction newInteraction = new Interaction();
        newInteraction.interactedObj = objIn;
        newInteraction.frame = frameIn;
        newInteraction.isRightHand = isRightHandIn;
        newInteraction.isGrasp = IsGraspIn;
        
        InteractionList.Add(newInteraction);
    }

    public void WriteInteractionCSV(string folderDirectory,List<GameObject> objListIn)
    { 
        recordedObjects = objListIn;
        
        csvWriter = new StreamWriter(folderDirectory + "/" + "Interactions" + ".csv");
        string header = "Frame,IsRightHand,isGRab,objNr";
        csvWriter.WriteLine(header);
        for (int i = 0; i < InteractionList.Count; i++)
        {
            string completeline = "";
            completeline += InteractionList[i].frame.ToString()+",";
            completeline += InteractionList[i].isRightHand.ToString() +",";
            completeline += InteractionList[i].isGrasp.ToString()+",";
            
            string nrString = "";
            GameObject tempObj = InteractionList[i].interactedObj;
            for (int j = 0; j < recordedObjects.Count; j++)
            {
                if (recordedObjects[j].name == tempObj.name)
                {
                    nrString = j.ToString();
                    break;
                }
            }
            completeline += nrString;
            
            csvWriter.WriteLine(completeline);
        }
        csvWriter.Close();
    }

    public bool ReadInteractionsCSV(string pathIn,List<GameObject> objListIn)
    {
        recordedObjects = objListIn;
        //"Frame,IsRightHand,isGRab,objNr"
        InteractionList = new List<Interaction>();
        string dir = pathIn + "/Interactions";
        
        TextAsset replayFile = Resources.Load<TextAsset>(dir);
        if (replayFile == null) { return false;}

        string[] dataLines = replayFile.text.Split("\n");
        if (dataLines.Length < 2)
        {
            Debug.Log("No Interactions found");
            return true;
        }
        for (int i = 1; i <  dataLines.Length; i++)
        {
            string[] dataValues = dataLines[i].Split(",");
            if (dataValues.Length == 4)
            {
                int objNr = int.Parse(dataValues[3]);
                AddInteractionToList(int.Parse(dataValues[0]), bool.Parse(dataValues[1]), recordedObjects[objNr],
                    bool.Parse(dataValues[2]));
            }
        }
        return true;
    }
    
    public struct Interaction
    {
        public GameObject interactedObj;
        public int frame;
        public bool isRightHand;
        public bool isGrasp;
    }
    

}
