using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class RecorderMaster : MonoBehaviour
{
    public int framerate = 30;
    public bool recording = false;
    public bool recordBody;
    public bool recordObjects;
    
    public bool rePlaying = false;
    public bool loadFromCsvFile = false;
    public string recordingFilesDir;
    public string path;

    private GameObject recorderObject;

    private bool replayObjects;
    private bool replayHands;
    private bool replayBodyRest;
    // Start is called before the first frame update
    void Start()
    {
        recorderObject = this.gameObject;
        recording = false;
        recordingFilesDir = Application.dataPath;
        recordingFilesDir = recordingFilesDir + "/Resources/Recordings";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r")) { recording = true; }

        if (Input.GetKeyDown("1"))//Load all the files
        {
            if (loadFromCsvFile)
            {
                Debug.Log(recordingFilesDir);
                path = EditorUtility.OpenFolderPanel("Choose Replay Folder", recordingFilesDir,"Recording_20230102_1719");
                string[] pathParts = path.Split( "Assets/Resources/");
                path = pathParts[1];

                recorderObject.GetComponent<ObjectManipulator>().loadFromCSVFile();
                recorderObject.GetComponent<HandPoseManipulation>().loadFromCSVFile();
            }
            else
            {
                recorderObject.GetComponent<ObjectManipulator>().loadFromGame();
                recorderObject.GetComponent<HandPoseManipulation>().loadFromGame();
                recorderObject.GetComponent<PlayerManipulator>().loadFromGame();
            }
        }

        if (Input.GetKeyDown("2")) //replay everything
        {
            recorderObject.GetComponent<ObjectManipulator>().startreplay();
            recorderObject.GetComponent<HandPoseManipulation>().startreplay();
            recorderObject.GetComponent<PlayerManipulator>().startreplay();
        }

        if (Input.GetKeyDown("3")) //replay single frame
        {
            Debug.Log("this is not well implemented in the recorderMaster");
        }
        
        
    }
}
