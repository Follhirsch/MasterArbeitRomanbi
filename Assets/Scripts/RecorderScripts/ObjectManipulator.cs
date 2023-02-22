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
    public Vector3[][] posArray;
    public Quaternion[][] oriArray;
    public int totalNrobjects;
    public List<GameObject> triggersToDeactivate;
    public List<MonoBehaviour> componentsToDeactivate;
    public float[][] veloArray;
    public string[][] interactionStringArray;

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
        
        triggersToDeactivate = new List<GameObject>();
        string[] tags = new [] { "NailHole","NailGroupTrigger" };
        for (int i = 0; i < tags.Length; i++)
        {
            triggersToDeactivate.AddRange(GameObject.FindGameObjectsWithTag(tags[i]));
        }
        
        componentsToDeactivate.AddRange(FindObjectsOfType<HammeringNail>());
        componentsToDeactivate.AddRange(FindObjectsOfType<ConstrainedNailScript>());
        componentsToDeactivate.AddRange(FindObjectsOfType<HandleScrewing>());
        componentsToDeactivate.AddRange(FindObjectsOfType<ButtonPress>());
        componentsToDeactivate.AddRange(FindObjectsOfType<HandleOnHammerScript>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public void startreplay() 
    {
        activateTriggersAndComponentsForReplay(false);
        replaying = true;
        StartCoroutine(ReplayObjects(false));
    }
    
     public void playShadowhands()
    {
        replaying = true;
        StartCoroutine(ReplayObjects(true));

    }
    */
    
    public void loadFromGame()
    {
        ObjectRecorder objRec = recorderSource.GetComponent<ObjectRecorder>();
        framerate = objRec.framerate;
        posArray = objRec.posVectors.ToArray();
        oriArray = objRec.oriQuaternion.ToArray();
        veloArray = objRec.velocityList.ToArray();
        interactionStringArray = objRec.IntaractinValuesList.ToArray();
        objectsToReplay = objRec.ObjectsToRecord;
        totalNrobjects = objectsToReplay.Count;
    }
    public bool loadFromCSVFile(string pathIn)
    {
        dir = pathIn + "/Objects";
        replayFile = Resources.Load<TextAsset>(dir);
        if (replayFile == null) { return false;}
        //syntax csv object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz,interactionvalues...
        string[] dataLines = replayFile.text.Split("\n");
        string[] recorderOptionStrings = dataLines[0].Split(",");
        string[] header = dataLines[1].Split(",");
        int frames = dataLines.Length - 2;
        framerate = int.Parse(recorderOptionStrings[1]); 
        totalNrobjects = int.Parse(recorderOptionStrings[3]);
        List<Vector3[]> tempPosVectorList = new List<Vector3[]>();
        List<Quaternion[]> tempOriList = new List<Quaternion[]>();
        List<float[]> tempVeloList = new List<float[]>();
        List<string[]> tempInteractionList = new List<string[]>();

        for (int i = 0; i < frames-1; i++)
        {
            string[] dataValues = dataLines[i + 2].Split(","); // starting from second line in csv
            Vector3[] tempPosFrame = new Vector3[totalNrobjects];
            Quaternion[] tempOriFrame = new Quaternion[totalNrobjects];
            float[] tempVeloFrame = new float[totalNrobjects];
            string[] tempInteractionFrame = new string[totalNrobjects];

            for (int j = 0; j < totalNrobjects; j++)
            {
                int k = j * 9;
                Vector3 positionData = new Vector3(float.Parse(dataValues[k]), float.Parse(dataValues[k + 1]),
                    float.Parse(dataValues[k + 2]));
                Quaternion orientationData = new Quaternion(float.Parse(dataValues[k + 3]),
                    float.Parse(dataValues[k + 4]), float.Parse(dataValues[k + 5]), float.Parse(dataValues[k + 6]));
                tempPosFrame[j] = positionData;
                tempOriFrame[j] = orientationData;
                tempVeloFrame[j] = float.Parse(dataValues[k + 7]);
                tempInteractionFrame[j] = dataValues[k + 8];
            }
            tempPosVectorList.Add(tempPosFrame);
            tempOriList.Add(tempOriFrame);
            tempVeloList.Add(tempVeloFrame);
            tempInteractionList.Add(tempInteractionFrame);
        }

        posArray = tempPosVectorList.ToArray();
        oriArray = tempOriList.ToArray();
        interactionStringArray = tempInteractionList.ToArray();
        veloArray = tempVeloList.ToArray();
        objectsToReplay = gameObject.GetComponent<ObjectRecorder>().ObjectsToRecord;
        totalNrobjects = objectsToReplay.Count;

        if (!GameObject.Find("MTM-transcription").GetComponent<ObjectInteractions>().ReadInteractionsCSV(pathIn,objectsToReplay))
        {
            Debug.Log("Interactions not loaded");
            return false;
        }
        
        Debug.Log("Objects CSV file Loaded");
        return true;
    }


    public void playFrame(int frameToPlay)
    {
        if (frame < 0 || frame > posArray.Length) 
        {
            Debug.Log("Invalid Frame: " + frameToPlay.ToString());
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

            objectsToReplay[i].GetComponent<InteractableObject>()
                .ReplayFromRecording(interactionStringArray[frameToPlay][i]);
        }
    }

    /*
    IEnumerator ReplayObjects(bool rewindAfter)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < posArray.Length; i++)
        {
            playFrame(i);
            yield return new WaitForSeconds(1 / framerate);

            if (!replaying)
            {
                activateTriggersAndComponentsForReplay(true);
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
                    activateTriggersAndComponentsForReplay(true);
                    Debug.Log("replay stopped");
                    yield break;
                }
            }
        }
        replaying = false;
        activateTriggersAndComponentsForReplay(true);
    }
    */

    void moveObjcts(int currentFrame,int currentObjNr ,GameObject currenObj)
    {
        currenObj.transform.position = posArray[currentFrame][currentObjNr];
            currenObj.transform.rotation = oriArray[currentFrame][currentObjNr];
    }
    public void activateTriggersAndComponentsForReplay(bool activate)
    {
        foreach (GameObject obj in triggersToDeactivate)
        {
            obj.SetActive(activate);
        }

        foreach (MonoBehaviour component in componentsToDeactivate)
        {
            component.enabled = activate;
        }

        foreach (var obj in objectsToReplay)
        {
            string n = obj.name;
            if (n.Equals("Button") || n.Equals("PedalButton") || n.Equals("VR Objects-Nail fixed") ||
                n.Equals("hammerhead-complete") || n.Equals("Handle-complete")||n.Equals("CrankHandle")){continue;}
            obj.GetComponent<Rigidbody>().useGravity = activate;
            if (activate)
            {
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
            else
            {
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
            
        }
    }

}
