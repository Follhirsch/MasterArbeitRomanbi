using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectManipulator : MonoBehaviour
{
    public string dir;
    private TextAsset replayFile;
    public GameObject recorderSource;

    public GameObject sceneTarget;
    public List<GameObject> objectsToReplay;
    public int frame;
    private Vector3[][] posArray;
    private Quaternion[][] oriArray;
    public int totalNrobjects;

    private int framerate = 30;

    private bool replaying = false;
    // Start is called before the first frame update
    void Start()
    {
        sceneTarget = gameObject.GetComponent<RecorderMaster>().SceneToRecord;
        recorderSource = this.gameObject;
        frame = 0;
        replaying = false;
        objectsToReplay = gameObject.GetComponent<ObjectRecorder>().ObjectsToRecord;
        totalNrobjects = objectsToReplay.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("3"))
        {
            replaying = false;
           playFrame(frame);
        }
    }

    public void startreplay()
    {
        replaying = true;
        StartCoroutine(ReplayObjects(false));
    }

    public void playShadowhands()
    {
        replaying = true;
        StartCoroutine(ReplayObjects(true));

    }
    
    public void loadFromGame()
    {
        framerate = recorderSource.GetComponent<ObjectRecorder>().framerate;
        posArray = recorderSource.GetComponent<ObjectRecorder>().posVectors.ToArray();
        oriArray = recorderSource.GetComponent<ObjectRecorder>().oriQuaternion.ToArray();
        objectsToReplay = gameObject.GetComponent<ObjectRecorder>().ObjectsToRecord;
        totalNrobjects = objectsToReplay.Count;
    }
    public void loadFromCSVFile(string pathIn)
    {
        dir = pathIn + "/Objects";
        replayFile = Resources.Load<TextAsset>(dir);
        //syntax csv object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz,interactionvalues...
        string[] dataLines = replayFile.text.Split("\n");
        string[] recorderOptionStrings = dataLines[0].Split(",");
        string[] header = dataLines[1].Split(",");
        int frames = dataLines.Length - 2;
        framerate = int.Parse(recorderOptionStrings[1]); 
        totalNrobjects = int.Parse(recorderOptionStrings[3]);
        Debug.Log(totalNrobjects.ToString()+"numberofobj");

        List<Vector3[]> tempPosVectorList = new List<Vector3[]>();
        List<Quaternion[]> tempOriList = new List<Quaternion[]>();

        for (int i = 0; i < frames-1; i++)
        {
            string[] dataValues = dataLines[i + 2].Split(","); // starting from second line in csv
            Vector3[] tempPosFrame = new Vector3[totalNrobjects];
            Quaternion[] tempOriFrame = new Quaternion[totalNrobjects];
            for (int j = 0; j < totalNrobjects; j++)
            {
                int k = j * 8;
                Vector3 positionData = new Vector3(float.Parse(dataValues[k]),float.Parse(dataValues[k+1]),float.Parse(dataValues[k+2]));
                Quaternion orientationData = new Quaternion(float.Parse(dataValues[k+3]),
                    float.Parse(dataValues[k + 4]), float.Parse(dataValues[k + 5]),float.Parse(dataValues[k + 6]));
                tempPosFrame[j] = positionData;
                tempOriFrame[j] = orientationData;
            }
            tempPosVectorList.Add(tempPosFrame);
            tempOriList.Add(tempOriFrame);
        }

        posArray = tempPosVectorList.ToArray();
        oriArray = tempOriList.ToArray();
        objectsToReplay = gameObject.GetComponent<ObjectRecorder>().ObjectsToRecord;
        totalNrobjects = objectsToReplay.Count;
        Debug.Log("Objects CSV file Loaded");
    }


    public void playFrame(int frameToPlay)
    {
        if (frame < 0 || frame > posArray.Length) 
        {
            Debug.Log("invald Frame: " + frameToPlay.ToString());
            return;
        }

        if (posArray == null && oriArray == null)
        {
            Debug.Log("Positions not loaded"); 
            return;
        }

        for (int i = 0; i < objectsToReplay.Count; i++)
        {
            objectsToReplay[i].transform.position = posArray[frameToPlay][i];
            objectsToReplay[i].transform.rotation = oriArray[frameToPlay][i];
            //TODO: object interaction values;
        }
    }

    IEnumerator ReplayObjects(bool rewindAfter)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < posArray.Length; i++)
        {
            playFrame(i);
            yield return new WaitForSeconds(1 / framerate);

            if (!replaying)
            {
                Debug.Log("replay stopped");
                yield break;
            }
        }

        if (rewindAfter)
        {
            for (int i = posArray.Length-1; i > 0 ; i = i-2)
            {
                playFrame(i);
                yield return new WaitForSeconds(1 / framerate);

                if (!replaying)
                {
                    Debug.Log("replay stopped");
                    yield break;
                }
            }
        }
        replaying = false;
    }

    void moveObjcts(int currentFrame,int currentObjNr ,GameObject currenObj)
    {
        currenObj.transform.position = posArray[currentFrame][currentObjNr];
            currenObj.transform.rotation = oriArray[currentFrame][currentObjNr];
    }

}
