using System.Collections;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
//using RootMotion.Demos;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR;
using VRfree;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ObjectRecorder : MonoBehaviour
{
    StreamWriter csvWriter;
    public GameObject recorderObject;
    public GameObject MTMobj;
    public List<Vector3[]> posVectors = new List<Vector3[]>();
    public List<Quaternion[]> oriQuaternion = new List<Quaternion[]>();
    public GameObject scene;

    public GameObject recObj1;
    public GameObject recObj2;
    public GameObject recObj3;
    public GameObject recObj4;
    public GameObject recObj5;
    public GameObject recObj6;
    public GameObject recObj7;
    public GameObject recObj8;
    public GameObject recObj9;
    public GameObject recObj10;

    public string searchTag = "InteractableObject";
    [FormerlySerializedAs("ObjectsTorecord")] public List<GameObject> ObjectsToRecord = new List<GameObject>();

    //public List<GameObject> objects = new List<GameObject>();
    public int framerate;
    string FolderDirectory;
    
    //private int samples = 0;
    
    private int totalNrOfObjects;


    // Start is called before the first frame update
    void Start()
    {
        recorderObject = this.gameObject;
        framerate = recorderObject.GetComponent<RecorderMaster>().framerate;
        scene = gameObject.GetComponent<RecorderMaster>().SceneToRecord;
        locateObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (!recorderObject.GetComponent<RecorderMaster>().recordObjects) { return;}
        
        if (Input.GetKeyDown("l")) { locateObjects(); }

        /*if (Input.GetKeyDown("t"))
        { 
            Vector3 pos; 
            Quaternion ori;
            if (GetValuesFromObj(scene, out pos, out ori) > 0 ) { Debug.Log("obj has childs"); }
            else { Debug.Log("obj has NO childs"); }
            Debug.Log(pos);
        }*/
    }

    public void LogData()
    {
        string completeLine = "";
        totalNrOfObjects = ObjectsToRecord.Count;
        Vector3[] tempArray = new Vector3[totalNrOfObjects]; // dont forget to add children
        Quaternion[] tempOriArray = new Quaternion[totalNrOfObjects];

        for (int i = 0; i < totalNrOfObjects; i++)
        {
            string tempString = "";
            Vector3 pos = ObjectsToRecord[i].transform.position;
            Quaternion ori = ObjectsToRecord[i].transform.rotation;
            tempArray[i] = pos;
            tempOriArray[i] = ori;
            tempString += posOriString(pos, ori);
            tempString +=CreateInteractableString(ObjectsToRecord[i]);
            completeLine += tempString;
        }

        posVectors.Add(tempArray);
        oriQuaternion.Add(tempOriArray);
        
        completeLine =  completeLine.Substring(0, completeLine.Length - 1);//remove , from the end
        csvWriter.WriteLine(completeLine);
        //syntax csv: object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz,object1.rw,object1.Interactionvalues ...
    }

    public void StopRecording()
    {
        csvWriter.Close();
        csvWriter = new StreamWriter(FolderDirectory + "/" + "Interactions" + ".csv");
        List<ObjectInteractions.Objectinteraction> interactionsList = MTMobj.GetComponent<ObjectInteractions>().InteractionList;
        string header = "Frame,IsRightHand,isGRab,HirarchyLevel,objNR,childNr,GrandchildNr";
        for (int i = 0; i < interactionsList.Count; i++)
        {
            string completeline = "";
            completeline += interactionsList[i].frame.ToString()+",";
            completeline += Convert.ToInt32(interactionsList[i].isRightHand).ToString();
            completeline += Convert.ToInt32(interactionsList[i].isGrabingInteraction).ToString();
            string nrString = "";
            GameObject tempObj = interactionsList[i].interactedObj;
            for (int j = 0; j < 3; j++)
            {
                if ( tempObj.name.Equals(scene.name))
                {
                    nrString = (j-1).ToString() + "," + nrString;
                    break;
                }

                nrString = interactionsList[i].interactedObj.transform.GetSiblingIndex().ToString() + "," +
                           nrString;
                
                tempObj = tempObj.transform.parent.gameObject;
            }

            completeline += nrString;
            completeline =  completeline.Substring(0, completeline.Length - 1);
            csvWriter.WriteLine(completeline);
        }
        
    }

    public void StartRecording(string folderDir)
    {
        framerate = recorderObject.GetComponent<RecorderMaster>().framerate;
        FolderDirectory = folderDir;
        /* NOW PERFORMED IN RECORDERMASTER
        string dir = "Assets/Resources/Recordings/Recording" + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm");
        if(!Directory.Exists(dir))
        {
            FolderDirectory = Directory.CreateDirectory("Assets/Resources/Recordings/Recording"+ "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm"));
            dir = FolderDirectory.ToString();
        }
        */
            
        csvWriter = new StreamWriter(folderDir + "/" + "Objects" + ".csv");
        csvWriter.WriteLine("FPS,"+framerate.ToString()+"," + "NrOfObjects,"+totalNrOfObjects.ToString());
        string header = locateObjects();
        csvWriter.WriteLine(header);
    }

    string locateObjects()
    {
        ObjectsToRecord = new List<GameObject>();
        /*for (int i = 0; i < scene.transform.childCount; i++)
        {
            GameObject tempObj = scene.transform.GetChild(i).gameObject;
            if (tempObj.CompareTag(searchTag))
            {
                ObjectsToRecord.Add(tempObj);
            }

            int nrOfChildren = tempObj.transform.childCount;
            if (nrOfChildren > 0)
            {
                for (int ii = 0; ii < nrOfChildren; ii++)
                {
                    GameObject tempChild = tempObj.transform.GetChild(ii).gameObject;
                    if (tempChild.CompareTag(searchTag))
                    {
                        ObjectsToRecord.Add(tempChild);
                    }

                    int nrOfChildrensChildren = tempChild.transform.childCount;
                    if (nrOfChildrensChildren > 0)
                    {
                        for (int iii = 0; iii < nrOfChildrensChildren; iii++)
                        {
                            GameObject tempChildsChild = tempChild.transform.GetChild(iii).gameObject;
                            if (tempChildsChild.CompareTag(searchTag))
                            {
                                ObjectsToRecord.Add(tempChildsChild);
                            }
                        }
                    }
                }
            }
        }
        */
        ObjectsToRecord.Add(recObj1);
        ObjectsToRecord.Add(recObj2);
        ObjectsToRecord.Add(recObj3);
        ObjectsToRecord.Add(recObj4);
        ObjectsToRecord.Add(recObj5);
        ObjectsToRecord.Add(recObj6);
        ObjectsToRecord.Add(recObj7);
        ObjectsToRecord.Add(recObj8);
        ObjectsToRecord.Add(recObj9);
        ObjectsToRecord.Add(recObj10);


        totalNrOfObjects = ObjectsToRecord.Count;
        return CreateHeader();
    }

    string CreateStringSingleObj(GameObject obj,out Vector3 pos,out Quaternion ori)
    {
        int nrOfChildren = obj.transform.childCount;
        pos = obj.transform.position;
        ori = obj.transform.rotation;
        
        return null;
    }

    string posOriString(Vector3 pos,Quaternion ori)
    {
        string returnString = "";
        string positionString = pos.ToString("G");
        string orientationString = ori.ToString("G");
        string modifiedPositionString = positionString.Substring(1, positionString.Length - 2);
        string modifiedOrientationString = orientationString.Substring(1, orientationString.Length - 2);
        returnString = modifiedPositionString + "," + modifiedOrientationString + ",";
        //returnString += CreateInteractableString(obj)
        return returnString;
    }
    string CreateHeader()
    {
        string outputHeader = "";
        for (int i = 0; i < ObjectsToRecord.Count; i++)
        {
            outputHeader += headerStringSingleObj(ObjectsToRecord[i], "");
        }
        outputHeader = outputHeader.Substring(0, outputHeader.Length - 1);

        return outputHeader;
    }
    string headerStringSingleObj(GameObject obj,string prefix)
    {
        string childname = prefix;
        childname += obj.name;
        string returnString = childname + ".x," + childname + ".y," + childname + ".z," + childname + ".rx," +
                              childname + ".ry," + childname + ".rz," + childname + ".rw," + childname +
                              ".Interactable,";
        return returnString;
    }

    string CreateInteractableString(GameObject obj)
    {
        //InteractableObject interactionValues = obj.GetComponent<InteractableObject>() as InteractableObject;
        
        return "no DataImplemented,";
        //if (interactionValues == null) { return "noData,"; }

    }
}

