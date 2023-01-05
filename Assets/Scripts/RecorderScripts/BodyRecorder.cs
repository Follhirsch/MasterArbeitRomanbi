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
    public bool recording = false;
    public bool recordBody;
    public bool recordHands = false;
    public bool recordHandShadow = false;
    public bool recordBodyRest = false;
    public bool recordHip= false;
    public bool recordRightFoot = false;
    public bool recordLeftFoot = false;
    public bool recordHead = false;
    
    DirectoryInfo FolderDirectory;

    private float timer = 0.0f;
    private float samplingInterval;
    //private int samples = 0;

    
    //hands.transform.GetChild(4).gameObject;

    //GameObject leftHand = hands.transform.GetChild(1).gameObject;

    // Start is called before the first frame update
    void Start()
    {
        recorderObject = gameObject;
        framerate = recorderObject.GetComponent<RecorderMaster>().framerate;
        samplingInterval = 1 / framerate;
        locateHandObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!recorderObject.GetComponent<RecorderMaster>().recordBody) { return;}
        recordBodyRest = recordHip || recordHead || recordLeftFoot || recordRightFoot||recordHandShadow;
            
        if (Input.GetKeyDown("r")) { toggleRecording(); }
        if (Input.GetKeyDown("f"))
        {
            if (recordHands) {locateHandObjects();}
            if (recordBodyRest) { locateBodyRestObjects(); }
        }

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
        if (recordHands)
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

            csvWriterHands.WriteLine(completeLineHands);
        }

        if (recordBodyRest)
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



        
    }

    void toggleRecording()
    {
        if (recording) //sto recording
        {
            recording = false;
            if (recordHands) { csvWriterHands.Close(); }

            if (recordBodyRest) { csvWriterBody.Close();}
            Debug.Log("recording body stopped");
        }
        else // start recording
        {
            framerate = recorderObject.GetComponent<RecorderMaster>().framerate;
            samplingInterval = 1 / framerate;
            string dir = "Assets/Resources/Recordings/Recording" + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm");
            if(!Directory.Exists(dir))
            {
                FolderDirectory = Directory.CreateDirectory("Assets/Resources/Recordings/Recording"+ "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm"));
                dir = FolderDirectory.ToString();
            }

            if (recordHands)
            {
                string header = locateHandObjects();
                csvWriterHands = new StreamWriter(dir + "/" + "Hands" + ".csv");
                csvWriterHands.WriteLine("FPS,"+framerate.ToString()+"," + "NrOfHandObjects,"+"34");
                csvWriterHands.WriteLine(header);
            }
            if (recordBodyRest)
            {
                string header = locateBodyRestObjects();
                csvWriterBody = new StreamWriter(dir + "/" + "BodyRest" + ".csv");
                int NrOfObjects = 7;
                    /*Convert.ToInt16(recordHead) + 2 * Convert.ToInt16(recordHandShadow) +
                                  Convert.ToInt16(recordHip) + Convert.ToInt16(recordLeftFoot) +
                                  Convert.ToInt16(recordRightFoot);*/
                string firstline = "FPS,"+framerate.ToString()+"," + "NrOfObjects,"+NrOfObjects.ToString();
                firstline += ",LFoot," + Convert.ToString(recordLeftFoot) + ",RFoot," +
                             Convert.ToString(recordRightFoot) + ",Hip," +
                             Convert.ToString(recordHip) + ",Head," + Convert.ToString(recordHead) + ",HandsShadow," +
                             Convert.ToString(recordHandShadow);
                csvWriterBody.WriteLine(firstline);
                csvWriterBody.WriteLine(header);
            }

            recording = true;
            Debug.Log("recording body started");
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

}
