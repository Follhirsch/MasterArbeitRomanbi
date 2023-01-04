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
using Valve.VR;

public class ObjectRecorder : MonoBehaviour
{
    StreamWriter csvWriter;
    public GameObject recorderObject;
    public List<Vector3[]> posVectors = new List<Vector3[]>();
    public List<Quaternion[]> oriQuaternion = new List<Quaternion[]>();
    public GameObject scene;
    public List<GameObject> objects = new List<GameObject>();
    public int framerate;
    public bool recording = false;
    DirectoryInfo FolderDirectory;

    private float timer = 0.0f;
    private float samplingInterval;
    //private int samples = 0;

    private int NrOfObjects;


    // Start is called before the first frame update
    void Start()
    {
        recorderObject = this.gameObject;
        framerate = recorderObject.GetComponent<RecorderMaster>().framerate;
        samplingInterval = 1 / framerate;
        locateObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (!recorderObject.GetComponent<RecorderMaster>().recordObjects) { return;}
        
        if (Input.GetKeyDown("r")) {toggleRecording(); }
        if (Input.GetKeyDown("l")) { locateObjects(); }
        

        if (recording)
        {
            timer += Time.deltaTime;
            if (timer > samplingInterval)
            {
                logData();
                timer = timer - samplingInterval;
            }
        }
    }

    void logData()
    {
        string completeLine = "";
        Vector3[] tempArray = new Vector3[NrOfObjects];
        Quaternion[] tempOriArray = new Quaternion[NrOfObjects];

            for (int i = 0; i < NrOfObjects; i++) //get position and orientation of objects
            {
                Vector3 pos = scene.transform.GetChild(i).position;
                Quaternion ori = scene.transform.GetChild(i).rotation;
                string positionString = pos.ToString("G");
                string orientationString = ori.ToString("G");
                string modifiedPositionString = positionString.Substring(1, positionString.Length - 2);
                string modifiedOrientationString = orientationString.Substring(1, orientationString.Length - 2);
                completeLine += modifiedPositionString + "," + modifiedOrientationString + ",";

                tempArray[i] = pos;
                tempOriArray[i] = ori;
            }
        posVectors.Add(tempArray);
        oriQuaternion.Add(tempOriArray);
        

        completeLine =  completeLine.Substring(0, completeLine.Length - 1);
        csvWriter.WriteLine(completeLine);
        //syntax csv: object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz,object1.rw ...
    }

    void toggleRecording()
    {
        if (recording) //sto recording
        {
            recording = false;
            csvWriter.Close();
            Debug.Log("recording stopped");
        }
        else // start recording
        {
            //samples = 0;
            framerate = recorderObject.GetComponent<RecorderMaster>().framerate;
            samplingInterval = 1 / framerate;
            
            FolderDirectory = Directory.CreateDirectory("Assets/Recources/Recordings/Recording"+ "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm"));
            string dir = FolderDirectory.ToString();
            csvWriter = new StreamWriter(dir + "/" + "Objects" + ".csv");
            csvWriter.WriteLine("FPS,"+framerate.ToString()+"," + "NrOfObjects,"+NrOfObjects.ToString());
            string header = locateObjects();
            
            csvWriter.WriteLine(header);
            recording = true;
            Debug.Log("recording started");
        }
    }

    string locateObjects()
    {

        string headerconstruction = "";
        NrOfObjects = scene.transform.childCount;

       for (int i = 0; i < NrOfObjects; i++)
       {
            objects.Add(scene.transform.GetChild(i).gameObject);
            string childname = scene.transform.GetChild(i).name;
                headerconstruction += childname + ".x," + childname + ".y," + childname + ".z," + childname + ".rx," + childname + ".ry," + childname + ".rz," + childname + ".rw,";
       }

        return headerconstruction;

    }

}
