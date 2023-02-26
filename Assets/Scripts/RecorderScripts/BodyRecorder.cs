using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
//using RootMotion.Demos;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Valve.Newtonsoft.Json.Utilities;
using Valve.VR;


public class BodyRecorder : MonoBehaviour
{
    StreamWriter csvWriterHands;
    StreamWriter csvWriterBody;
    public GameObject hands;
    public GameObject player;
    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject recorderObject;
    public List<Vector3[]> rPosVectors = new List<Vector3[]>();
    public List<Quaternion[]> rOriQuaternion = new List<Quaternion[]>();
    public List<Vector3[]> lPosVectors = new List<Vector3[]>();
    public List<Quaternion[]> lOriQuaternion = new List<Quaternion[]>();
    public List<Vector3[]> bodyRestPosVectors = new List<Vector3[]>();
    public List<Quaternion[]> bodyRestOriQuaternions = new List<Quaternion[]>();
    public int framerate;
    public bool recordBody;
    public bool recordHands = false;
    public bool recordHandPlayer = false;
    public bool recordBodyRest = false;
    public bool recordHip= false;
    public bool recordRightFoot = false;
    public bool recordLeftFoot = false;
    public bool recordHead = false;
    
    public List<int> lHstartGraspFrame = new List<int>();
    public List<int> lHstopGraspFrame = new List<int>();
    public List<int> rHstartGraspFrame = new List<int>();
    public List<int> rHstopGraspFrame = new List<int>();
    public bool rightHandOnGrasp;
    public bool leftHandOnGrasp;

    DirectoryInfo FolderDirectory;
    //private int samples = 0;

    
    //hands.transform.GetChild(4).gameObject;

    //GameObject leftHand = hands.transform.GetChild(1).gameObject;

    // Start is called before the first frame update
    void Start()
    {
        recorderObject = gameObject;
        framerate = recorderObject.GetComponent<RecorderMaster>().framerate;
        locateHandObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!recorderObject.GetComponent<RecorderMaster>().recordBody) { return;}
        recordBodyRest = recordHip || recordHead || recordLeftFoot || recordRightFoot||recordHandPlayer;
        
