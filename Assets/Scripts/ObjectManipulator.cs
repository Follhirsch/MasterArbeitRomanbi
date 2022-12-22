using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{
    public bool loadFromFile = false;
    public TextAsset replayFile;
    public GameObject recorderSource;

    public GameObject sceneTarget;
    
    public int frame;
    private Vector3[][] posArray;
    private Quaternion[][] oriArray;

    private int framerate = 30;
    // Start is called before the first frame update
    void Start()
    {
        frame = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            //get hand posees
            if(loadFromFile)
            {
                loadFromCSVFile();
            }
            else
            {
                loadFromGame();
            }

        }
        if (Input.GetKeyDown("2"))
        {
            StartCoroutine(replayObjects());
        }
        if (Input.GetKeyDown("3"))
        {
           playFrame();
        }
    }

    void loadFromGame()
    {
        framerate = recorderSource.GetComponent<ObjectRecorder>().framerate;
        posArray = recorderSource.GetComponent<ObjectRecorder>().posVectors.ToArray();
        oriArray = recorderSource.GetComponent<ObjectRecorder>().oriQuaternion.ToArray();

    }
    void loadFromCSVFile()
    {
        //syntax csv object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz...
        string[] dataLines = replayFile.text.Split("\n");
        string[] recorderOptionStrings = dataLines[0].Split(",");
        string[] header = dataLines[1].Split(",");
        int frames = dataLines.Length - 2;
        framerate = int.Parse(recorderOptionStrings[1]); 
        int objects = int.Parse(recorderOptionStrings[3]);;

        List<Vector3[]> tempPosVectorList = new List<Vector3[]>();
        List<Quaternion[]> tempOriList = new List<Quaternion[]>();
        Vector3[] tempPosFrame = new Vector3[objects];
        Quaternion[] tempOriFrame = new Quaternion[objects];

        for (int i = 0; i < frames - 1; i++)// starting from second line in code
        {
            string[] dataValues = dataLines[i + 2].Split(",");
            for (int ii = 0; ii < objects; ii++)
            {
                int iii = ii * 7;
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
        Debug.Log("Replay loaded");
        Debug.Log(posArray[1][10]);
    }


    void playFrame()
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
    }

    IEnumerator replayObjects()
    {
        for (int i = 0; i < posArray.Length; i++)
        {

            // do right hand pose
            if (posArray != null && oriArray != null)
            {
                Debug.Log("Playframe"+ frame.ToString());
                for (int ii = 0; ii < sceneTarget.transform.childCount; ii++)
                {
                    sceneTarget.transform.GetChild(ii).transform.position = posArray[i][ii];
                    sceneTarget.transform.GetChild(ii).transform.rotation = oriArray[i][ii];
                }
            }
            else
            {
                Debug.Log("Positions not loaded");
            }
            yield return new WaitForSeconds(1 / framerate);
            
        }

    }
}
