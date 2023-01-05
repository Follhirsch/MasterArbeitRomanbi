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
        framerate = recorderSource.GetComponent<BodyRecorder>().framerate;
        posArray = recorderSource.GetComponent<BodyRecorder>().bodyRestPosVectors.ToArray();
        oriArray = recorderSource.GetComponent<BodyRecorder>().bodyRestOriQuaternions.ToArray();
        Debug.Log("Player Files Loaded");
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
    
    IEnumerator ReplayObjects()
    {
        Debug.Log("Replay started");
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
        Debug.Log("Replay stopped");
    }
}


