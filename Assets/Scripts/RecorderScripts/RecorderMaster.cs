using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Valve.Newtonsoft.Json.Utilities;

public class RecorderMaster : MonoBehaviour
{
    public int framerate = 30;
    private float timer = 0.0f;
    private float samplingInterval;
    public bool recording = false;
    public bool recordBody;
    public bool recordObjects;
    public bool transcribeMTM;
    private int recordedSequenceNr;
    public int frame;
    
    public ObjectManipulator objMani;
    public HandPoseManipulation handMani;
    public PlayerManipulator playerMani;
    public BodyRecorder bodyRec;
    public ObjectRecorder objRec;
    
    
    public bool rePlaying = false;
    public bool loadFromCsvFile = false;
    public string recordingFilesDir;
    private DirectoryInfo folderDirectory; // folder where a recording is stored
    public string path;
    public GameObject SceneToRecord;
    public GameObject MTMobj;

    


    private GameObject recorderObject;

    private bool replayObjects;
    private bool replayHands;
    private bool replayBodyRest;
    // Start is called before the first frame update
    void Start()
    {
        recorderObject = this.gameObject;
        objMani = recorderObject.GetComponent<ObjectManipulator>();
        handMani = recorderObject.GetComponent<HandPoseManipulation>();
        playerMani = recorderObject.GetComponent<PlayerManipulator>();
        bodyRec = recorderObject.GetComponent<BodyRecorder>();
        objRec = recorderObject.GetComponent<ObjectRecorder>();
        
        
        
        
        
        recording = false;
        samplingInterval = 1 / framerate;
        frame = 0;
        recordedSequenceNr = 0;
        recordingFilesDir = Application.dataPath;
        recordingFilesDir = recordingFilesDir + "/Resources/Recordings";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r")) { ToggleRecording(); }

        if (Input.GetKeyDown("1"))//Load all the files
        {
            if (loadFromCsvFile)
            {
                //Debug.Log(recordingFilesDir);
                path = EditorUtility.OpenFolderPanel("Choose Replay Folder", recordingFilesDir,"Recording_20230102_1719");
                string[] pathParts = path.Split( "Assets/Resources/");
                path = pathParts[1];
                Debug.Log(path);

                objMani.loadFromCSVFile(path);
                handMani.loadFromCSVFile(path);
                playerMani.loadFromCSVFile(path);
            }
            else
            {
                objMani.loadFromGame();
                handMani.loadFromGame();
                playerMani.loadFromGame();
            }
        }

        if (Input.GetKeyDown("2")) //replay everything
        {
            objMani.startreplay();
            handMani.startreplay();
            playerMani.startreplay();
            Debug.Log("Replay started");
        }

        if (Input.GetKeyDown("3")) //replay single frame
        {
            
        }
        
        if (recording)
        {
            timer += Time.deltaTime;
            if (timer > samplingInterval)
            {
                if (recordObjects) {objRec.LogData();}
                if (recordBody) {bodyRec.LogData();}
                
                //frame++;
                timer = timer - samplingInterval;
            }
        }
        
        
    }
    
    void ToggleRecording()
    {
        if (recording) //stop recording
        {
            MTMobj.GetComponent<TranscriptionMaster>().transcribtionOn = false;
            recording = false;
            if (recordObjects) {objRec.StopRecording(); }
            if (recordBody){bodyRec.StopRecording();}
            Debug.Log("recording stopped");
        }
        else //start recording
        {
            folderDirectory = Directory.CreateDirectory(recordingFilesDir + "/"+"Recording"+ "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmm_ss")); // returns a DirectoryInfo object
            string recordingFolderDir = folderDirectory.ToString();
            folderDirectory = Directory.CreateDirectory(recordingFolderDir + "/"+"Sequence"+ recordedSequenceNr.ToString()); // returns a DirectoryInfo object
            string sequenceFolderDir = folderDirectory.ToString();
            
            if (recordObjects) {objRec.StartRecording(sequenceFolderDir); }
            if (recordBody) {bodyRec.StartRecording(sequenceFolderDir); }
            
            recording = true;
            if (transcribeMTM)
            {
                MTMobj.GetComponent<TranscriptionMaster>().transcribtionOn = true;
            }
            Debug.Log("recording started");
        }
    }
    
    

    void RecordNewSequenceWithoutStopping()
    {
        //stop old sequence
        if (recordObjects) {recorderObject.GetComponent<ObjectRecorder>().StopRecording(); }
        if (recordBody){recorderObject.GetComponent<BodyRecorder>().StopRecording();}
        
        //start newsequence
        string recordingFolderDir = folderDirectory.ToString();
        folderDirectory = Directory.CreateDirectory(recordingFolderDir + "/"+"Sequence"+ recordedSequenceNr.ToString()); // returns a DirectoryInfo object
        string sequenceFolderDir = folderDirectory.ToString();
        
        if (recordObjects) {recorderObject.GetComponent<ObjectRecorder>().StartRecording(sequenceFolderDir); }
        if (recordBody) {recorderObject.GetComponent<BodyRecorder>().StartRecording(sequenceFolderDir); }
    }
}
