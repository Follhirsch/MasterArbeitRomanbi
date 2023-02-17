using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public int sequence = 0;
    public int frame;
    
    public ObjectManipulator objMani;
    public HandPoseManipulation handMani;
    public PlayerManipulator playerMani;
    public BodyRecorder bodyRec;
    public ObjectRecorder objRec;
    
    
    public bool rePlaying = false;
    public bool loadFromCsvFile = false;
    public string recordingFilesDir;
    private DirectoryInfo folderDir; // folder where a recording is stored
    private DirectoryInfo BaseFolderDir;
    public string path;
    public GameObject SceneToRecord;
    public GameObject MTMobj;
    private TranscriptionMaster MTMmaster;




    private GameObject recorderObject;

    private bool replayObjects;
    private bool replayHands;
    private bool replayBodyRest;
    private bool replayFolderCreated;
    // Start is called before the first frame update
    void Start()
    {
        replayFolderCreated = false;
        recorderObject = this.gameObject;
        objMani = recorderObject.GetComponent<ObjectManipulator>();
        handMani = recorderObject.GetComponent<HandPoseManipulation>();
        playerMani = recorderObject.GetComponent<PlayerManipulator>();
        bodyRec = recorderObject.GetComponent<BodyRecorder>();
        objRec = recorderObject.GetComponent<ObjectRecorder>();
        MTMmaster = MTMobj.GetComponent<TranscriptionMaster>();




        recording = false;
        samplingInterval = 1 / framerate;
        frame = 0;
        recordingFilesDir = Application.dataPath;
        recordingFilesDir = recordingFilesDir + "/Resources/Recordings";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            if (rePlaying){return;}
            ToggleRecording();
        }
        if (Input.GetKeyDown("left"))
        {
            sequence = sequence < 1 ? 0 : sequence - 1;
            MTMmaster.ChangeSequence(sequence);
        }
        if (Input.GetKeyDown("right"))
        {
            sequence = sequence > 18 ? 19 : sequence + 1;
            MTMmaster.ChangeSequence(sequence);
        }

        if (Input.GetKeyDown("1"))//Load all the files
        {
            if(recording){return;}
            if (loadFromCsvFile)
            {
                //Debug.Log(recordingFilesDir);
                path = EditorUtility.OpenFolderPanel("Choose Replay Folder", recordingFilesDir,"Recording_20230102_1719");
                string[] pathParts = path.Split( "Assets/Resources/");
                path = pathParts[1];
                folderDir = new DirectoryInfo("Assets/Resources/" + path);
                Debug.Log("fromreplayloading: "+folderDir);

                bool objLoaded = objMani.loadFromCSVFile(path);
                bool handsLoaded = handMani.loadFromCSVFile(path);
                bool playerLoaded = playerMani.loadFromCSVFile(path);

                if (!(objLoaded && handsLoaded && playerLoaded))
                {
                    Debug.Log("no file found");
                }
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
            if(recording){return;}
            Debug.Log("Replay started");
            StartCoroutine(replayEveryting());
        }

        if (Input.GetKeyDown("3")) //replay single frame
        {
            if(recording){return;}
            stopReplay();
            EnableEverything(true);
            objMani.playFrame(frame);
            playerMani.playFrame(frame);
            handMani.playFrame(frame);
        }
        if (Input.GetKeyDown("4")) 
        {
            if(recording){return;}
            loadFromCsvFile = !loadFromCsvFile;
        }
        
        
        if (recording)
        {
            timer += Time.deltaTime;
            if (timer > samplingInterval)
            {
                if (recordObjects) {objRec.LogData();}
                if (recordBody) {bodyRec.LogData();}
                MTMmaster.TranscribeBody();
                frame++;
                timer = timer - samplingInterval;
            }
        }
    }

    void ToggleRecording()
    {
        if (recording) //stop recording
        {
            if (transcribeMTM)
            {
                MTMmaster.turnTranscriptionOff(folderDir.ToString());
            }
            
            recording = false;
            if (recordObjects) {objRec.StopRecording(); }
            if (recordBody){bodyRec.StopRecording();}
            AssetDatabase.Refresh();
            Debug.Log("recording stopped");
        }
        else //start recording
        {
            frame = 0;
            if (!replayFolderCreated)
            {
                string basefolderpath = recordingFilesDir + "/" + "Recording" + "_" +
                                        System.DateTime.Now.ToString("yyyyMMdd_HHmm_ss");
                BaseFolderDir = Directory.CreateDirectory(basefolderpath);
                replayFolderCreated = true;
            }

            string recordingFolderDir = BaseFolderDir.ToString();
            string tempdir = CreateUniqueFolderPath(recordingFolderDir, ("Sequence" + sequence.ToString()));
            folderDir = Directory.CreateDirectory(tempdir); // returns a DirectoryInfo object
            string sequenceFolderDir = folderDir.ToString();

            if (recordObjects) {objRec.StartRecording(sequenceFolderDir); }
            if (recordBody) {bodyRec.StartRecording(sequenceFolderDir); }
            
            recording = true;
            if (transcribeMTM)
            {
                MTMmaster.turnTranscriptionOn();
            }
            Debug.Log("recording started");
        }
    }

    IEnumerator replayEveryting()
    {
        EnableEverything(true);
        yield return new WaitForSeconds(0.5f);
        if(transcribeMTM)
        {
            MTMmaster.transcribtionOn = true;
        }
        
        int lengthObjects = objMani.posArray.Length;
        int lengthPlayer = playerMani.posArray.Length;
        int lengthHands = handMani.lPosArray.Length;
        int maxlength = new[] { lengthObjects, lengthPlayer, lengthHands }.Max();
        
        rePlaying = true;
        for (int i = 0; i < maxlength; i++)
        {
            frame = i;
            if (i < lengthObjects ) { objMani.playFrame(i); }
            if (i < lengthPlayer) {playerMani.playFrame(i); }
            if (i < lengthHands) { handMani.playFrame(i); }
            
            
            yield return new WaitForSeconds(1 / framerate);

            if (!rePlaying)
            {
                stopReplay();
                yield break;
            }
        }
        stopReplay();
        EnableEverything(false);
    }

    void stopReplay()
    {
        if(transcribeMTM)
        {
            MTMmaster.transcribtionOn = false;
            string sequenceFolderDir = folderDir.ToString();
            MTMmaster.WriteMTMCSV(sequenceFolderDir);
            AssetDatabase.Refresh();
        }
        rePlaying = false;
        Debug.Log("replay stopped");
    }

    void EnableEverything(bool enable)
    {
        objMani.activateTriggersAndComponentsForReplay(!enable);
        playerMani.EnablePlayer(enable);
        handMani.EnableReplayHands(enable);
    }
    /*void RecordNewSequenceWithoutStopping()
    {
        //stop old sequence
        if (recordObjects) {recorderObject.GetComponent<ObjectRecorder>().StopRecording(); }
        if (recordBody){recorderObject.GetComponent<BodyRecorder>().StopRecording();}
        
        //start newsequence
        string recordingFolderDir = folderDir.ToString();
        folderDir = Directory.CreateDirectory(recordingFolderDir + "/"+"Sequence"+ sequence.ToString()); // returns a DirectoryInfo object
        string sequenceFolderDir = folderDir.ToString();
        
        if (recordObjects) {recorderObject.GetComponent<ObjectRecorder>().StartRecording(sequenceFolderDir); }
        if (recordBody) {recorderObject.GetComponent<BodyRecorder>().StartRecording(sequenceFolderDir); }
    }*/
    string CreateUniqueFolderPath(string pathIn, string nameIn)
    {
        string fullpath = pathIn + "/" + nameIn ;
        bool alreadyExists = AssetDatabase.IsValidFolder(fullpath);
        if ( alreadyExists)
        {
            fullpath = CreateUniqueFolderPath(pathIn, (nameIn + "I"));
        }
        return fullpath;
    }
    string CreateUniqueFilePath(string pathIn, string nameIn, string filetypeIn)
    {
        string fullpath = pathIn + "/" + nameIn + filetypeIn;
        DirectoryInfo tempdirASDF = new DirectoryInfo(fullpath);
        //FileInfo[] info = tempdirASDF.GetFiles(filetypeIn);
        FileInfo file = new FileInfo(fullpath);
        bool alreadyExists = file.Exists;
        if ( alreadyExists)
        {
            fullpath = CreateUniqueFilePath(pathIn, (nameIn + "I"), filetypeIn);
        }
        return fullpath;
    }



}
