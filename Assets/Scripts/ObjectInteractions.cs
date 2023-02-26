using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VRfreePluginUnity;

public class ObjectInteractions : MonoBehaviour
{
    public List<Interaction> InteractionList = new List<Interaction>();
    public bool transcribtionOn;

    public bool supressNextHandMotion = false;
    public bool suppressGroupedObjectMotions = false;
    public float timeGroupObjSupression = 1f;
    private float groupedSuppressionTimeStamp;
    //public GameObject transcriptionDisplay;
    private Dictionary<GameObject, bool> test;
    private TranscriptionMaster transcriptionMaster;
    private GameObject groupSuppressionObj;
    StreamWriter csvWriter;
    private List<GameObject> recordedObjects;

    // Start is called before the first frame update
    void Start()
    {
        transcribtionOn = false;
        transcriptionMaster = gameObject.GetComponent<TranscriptionMaster>();
        groupedSuppressionTimeStamp = Time.realtimeSinceStartup;
    }
    
    
    public void addGraspedObject(GameObject graspedObj,bool isRightHand)
    {
        //update InteractableObject
        InteractableObject intObj = graspedObj.GetComponent<InteractableObject>();
        if (intObj is null){return;}
        intObj.UpdateValues(isRightHand, true);
        
        //Debug.Log("teoretical grasp: "+graspedObj.name);
        if (graspedObj.GetComponent<InteractableObject>().isInGroup)
        {
            if (suppressGroupedObjectMotions) {return;}
            suppressGroupedObjectMotions = true;
            groupedSuppressionTimeStamp = Time.realtimeSinceStartup;
            groupSuppressionObj = graspedObj;
        }
        
        //TranscribeMTM
        if (!transcribtionOn)
        {
            return;
        }

        if (supressNextHandMotion)
        {
            //Debug.Log("g suppressed: "+ graspedObj.name);
            return;
        }
        graspedObj.GetComponent<InteractableObject>().RemoveDisengaging();
        int frame = transcriptionMaster.RecorderObject.GetComponent<RecorderMaster>().frame;
        StartCoroutine(transcriptionMaster.CalculateGraspTransition(isRightHand,graspedObj,frame));
        AddInteractionToList(frame,isRightHand,graspedObj,true);
    }
    
    public void removeGraspedObj(GameObject releasedObj,bool isRightHand)
    {
        // update interactableObject
       // Debug.Log("teoretical release: "+releasedObj.name);
        InteractableObject intObj = releasedObj.GetComponent<InteractableObject>();
        if (intObj is null){return;}
        intObj.UpdateValues(isRightHand, false);
        
        if (suppressGroupedObjectMotions)
        {
            if (releasedObj.name == groupSuppressionObj.name) { suppressGroupedObjectMotions = false;}
            else if (releasedObj.GetComponent<InteractableObject>().isInGroup){return;}
        }
        
        //TranscribeMTM
        if (supressNextHandMotion)
        {
            //Debug.Log("rl suppressed" + releasedObj.name);
            supressNextHandMotion = false;
            return;
        }
        if (!transcribtionOn){return;}
        int frame = transcriptionMaster.RecorderObject.GetComponent<RecorderMaster>().frame;
        StartCoroutine(transcriptionMaster.CalculateReleaseTransition(isRightHand, releasedObj, frame));
        AddInteractionToList(frame,isRightHand,releasedObj,false);
    }

    public void replayInteractionFrame(int frameIn)
    {
        foreach (var inter in InteractionList)
        {
            if (inter.frame == frameIn)
            {
                if (inter.isGrasp)
                {
                    addGraspedObjectReplay(inter.interactedObj,inter.isRightHand,inter.frame);
                }
                else
                {
                    removeGraspedObjReplay(inter.interactedObj,inter.isRightHand,inter.frame);
                }
            }
        }
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
        if (suppressGroupedObjectMotions)
        {
            if (timeGroupObjSupression + groupedSuppressionTimeStamp < Time.realtimeSinceStartup)
            {
                suppressGroupedObjectMotions = false;
            }
        }
    }
    
    public void turnTranscriptionOn()
    {
        InteractionList.Clear();
        transcribtionOn = true;
        suppressGroupedObjectMotions = false;
        supressNextHandMotion = false;
    }
    public void turnTranscriptionOff(string sequenceDir,List<GameObject> objList)
    {
        transcribtionOn = false;
        WriteInteractionCSV(sequenceDir,objList);
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
        string newPath = CreateUniqueFilePath(folderDirectory, "Interactions", ".csv");
        csvWriter = new StreamWriter(newPath);
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
                if (recordedObjects[j].name.Equals(tempObj.name,StringComparison.Ordinal))
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
    string CreateUniqueFilePath(string pathIn, string nameIn, string filetypeIn)
    {
        string fullpath = pathIn + "/" + nameIn + filetypeIn;
        DirectoryInfo tempdirASDF = new DirectoryInfo(fullpath);
        //FileInfo[] info = tempdirASDF.GetFiles(filetypeIn);
        FileInfo file = new FileInfo(fullpath);
        bool alreadyExists = file.Exists;
        if ( alreadyExists)
        {
            fullpath = CreateUniqueFilePath(pathIn, (nameIn + "I"), filetypeIn);
        }
        return fullpath;
    }

}