        if (Input.GetKeyDown("l"))
        {
            if (recordHands) {locateHandObjects();}
            if (recordBodyRest) { locateBodyRestObjects(); }
        }
    }

    public void LogData()
    {
        if (recordHands) {LogHandData();}

        if (recordBodyRest) {LogBodyData();}
        
    }

    public void StopRecording()
    {
        if (recordHands) { csvWriterHands.Close(); }
        if (recordBodyRest) { csvWriterBody.Close();}
    }
    public void StartRecording(string folderDir,string calibStr)
    {
        framerate = recorderObject.GetComponent<RecorderMaster>().framerate;
        
        /* NOW DONE IN RECORDERMASTER
        string dir = "Assets/Resources/Recordings/Recording" + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm");
        if(!Directory.Exists(dir))
        {
            FolderDirectory = Directory.CreateDirectory("Assets/Resources/Recordings/Recording"+ "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm"));
            dir = FolderDirectory.ToString();
        }*/
        bodyRestPosVectors.Clear();
        bodyRestOriQuaternions.Clear();
        lPosVectors.Clear();
        lOriQuaternion.Clear();
        rPosVectors.Clear();
        rOriQuaternion.Clear();
        

        if (recordHands) 
        {
            string header = locateHandObjects();
            string newpath1 = CreateUniqueFilePath(folderDir, "Hands", ".csv");
            csvWriterHands = new StreamWriter(newpath1);
            csvWriterHands.WriteLine("FPS,"+framerate.ToString()+"," + "NrOfHandObjects,"+"34");
            csvWriterHands.WriteLine(header);
        }

        if (recordBodyRest)
        {
            string header = locateBodyRestObjects();
            string newpath2 = CreateUniqueFilePath(folderDir, "BodyRest", ".csv");
            csvWriterBody = new StreamWriter(newpath2);
            int NrOfObjects = 7;
            /*Convert.ToInt16(recordHead) + 2 * Convert.ToInt16(recordHandShadow) +
                              Convert.ToInt16(recordHip) + Convert.ToInt16(recordLeftFoot) +
                              Convert.ToInt16(recordRightFoot);*/
            string firstline = "FPS," + framerate.ToString() + "," + "NrOfObjects," + NrOfObjects.ToString()+",";
            firstline += calibStr;
            /*firstline += ",LFoot," + Convert.ToString(recordLeftFoot) + ",RFoot," + Convert.ToString(recordRightFoot) + ",Hip," +
                             Convert.ToString(recordHip) + ",Head," + Convert.ToString(recordHead) + ",HandsPLayer," +
                             Convert.ToString(recordHandPlayer);*/
            
            csvWriterBody.WriteLine(firstline);
            csvWriterBody.WriteLine(header);
        }


    }

    string locateHandObjects()
    {
        string headerconstruction = "";
        
        GameObject rightHandObject = hands.transform.GetChild(0).gameObject;
        GameObject leftHandObject = hands.transform.GetChild(1).gameObject;

        rightHand = rightHandObject.transform.GetChild(4).gameObject;
        leftHand = leftHandObject.transform.GetChild(4).gameObject;

        for (int ri = 0; ri < rightHand.transform.childCount; ri++)//right hand header
        {
                string childname = rightHand.transform.GetChild(ri).name;
                headerconstruction += childname + ".x," + childname + ".y," + childname + ".z," + childname + ".rx," +
                                      childname + ".ry," + childname + ".rz," + childname + ".rw,";
        }

        for (int li = 0; li < leftHand.transform.childCount; li++)// left hand header
        {
                string childname = leftHand.transform.GetChild(li).name;
                headerconstruction += childname + ".x," + childname + ".y," + childname + ".z," + childname + ".rx," +
                                      childname + ".ry," + childname + ".rz," + childname + ".rw,";
        }

        headerconstruction += "rHandCollisionObject1" + "," + "grabbed"+"lHandCollisionObject1" + "," + "grabbed"+ "rHandCollisionObject2" + "," + "grabbed"+"lHandCollisionObject2" + "," + "grabbed"; 
        return headerconstruction;
    }

    string locateBodyRestObjects()
    {
        string headerconstruction = "";
        
        //order lfoot,rfoot,hip,controller,head,lhand,rhand
        for (int i = 0; i < player.transform.childCount; i++)// left hand header
        {
            string childname = "";
            GameObject tempObject = player.transform.GetChild(i).gameObject;
            if (tempObject.transform.childCount > 0) {childname = player.transform.GetChild(i).name;}
            else { childname = player.transform.name; }

            headerconstruction += childname + ".x," + childname + ".y," + childname + ".z," + childname + ".rx," +
                                  childname + ".ry," + childname + ".rz," + childname + ".rw,";
        }
        
        return headerconstruction;
    }

    void LogHandData()
    {
            string completeLineHands = "";
            Vector3[] rTempArray = new Vector3[17];
            Vector3[] lTempArray = new Vector3[17];
            Quaternion[] rTempOriArray = new Quaternion[17];
            Quaternion[] lTempOriArray = new Quaternion[17];
            //int li = leftHand.transform.childCount;
        
            for (int ri = 0; ri < rightHand.transform.childCount; ri++) //get position of right hand
            {
                Vector3 pos = rightHand.transform.GetChild(ri).position;
                Quaternion ori = rightHand.transform.GetChild(ri).rotation;
                string positionString = pos.ToString("G");
                string orientationString = ori.ToString("G");
                string modifiedPositionString = positionString.Substring(1, positionString.Length - 2);
                string modifiedOrientationString = orientationString.Substring(1, orientationString.Length - 2);
                completeLineHands += modifiedPositionString + "," + modifiedOrientationString + ",";
                
                rTempArray[ri] = pos;
                rTempOriArray[ri] = ori;

            }
            rPosVectors.Add(rTempArray);
            rOriQuaternion.Add(rTempOriArray);


            for (int li = 0; li < leftHand.transform.childCount; li++) //get position of left hand
            {
                Vector3 pos = leftHand.transform.GetChild(li).position;
                Quaternion ori = leftHand.transform.GetChild(li).rotation;
                string positionString = pos.ToString("G");
                string orientationString = ori.ToString("G");
                string modifiedPositionString = positionString.Substring(1, positionString.Length - 2);
                string modifiedOrientationString = orientationString.Substring(1, orientationString.Length - 2);
                completeLineHands += modifiedPositionString + "," + modifiedOrientationString + ",";
                lTempArray[li] = pos;
                lTempOriArray[li] = ori;

            }
            lPosVectors.Add(lTempArray);
            lOriQuaternion.Add(lTempOriArray);
            //write additional Data for hand interactions with interactables
            completeLineHands += "";

            csvWriterHands.WriteLine(completeLineHands);
    }

    void LogBodyData()
    {
        string completeLineBody = "";
        int NrOfObjects = player.transform.childCount;
        Vector3[] tempArray = new Vector3[NrOfObjects];
        Quaternion[] tempOriArray = new Quaternion[NrOfObjects];
        for (int i = 0; i < NrOfObjects; i++) //get position of left hand
        {
            GameObject temObj = player.transform.GetChild(i).gameObject;
            Vector3 pos;
            Quaternion ori;
            if (temObj.transform.childCount > 0)
            {
                pos = temObj.transform.GetChild(0).position;
                ori = temObj.transform.GetChild(0).rotation;
            }
            else
            {
                pos = temObj.transform.position;
                ori = temObj.transform.rotation;
            }
            string positionString = pos.ToString("G");
            string orientationString = ori.ToString("G");
            string modifiedPositionString = positionString.Substring(1, positionString.Length - 2);
            string modifiedOrientationString = orientationString.Substring(1, orientationString.Length - 2);
            completeLineBody += modifiedPositionString + "," + modifiedOrientationString + ",";
            tempArray[i] = pos;
            tempOriArray[i] = ori;

        }
        bodyRestPosVectors.Add(tempArray);
        bodyRestOriQuaternions.Add(tempOriArray);

        csvWriterBody.WriteLine(completeLineBody);
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
