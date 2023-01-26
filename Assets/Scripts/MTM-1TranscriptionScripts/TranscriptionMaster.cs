using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TranscriptionMaster : MonoBehaviour
{
    public bool transcribeHands = false;
    public bool transcribeBody = false;
    public bool transcribeFromReplay = false;

    public GameObject HandsObject;
    public GameObject PlayerObject;
    public GameObject RecorderObject;
    public GameObject TranscriptionCanvas;

    public List<BasicMotion> MTMTranscription;

    // Start is called before the first frame update
    void Start()
    {
        MTMTranscription = new List<BasicMotion>();
        BasicMotion.initialzeDicts();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void updateCanvas()
    {
        string textOutput = "";
        for (int i = 0; i < MTMTranscription.Count; i++)
        {
            textOutput += MTMTranscription[i].createOutputString() + "\n";
        }

        TranscriptionCanvas.transform.GetChild(0).GetComponent<Text>().text = textOutput;

        /* old implementation
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
        */
    }

    public IEnumerator CalculateGraspTransition(bool isRightHand, GameObject obj, int frame)
    {
        yield return new WaitForSeconds(0f);
        //calculate Grasp
        MTMTranscription.Add(CalculateGraspCode(isRightHand, obj, frame));

        //TODO:  Calculate Reach
        
        updateCanvas();
    }

    public IEnumerator CalculateReleaseTransition(bool isRightHand, GameObject obj, int frame)
    {
        yield return new WaitForSeconds(0f);
        //calculate Grasp
        MTMTranscription.Add(CalculateReleaseCode(isRightHand, obj, frame)); 

        //TODO:  Calculate Move and other things
        updateCanvas();
    }

    BasicMotion CalculateGraspCode(bool isRightHand, GameObject obj, int frame)
    {
        List<Release> recentThisObjReleases = new List<Release>();

        foreach (BasicMotion mot in MTMTranscription) // create release List
        {
            if (mot is Release)
            {
                Release rl = mot as Release;
                if (!obj.name.Equals(rl.m_object.name, StringComparison.Ordinal))
                {
                }

                if (frame - rl.frame < ThresholdValues.regraspAllowedFrames)
                {
                    recentThisObjReleases.Add(rl);
                }
            }
        }

        if (recentThisObjReleases.Count > 0)
        {
            //obj was recently grasped!
            foreach (var rl in recentThisObjReleases)
            {
                if (isRightHand == rl.isRightHand)
                {
                    return new Grasp(isRightHand, 2, 0, obj); // Regrasp
                }
                else
                {
                    return new Grasp(isRightHand, 3, 0, obj); //Hand change
                }
            }
        }

        bool isSmall = obj.GetComponent<InteractableObject>().isSmall;
        if (obj.GetComponent<InteractableObject>().isInGroup)
        {
            int specification = 0;
            if (isSmall)
            {
                specification = 2;
            }
            else
            {
                specification = 1;
            }

            return new Grasp(isRightHand, 4, specification, obj); // Choosing Grasp
        }

        if (isSmall)
        {
            return new Grasp(isRightHand, 1, 2, obj); //Difficult Grasp
        }

        if (obj.GetComponent<InteractableObject>().isCylindrical)
        {
            return new Grasp(isRightHand, 1, 3, obj); //Cylindric grasp
        }

        return new Grasp(isRightHand, 1, 1, obj); //Easy Grasp
    }

    BasicMotion CalculateReleaseCode(bool isRightHand, GameObject obj, int frame)
    {
        for (int i = MTMTranscription.Count - 1; i < 0; i--)
        {
            if (MTMTranscription[i] is not Grasp) { continue; }
            Grasp g = MTMTranscription[i] as Grasp;
            if (!obj.name.Equals(g.m_object.name, StringComparison.Ordinal)) { continue; }
            if (g.differentiation == 5)
            {
                return new Release(isRightHand, obj, 2);
            }
            else
            {
                return new Release(isRightHand, obj, 1);
            }
        }
        Debug.Log("No matching Grasp Found, RL2 added");
        return new Release(isRightHand, obj, 2);
    }
}