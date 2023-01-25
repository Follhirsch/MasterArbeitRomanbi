using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TranscriptionMaster : MonoBehaviour
{
    public  bool transcribeHands = false;
    public bool transcribeBody = false;
    public bool transcribeFromReplay = false;

    public GameObject HandsObject;
    public GameObject PlayerObject;
    public GameObject RecorderObject;
    public GameObject TranscriptionCanvas;

    public List<string[]> outputData;//structure: [righthandMTM,lefthandMTM,bodyMTM,RightfootMTM,LeftfootMTM]
    // Start is called before the first frame update
    void Start()
    {
        outputData = new List<string[,]>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void updateCanvas()
    {
        string textOutputRH = "";
        string textOutputLH = "";
        string textOutputBody = "";
        string textOutputRF = "";
        string textOutputLF = "";
        
        for (int i = 0; i < outputData.Count; i++)
        {
            textOutputRH = outputData[i][0]+"\n";
            textOutputLH = outputData[i][1]+"\n";
            textOutputBody = outputData[i][2]+"\n";
            textOutputRF = outputData[i][3]+"\n";
            textOutputLF = outputData[i][4]+"\n";
        }

        TranscriptionCanvas.transform.GetChild(0).GetComponent<Text>().text = textOutputRH;
        TranscriptionCanvas.transform.GetChild(0).GetComponent<Text>().text = textOutputLH;

    }
}

