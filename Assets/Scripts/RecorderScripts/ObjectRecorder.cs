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
    //public List<GameObject> objects = new List<GameObject>();
    public int framerate;
    DirectoryInfo FolderDirectory;
    
    //private int samples = 0;

    private int NrOfObjects;
    private int totalNrOfObjects; //with children


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

        if (Input.GetKeyDown("t"))
        { 
            Vector3 pos; 
            Quaternion ori;
            if (GetValuesFromObj(scene, out pos, out ori) > 0 ) { Debug.Log("obj has childs"); }
            else { Debug.Log("obj has NO childs"); }
            Debug.Log(pos);
        }
    }

    public void LogData()
    {
        string completeLine = "";
        Vector3[] tempArray = new Vector3[totalNrOfObjects]; // dont forget to add children
        Quaternion[] tempOriArray = new Quaternion[totalNrOfObjects];
        int tempGlobalIndex = 0; 
        
            for (int i = 0; i < NrOfObjects; i++) //get position and orientation of objects, maximum 3 layers in scene
            {
                GameObject tempObj = scene.transform.GetChild(i).gameObject;
                Vector3 pos;
                Quaternion ori;
                int nrOfChildrenInObj = GetValuesFromObj(tempObj, out pos, out ori);
                completeLine += creatStringSingleObj(pos,ori);
                tempArray[tempGlobalIndex] = pos;
                tempOriArray[tempGlobalIndex] = ori;
                tempGlobalIndex++;
                if (nrOfChildrenInObj > 0)
                {
                    for (int ii = 0; ii < nrOfChildrenInObj; ii++)
                    {
                        GameObject tempChild = tempObj.transform.GetChild(ii).gameObject;
                        int nrOfChildrenInChild = GetValuesFromObj(tempChild, out pos, out ori);
                        completeLine += creatStringSingleObj(pos, ori);
                        tempArray[tempGlobalIndex] = pos;
                        tempOriArray[tempGlobalIndex] = ori;
                        tempGlobalIndex++;
                        if (nrOfChildrenInChild > 0)
                        {
                            for (int iii = 0; iii < nrOfChildrenInChild; iii++)
                            {
                                GameObject tempChildesChild = tempChild.transform.GetChild(iii).gameObject;
                                int uslesReturn = GetValuesFromObj(tempChildesChild, out pos, out ori);
                                completeLine += creatStringSingleObj(pos, ori);
                                tempArray[tempGlobalIndex] = pos;
                                tempOriArray[tempGlobalIndex] = ori;
                                tempGlobalIndex++;
                            }
                        }
                    }
                }
            }
        posVectors.Add(tempArray);
        oriQuaternion.Add(tempOriArray);
        
        completeLine =  completeLine.Substring(0, completeLine.Length - 1);//remove , from the end
        csvWriter.WriteLine(completeLine);
        //syntax csv: object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz,object1.rw ...
    }

    public void StopRecording()
    {
        csvWriter.Close();
    }

    public void StartRecording(string folderDir)
    {
        framerate = recorderObject.GetComponent<RecorderMaster>().framerate;
        
        /* NOW PERFORMED IN RECORDERMASTER
        string dir = "Assets/Resources/Recordings/Recording" + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm");
        if(!Directory.Exists(dir))
        {
            FolderDirectory = Directory.CreateDirectory("Assets/Resources/Recordings/Recording"+ "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm"));
            dir = FolderDirectory.ToString();
        }
        */
            
        csvWriter = new StreamWriter(folderDir + "/" + "Objects" + ".csv");
        csvWriter.WriteLine("FPS,"+framerate.ToString()+"," + "NrOfObjects,"+NrOfObjects.ToString()+","+"TotalNrOfObjects,"+totalNrOfObjects.ToString());
        string header = locateObjects();
        csvWriter.WriteLine(header);
    }

    string locateObjects()
    {

        string headerconstruction = "";
        NrOfObjects = scene.transform.childCount;
        totalNrOfObjects = NrOfObjects;
        //cout children 
        string headerAddonForInteractions = "";

       for (int i = 0; i < NrOfObjects; i++)
       {
           string tempHeader = "";
           //objects.Add(scene.transform.GetChild(i).gameObject);
           GameObject tempObj = scene.transform.GetChild(i).gameObject;
           tempHeader = headerStringSingleObj(tempObj,"");
           headerconstruction += tempHeader;
           int nrOfChildren = tempObj.transform.childCount;
           totalNrOfObjects += nrOfChildren;
           if ( nrOfChildren > 0)
           {
               for (int ii = 0; ii < nrOfChildren; ii++)
               {
                   GameObject tempChild = tempObj.transform.GetChild(ii).gameObject;
                   tempHeader = headerStringSingleObj(tempChild,"/");
                   headerconstruction += tempHeader;
                   int nrOfChildrensChildren = tempChild.transform.childCount;
                   totalNrOfObjects += nrOfChildrensChildren;
                   
                   if (nrOfChildrensChildren > 0)
                   {
                       for (int iii = 0; iii < nrOfChildrensChildren; iii++)
                       {
                           GameObject tempChildsChild = tempChild.transform.GetChild(iii).gameObject;
                           tempHeader = headerStringSingleObj(tempChildsChild,"//");
                           headerconstruction += tempHeader;
                       }
                   }
               }
           }
           headerAddonForInteractions += "";
       }
       return headerconstruction;

    }

    int GetValuesFromObj(GameObject obj,out Vector3 pos,out Quaternion ori)
    {
        int nrOfChildren = obj.transform.childCount;
        pos = obj.transform.position;
        ori = obj.transform.rotation;
        
        return nrOfChildren;
    }

    string creatStringSingleObj(Vector3 pos,Quaternion ori)
    {
        string returnString = "";
        string positionString = pos.ToString("G");
        string orientationString = ori.ToString("G");
        string modifiedPositionString = positionString.Substring(1, positionString.Length - 2);
        string modifiedOrientationString = orientationString.Substring(1, orientationString.Length - 2);
        returnString = modifiedPositionString + "," + modifiedOrientationString + ",";
        return returnString;
    }

    string headerStringSingleObj(GameObject obj,string prefix)
    {
        string childname = prefix;
        childname += obj.name;
        string returnString = childname + ".x," + childname + ".y," + childname + ".z," + childname + ".rx," +
                              childname + ".ry," + childname + ".rz," + childname + ".rw,";
        return returnString;
    }

}

