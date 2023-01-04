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

    // Start is called before the first frame update
    void Start()
    {
        recorderObject = this.gameObject;
        Debug.Log(recorderObject);
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
                string[] pathparts = path.Split( "Assets/Resources/");
                Debug.Log(pathparts[1]);
                path = pathparts[1];
                recorderObject.GetComponent<ObjectManipulator>().loadFromCSVFile();
                //recorderObject.GetComponent<HandPoseManipulation>()
            }
            else
            {
                recorderObject.GetComponent<ObjectManipulator>().loadFromGame();
            }
        }

        if (Input.GetKeyDown("2")) //replay everything
        {
            recorderObject.GetComponent<ObjectManipulator>().startreplay();
            
        }

        if (Input.GetKeyDown("3")) //replay single frame
        {
            Debug.Log("this is not well implemented in the recorderMaster");
        }
        
        
    }
}
