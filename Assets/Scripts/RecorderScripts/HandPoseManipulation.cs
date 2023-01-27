using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HandPoseManipulation : MonoBehaviour
{
    public GameObject recorderSource;

    public GameObject handTarget;
    
    public string dir;
    private TextAsset replayFile;
    
    public int framerate = 30;
    public bool replaying = false;
    
    public int frame;
    public Vector3[][] rPosArray;
    public Quaternion[][] rOriArray;
    public Vector3[][] lPosArray;
    public Quaternion[][] lOriArray;
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
        /*if (Input.GetKeyDown("1"))
        {
            //get hand posees
            //loadFromGame();
        }
        if (Input.GetKeyDown("2"))
        {
            //StartCoroutine( replayHands());
        }*/
        if (Input.GetKeyDown("3"))
        {
            replaying = false;
            playFrame();
        }
    }
    
    public void startreplay()
    {
        replaying = true;
        StartCoroutine(replayHands());
    }

    public void loadFromGame()
    {
        rPosArray = recorderSource.GetComponent<BodyRecorder>().rPosVectors.ToArray();
        rOriArray = recorderSource.GetComponent<BodyRecorder>().rOriQuaternion.ToArray();
        lPosArray = recorderSource.GetComponent<BodyRecorder>().lPosVectors.ToArray();
        lOriArray = recorderSource.GetComponent<BodyRecorder>().lOriQuaternion.ToArray();

    }

    public void loadFromCSVFile()
    {
        dir = recorderSource.GetComponent<RecorderMaster>().path + "/Hands";
        replayFile = Resources.Load<TextAsset>(dir);
        

        //syntax csv object1.x,object1.y,object1.z,object1.rx,object1.ry,object1.rz...
        string[] dataLines = replayFile.text.Split("\n");
        string[] recorderOptionStrings = dataLines[0].Split(",");
        string[] header = dataLines[1].Split(",");
        int frames = dataLines.Length - 2;
        framerate = int.Parse(recorderOptionStrings[1]); 
        int NrOfobjects = int.Parse(recorderOptionStrings[3]);
        
        List<Vector3[]> ltempPosVectorList = new List<Vector3[]>();
        List<Quaternion[]> ltempOriList = new List<Quaternion[]>();
        List<Vector3[]> rtempPosVectorList = new List<Vector3[]>();
        List<Quaternion[]> rtempOriList = new List<Quaternion[]>();

        //assign vaues to list
        for (int i = 0; i < frames - 1 ; i++) // 
        {
            string[] dataValues = dataLines[i + 2].Split(","); // starting from second line in csv
            Vector3[] ltempPosFrame = new Vector3[NrOfobjects/2];
            Quaternion[] ltempOriFrame = new Quaternion[NrOfobjects];
            Vector3[] rtempPosFrame = new Vector3[NrOfobjects/2];
            Quaternion[] rtempOriFrame = new Quaternion[NrOfobjects];
            for (int ii = 0; ii < NrOfobjects; ii++)
            {
                int iii = ii * 7;
                //get data
                Vector3 positionData = new Vector3(float.Parse(dataValues[iii]),float.Parse(dataValues[iii+1]),float.Parse(dataValues[iii+2]));
                Quaternion orientationData = new Quaternion(float.Parse(dataValues[iii+3]),
                    float.Parse(dataValues[iii + 4]), float.Parse(dataValues[iii + 5]),float.Parse(dataValues[iii + 6]));
                if (ii < NrOfobjects / 2)
                {
                    rtempPosFrame[ii] = positionData;
                    rtempOriFrame[ii] = orientationData;
                }
                else
                {
                    ltempPosFrame[ii-NrOfobjects/2] = positionData;
                    ltempOriFrame[ii-NrOfobjects/2] = orientationData;
                    
                }
                
            }

            ltempPosVectorList.Add(ltempPosFrame);
            rtempPosVectorList.Add(rtempPosFrame);
            ltempOriList.Add(ltempOriFrame);
            rtempOriList.Add(rtempOriFrame);
        }
        lPosArray = ltempPosVectorList.ToArray();
        lOriArray = ltempOriList.ToArray();
        rPosArray = rtempPosVectorList.ToArray();
        rOriArray = rtempOriList.ToArray();
        
        Debug.Log("Hands CSV file Loaded");
    }
    

    IEnumerator replayHands()
    {
        for (int i = 0; i < rPosArray.Length; i++)
        {

            // do right hand pose
            if (rPosArray != null && rOriArray != null)
            {
                GameObject rightHandObject = handTarget.transform.GetChild(0).gameObject;
                GameObject rightHandTarget = rightHandObject.transform.GetChild(4).gameObject;
                
                for (int ri = 0; ri < rightHandTarget.transform.childCount; ri++)
                {
                    rightHandTarget.transform.GetChild(ri).transform.position = rPosArray[i][ri];
                    rightHandTarget.transform.GetChild(ri).transform.rotation = rOriArray[i][ri];
                }
            }
            // do left hand pose
            if (lPosArray != null && lOriArray != null)
            {
                
                GameObject leftHandObject = handTarget.transform.GetChild(1).gameObject;
                GameObject lefthandTarget = leftHandObject.transform.GetChild(4).gameObject;
                for (int li = 0; li < lefthandTarget.transform.childCount; li++)
                {
                    lefthandTarget.transform.GetChild(li).transform.position = lPosArray[i][li];
                    lefthandTarget.transform.GetChild(li).transform.rotation = lOriArray[i][li];
                }
            }
            
            
            else
            {
                Debug.Log("HandPositions not loaded");
            }
            yield return new WaitForSeconds(1/recorderSource.GetComponent<BodyRecorder>().framerate);
        }
    }

    public void playFrame()
    {
        // do right hand pose
        if (rPosArray != null && rOriArray != null)
        {
            GameObject rightHandObject = handTarget.transform.GetChild(0).gameObject;
            GameObject rightHandTarget = rightHandObject.transform.GetChild(4).gameObject;
                
            for (int ri = 0; ri < rightHandTarget.transform.childCount; ri++)
            {
                rightHandTarget.transform.GetChild(ri).transform.position = rPosArray[frame][ri];
                rightHandTarget.transform.GetChild(ri).transform.rotation = rOriArray[frame][ri];
            }
        }
        // do left hand pose
        if (lPosArray != null && lOriArray != null)
        {
            GameObject leftHandObject = handTarget.transform.GetChild(1).gameObject;
                
            GameObject lefthandTarget = leftHandObject.transform.GetChild(4).gameObject;
            for (int li = 0; li < lefthandTarget.transform.childCount; li++)
            {
                lefthandTarget.transform.GetChild(li).transform.position = lPosArray[frame][li];
                lefthandTarget.transform.GetChild(li).transform.rotation = lOriArray[frame][li];
            }
        }
            
            
        else
        {
            Debug.Log("Positions not loaded");
        }
    }
}
