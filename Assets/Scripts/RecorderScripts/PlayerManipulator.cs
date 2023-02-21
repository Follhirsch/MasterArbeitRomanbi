using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerManipulator : MonoBehaviour
{
    public string dir;
    private TextAsset replayFile;
    public GameObject recorderSource;

    public GameObject PlayerTarget;
    public GameObject ReplayHuman;
    public int frame;
    public Vector3[][] posArray;
    public Quaternion[][] oriArray;

    private int framerate = 30;

    private bool replaying = false;
    public bool showHuman = false;
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
    /*
    public void startreplay()
    {
        if (posArray != null)
        {
            replaying = true;
            StartCoroutine(ReplayObjects());
        }
        else
        {
            Debug.Log("no player replay file");
        }
    }
    */
    
    public void loadFromGame()
    {
        framerate = recorderSource.GetComponent<BodyRecorder>().framerate;
        posArray = recorderSource.GetComponent<BodyRecorder>().bodyRestPosVectors.ToArray();
        oriArray = recorderSource.GetComponent<BodyRecorder>().bodyRestOriQuaternions.ToArray();
        Debug.Log("Player Files Loaded");
    }
    
    public Tuple<bool,Tuple<float,float,float>> loadFromCSVFile(string pathIn)
    {
        dir = pathIn + "/BodyRest";
        replayFile = Resources.Load<TextAsset>(dir);
        if (replayFile == null)
        {
            return new Tuple<bool, Tuple<float, float, float>>(false, new Tuple<float, float, float>(1, 1, 1));
        }

        //syntax csv object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz...
        string[] dataLines = replayFile.text.Split("\n");
        string[] recorderOptionStrings = dataLines[0].Split(",");
        string[] header = dataLines[1].Split(",");
        int frames = dataLines.Length - 2;
        framerate = int.Parse(recorderOptionStrings[1]); 
        int objects = int.Parse(recorderOptionStrings[3]);
        Tuple<float, float, float> calibValues = new Tuple<float, float, float>(
            float.Parse(recorderOptionStrings[5]), float.Parse(recorderOptionStrings[7]),
            float.Parse(recorderOptionStrings[9])
        );
        
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
        Debug.Log("Player CSV file Loaded");
        return new Tuple<bool, Tuple<float, float, float>>(true, calibValues);
    }
    public void playFrame()
    {
        if (posArray != null && oriArray != null)
        {
            for (int ii = 0; ii < PlayerTarget.transform.childCount; ii++)
            {
                PlayerTarget.transform.GetChild(ii).transform.position = posArray[frame][ii];
                PlayerTarget.transform.GetChild(ii).transform.rotation = oriArray[frame][ii];
            }
        }
        else
        {
            Debug.Log("Positions not loaded");
        }
        frame++;
    }

    public void playFrame(int frameToPlay)
    {
        if (frame < 0 || frame > posArray.Length) 
        {
            Debug.Log("Invalid Frame: " + frameToPlay.ToString());
            return;
        }
        if(posArray == null && oriArray == null)
        {
            Debug.Log("Positions not loaded"); 
            return;
        }
        
        for (int i = 0; i < PlayerTarget.transform.childCount; i++)
        {
            PlayerTarget.transform.GetChild(i).GetChild(0).transform.position = posArray[frameToPlay][i];
            PlayerTarget.transform.GetChild(i).GetChild(0).transform.rotation = oriArray[frameToPlay][i];
        }
        
    }
    
    IEnumerator ReplayObjects()
    {
        yield return new WaitForSeconds(0.5f);
        
        for (int i = 0; i < posArray.Length; i++)
        {
            if (posArray != null && oriArray != null)
            {
                for (int ii = 0; ii < PlayerTarget.transform.childCount; ii++)
                {
                    PlayerTarget.transform.GetChild(ii).GetChild(0).transform.position = posArray[i][ii];
                    PlayerTarget.transform.GetChild(ii).GetChild(0).transform.rotation = oriArray[i][ii];
                }
            }
            else {Debug.Log("Positions not loaded"); }
            
            yield return new WaitForSeconds(1 / framerate);
            
            if (!replaying) { break; }
        }
    }
    public void EnablePlayer(bool enable)
    {
        PlayerTarget.SetActive(enable);
        if (showHuman)
        {
            ReplayHuman.SetActive(enable);
            //TODO adabt player dimensions
        }
    }
}


