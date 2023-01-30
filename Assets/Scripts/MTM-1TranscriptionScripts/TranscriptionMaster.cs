using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    IEnumerator updateCanvas()
    {
        yield return new WaitForSeconds(0.1f);
        string textOutput = "";
        for (int i = 0; i < MTMTranscription.Count; i++)
        {
            //Debug.Log(MTMTranscription[i].createOutputString());
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
        if (!transcribeHands) { yield break;}
        
        yield return new WaitForSeconds(1f);
        BasicMotion gB = CalculateGraspCode(isRightHand, obj, frame);

        BasicMotion rB = CalculateReach(gB as Grasp, frame);

        // add Motions to Table
        MTMTranscription.Add(rB);
        MTMTranscription.Add(gB);
        
        StartCoroutine( updateCanvas());
    }

    public IEnumerator CalculateReleaseTransition(bool isRightHand, GameObject obj, int frame)
    {
        if (!transcribeHands) { yield break;}
        yield return new WaitForSeconds(1f);
        //calculate Release

        MTMTranscription.Add(CalculateReleaseCode(isRightHand, obj, frame));

        //TODO:  Calculate Move and other things
        StartCoroutine( updateCanvas());
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
                    return new Grasp(isRightHand, 2, 0, obj, frame); // Regrasp
                }
                else
                {
                    return new Grasp(isRightHand, 3, 0, obj, frame); //Hand change
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

            return new Grasp(isRightHand, 4, specification, obj, frame); // Choosing Grasp
        }

        if (isSmall)
        {
            return new Grasp(isRightHand, 1, 2, obj, frame); //Difficult Grasp
        }

        if (obj.GetComponent<InteractableObject>().isCylindrical)
        {
            return new Grasp(isRightHand, 1, 3, obj, frame); //Cylindric grasp
        }

        return new Grasp(isRightHand, 1, 1, obj, frame); //Easy Grasp
    }

    BasicMotion CalculateReleaseCode(bool isRightHand, GameObject obj, int frame)
    {
        for (int i = MTMTranscription.Count - 1; i < 0; i--)
        {
            if (MTMTranscription[i] is not Grasp)
            {
                continue;
            }

            Grasp g = MTMTranscription[i] as Grasp;
            if (!obj.name.Equals(g.m_object.name, StringComparison.Ordinal))
            {
                continue;
            }

            if (g.differentiation == 5)
            {
                return new Release(isRightHand, obj, 2, frame);
            }
            else
            {
                return new Release(isRightHand, obj, 1, frame);
            }
        }

        Debug.Log("No matching Grasp Found, RL2 added");
        return new Release(isRightHand, obj, 2, frame);
    }

    BasicMotion CalculateReach(Grasp g, int frame)
    {
        if (g.differentiation == 2) // was regrasp -> no Reach motion
        {
            return null;
        }

        // find frames of motion
        List<Release> lastReleases = new List<Release>();
        for (int i = 0; i < MTMTranscription.Count; i++)
        {
            if (MTMTranscription[i] is Release)
            {
                Release tempRelease = MTMTranscription[i] as Release;
                if (tempRelease.isRightHand == g.isRightHand)
                {
                    lastReleases.Add(tempRelease);
                }
            }
        }
        int startFrame = 0;
        if (lastReleases.Count>0)
        {
            startFrame = lastReleases.Last().frame;
        }
        
            //get data from recorder
        Vector3[][] recorderData;
        int column = 1;
        if (g.isRightHand)
        {
            if (RecorderObject.GetComponent<RecorderMaster>().rePlaying)
            {
                recorderData = RecorderObject.GetComponent<HandPoseManipulation>().rPosArray;
            }
            else
            {
                recorderData = RecorderObject.GetComponent<BodyRecorder>().rPosVectors.ToArray();
            }
        }
        else
        {
            if (RecorderObject.GetComponent<RecorderMaster>().rePlaying)
            {
                recorderData = RecorderObject.GetComponent<HandPoseManipulation>().lPosArray;
            }
            else
            {
                recorderData = RecorderObject.GetComponent<BodyRecorder>().lPosVectors.ToArray();
            }
        }
        int[] distances = DistanceClassification(CreateSinglePath(recorderData, column, startFrame, frame));
        int distance = distances.Last();


        if (g.differentiation == 1 && g.specification == 2) // precise Grasp
        {
            return new Reach(4, distance, g.isRightHand,frame);
        }

        if (g.differentiation == 4)
        {
            return new Reach(3, distance, g.isRightHand,frame);
        }

        Reach rOut;
        if (g.m_object.GetComponent<InteractableObject>().isAtKnownLocation)
        {
            rOut = new Reach(1, distance, g.isRightHand,frame);
        }
        else
        {
            rOut = new Reach(2, distance, g.isRightHand,frame);
        }

        //Todo: check if moving at start or end

        return rOut;


        
    }

    int[] DistanceClassification(Vector3[] path) //TODO:basic distance should be improved
    {
        int amountOfMotions = 1;
        int[] returnArray = new int[amountOfMotions];
        returnArray[0] = (int)(((path.Last() - path[0]).magnitude)*100);//round to adequate numbers
        return returnArray;
    }

    /*Vector3[] createSinglePath(List<Vector3[]> recorderDataIn,int column,int startFrame, int endFrame)
    {
        Vector3[] outputArray = new Vector3[endFrame - startFrame + 1];
        int ii = 0;
        for (int i = startFrame; i < endFrame+1; i++)
        {
            outputArray[ii] = recorderDataIn[i][column];
            ii++;
        }
        return outputArray;
    }
    */
    Vector3[] CreateSinglePath(Vector3[][] recorderDataIn,int column,int startFrame, int endFrame)
    {
        Vector3[] outputArray = new Vector3[endFrame - startFrame + 1];
        int ii = 0;
        for (int i = startFrame; i < endFrame+1; i++)
        {
            outputArray[ii] = recorderDataIn[i][column];
            ii++;
        }
        return outputArray;
    }
    
}