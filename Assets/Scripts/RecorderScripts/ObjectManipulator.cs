using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{
    public string dir;
    private TextAsset replayFile;
    public GameObject recorderSource;

    public GameObject sceneTarget;
    
    public int frame;
    private Vector3[][] posArray;
    private Quaternion[][] oriArray;

    private int framerate = 30;

    private bool replaying = false;
    // Start is called before the first frame update
    void Start()
    {
        recorderSource = this.gameObject;
        frame = 0;
        replaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("3"))
        {
            replaying = false;
           playFrame();
        }
    }

    public void startreplay()
    {
        replaying = true;
        StartCoroutine(ReplayObjects());
    }
    
    public void loadFromGame()
    {
        framerate = recorderSource.GetComponent<ObjectRecorder>().framerate;
        posArray = recorderSource.GetComponent<ObjectRecorder>().posVectors.ToArray();
        oriArray = recorderSource.GetComponent<ObjectRecorder>().oriQuaternion.ToArray();
        Debug.Log("Object Files Loaded");
    }
    public void loadFromCSVFile()
    {
        dir = recorderSource.GetComponent<RecorderMaster>().path + "/Objects";
        
        replayFile = Resources.Load<TextAsset>(dir);
        
        //syntax csv object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz...
        string[] dataLines = replayFile.text.Split("\n");
        string[] recorderOptionStrings = dataLines[0].Split(",");
        string[] header = dataLines[1].Split(",");
        int frames = dataLines.Length - 2;
        framerate = int.Parse(recorderOptionStrings[1]); 
        int objects = int.Parse(recorderOptionStrings[3]);

        List<Vector3[]> tempPosVectorList = new List<Vector3[]>();
        List<Quaternion[]> tempOriList = new List<Quaternion[]>();

        for (int i = 0; i < frames - 1 ; i++) // 
        {
            string[] dataValues = dataLines[i + 2].Split(","); // starting from second line in csv
            Vector3[] tempPosFrame = new Vector3[objects];
            Quaternion[] tempOriFrame = new Quaternion[objects];
            for (int ii = 0; ii < objects; ii++)
            {
                int iii = ii * 7;
                //get data
                Vector3 positionData = new Vector3(float.Parse(dataValues[iii]),float.Parse(dataValues[iii+1]),float.Parse(dataValues[iii+2]));
                Quaternion orientationData = new Quaternion(float.Parse(dataValues[iii+3]),
                    float.Parse(dataValues[iii + 4]), float.Parse(dataValues[iii + 5]),float.Parse(dataValues[iii + 6]));
                
                tempPosFrame[ii] = positionData;
                tempOriFrame[ii] = orientationData;
            }
            tempPosVectorList.Add(tempPosFrame);
            tempOriList.Add(tempOriFrame);
        }
        
        posArray = tempPosVectorList.ToArray();
        oriArray = tempOriList.ToArray();
        Debug.Log("Objects CSV file Loaded");
    }


    public void playFrame()
    {
        if (posArray != null && oriArray != null)
        {
            for (int ii = 0; ii < sceneTarget.transform.childCount; ii++)
            {
                sceneTarget.transform.GetChild(ii).transform.position = posArray[frame][ii];
                sceneTarget.transform.GetChild(ii).transform.rotation = oriArray[frame][ii];
            }
        }
        else
        {
            Debug.Log("Positions not loaded");
        }
        frame++;
    }

    IEnumerator ReplayObjects()
    {
        Debug.Log("Replay started");
        for (int i = 0; i < posArray.Length; i++)
        {
            if (posArray != null && oriArray != null)
            {
                for (int ii = 0; ii < sceneTarget.transform.childCount; ii++)
                {
                    sceneTarget.transform.GetChild(ii).transform.position = posArray[i][ii];
                    sceneTarget.transform.GetChild(ii).transform.rotation = oriArray[i][ii];
                }
            }
            else {Debug.Log("Positions not loaded"); }
            
            yield return new WaitForSeconds(1 / framerate);
            
            if (!replaying) { break; }
        }
        Debug.Log("Replay stopped");
    }
    

}