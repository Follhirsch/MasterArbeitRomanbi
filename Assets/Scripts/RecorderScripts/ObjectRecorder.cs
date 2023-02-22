using System.Collections;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
//using RootMotion.Demos;
using Unity.VisualScripting;
using UnityEditor;
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
    public GameObject recObj11;
    public List<string[]> IntaractinValuesList = new List<string[]>();
    public List<float[]> velocityList = new List<float[]>();
    public string folderdir;

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
        float[] tempVeloArray = new float[totalNrOfObjects];
        string[] tempInteractionArray = new string[totalNrOfObjects];

        for (int i = 0; i < totalNrOfObjects; i++)
        {
            string tempString = "";
            Vector3 pos = ObjectsToRecord[i].transform.position;
            Quaternion ori = ObjectsToRecord[i].transform.rotation;
            float velo = ObjectsToRecord[i].GetComponent<Rigidbody>().velocity.magnitude;
            tempArray[i] = pos;
            tempOriArray[i] = ori;
            tempVeloArray[i] = velo;
            tempInteractionArray[i] = CreateInteractableString(ObjectsToRecord[i]);
            tempString += posOriVelString(pos, ori, velo);
            tempString += tempInteractionArray[i]+",";
            completeLine += tempString;
        }

        posVectors.Add(tempArray);
        oriQuaternion.Add(tempOriArray);
        IntaractinValuesList.Add(tempInteractionArray);
        velocityList.Add(tempVeloArray);
        
        completeLine =  completeLine.Substring(0, completeLine.Length - 1);//remove , from the end
        csvWriter.WriteLine(completeLine);
        //syntax csv: object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz,object1.rw,object1.Interactionvalues ...
    }

    public void StopRecording()
    {
        csvWriter.Close();
    }

    public void StartRecording(string folderDir)
    {
        folderdir = folderDir;
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
        string newpath = CreateUniqueFilePath(folderDir, "Objects", ".csv");
        csvWriter = new StreamWriter(newpath);
        csvWriter.WriteLine("FPS,"+framerate.ToString()+"," + "NrOfObjects,"+totalNrOfObjects.ToString());
        string header = locateObjects();
        csvWriter.WriteLine(header);
    }

    string locateObjects()
    {
        ObjectsToRecord = new List<GameObject>();
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
        ObjectsToRecord.Add(recObj11);

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

    string posOriVelString(Vector3 pos,Quaternion ori,float velocity)
    {
        string returnString = "";
        string positionString = pos.ToString("G");
        string orientationString = ori.ToString("G");
        string modifiedPositionString = positionString.Substring(1, positionString.Length - 2);
        string modifiedOrientationString = orientationString.Substring(1, orientationString.Length - 2);
        returnString = modifiedPositionString + "," + modifiedOrientationString + ","+velocity+",";
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
                              childname + ".ry," + childname + ".rz," + childname + ".rw," + childname + ".velocity," +
                              childname + ".Interactable,";
        return returnString;
    }

    string CreateInteractableString(GameObject obj)
    { 
        return obj.GetComponent<InteractableObject>().CreateStringToRecord();
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

