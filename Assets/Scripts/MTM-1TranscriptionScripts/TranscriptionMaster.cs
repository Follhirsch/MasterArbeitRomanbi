using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TranscriptionMaster : MonoBehaviour
{
    StreamWriter csvWriter;
    public bool transcribtionOn;
    public bool transcribeHands = false;
    public bool transcribeBody = false;
    public bool transcribeFromReplay = false;
    //public bool supressNextHandMotion;

    public GameObject HandsObject;
    public GameObject PlayerObject;
    public GameObject RecorderObject;
    public GameObject TranscriptionCanvas;
    public BodyTranscription BodyMTM;

    public List<BasicMotion> MTMTranscription;

    // Start is called before the first frame update
    void Start()
    {
        MTMTranscription = new List<BasicMotion>();
        BasicMotion.initialzeDicts();
        transcribtionOn = false;
        BodyMTM = gameObject.GetComponent<BodyTranscription>();
        //supressNextHandMotion = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown("5"))
        {
            MTMTranscription.Add(new Reach(1,2,true,100));
            WriteMTMCSV("Assets/Resources/Recordings/Recording_20230211_1554_58/Sequence0");
        }
        */
    }

    public void turnTranscriptionOn()
    {
        MTMTranscription.Clear();
        transcribtionOn = true;
    }

    public void TranscribeBody()
    {
        if (!transcribtionOn) {return;}
        if(!transcribeBody){return;}
        BodyMTM.UpdateBodyTranscription();
    }

    public IEnumerator updateCanvas()
    {
        yield return new WaitForSeconds(0.2f);
        string textOutput = ""; 
        
        for (int i = Math.Max(MTMTranscription.Count-10,0); i < MTMTranscription.Count; i++)
        {
            textOutput += MTMTranscription[i].createOutputString(false) + "\n";
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
        if (!transcribtionOn) {yield break;}
        if (!transcribeHands) { yield break;}
        //if (supressNextHandMotion){yield break;}

        yield return new WaitForSeconds(1f);
        Grasp g = CalculateGrasp(isRightHand, obj, frame);
        
        if (g.differentiation != 2) // was not regrasp? -> add reach
        {
            BasicMotion rB = CalculateReach(g, frame);
            MTMTranscription.Add(rB);
        }
        MTMTranscription.Add(g);
        
        StartCoroutine( updateCanvas());
    }
    public IEnumerator CalculateReleaseTransition(bool isRightHand, GameObject obj, int frame)
    {
        if (!transcribtionOn) {yield break;}
        if (!transcribeHands) { yield break;}

        /*if (supressNextHandMotion)
        {
            supressNextHandMotion = false;
            yield break;
        }
        */
        
        yield return new WaitForSeconds(1f);
        //calculate Release
        Release rl = CalculateRelease(isRightHand, obj, frame);

        InteractableObject interactionValues = rl.m_object.GetComponent<InteractableObject>();
        if (interactionValues.isCrank)
        {
            Crank c = CalculateCrank(rl);
            MTMTranscription.Add(rl);
            yield break;
        }

        bool positioningInvolved = interactionValues.gotPositioned;
        interactionValues.gotPositioned = false;
        bool disengagingInvolved = interactionValues.gotDisengaged;

        if (disengagingInvolved)
        {
            Disengage d = CalculateDisengage(rl);
            MTMTranscription.Add(d);
        }

        
        
        Move m = CalculateMove(rl,positioningInvolved);
        
        if (m != null)
        {
            MTMTranscription.Add(m);
        }
        
        if (positioningInvolved)
        {
            Position p = CalculatePositioning(rl);
            MTMTranscription.Add(p);
            //TODO: how to takkle positioning and disengaging
        }
        
        
        MTMTranscription.Add(rl);
        
        StartCoroutine( updateCanvas());
    }
    Disengage CalculateDisengage(Release rl)
    {
        InteractableObject InteractionValues = rl.m_object.GetComponent<InteractableObject>();
        return new Disengage(rl.isRightHand, InteractionValues.disengagingforce, rl.m_object, rl.frame);
    }
    Grasp CalculateGrasp(bool isRightHand, GameObject obj, int frame)
    {
        List<Release> releasListForPossibleRegrasps = new List<Release>();
        List<Grasp> graspListOtherH = new List<Grasp>();

        foreach (BasicMotion mot in MTMTranscription) // create release List
        {
            if (mot is Release)
            {
                Release rl = mot as Release;
                if (obj.name.Equals(rl.m_object.name, StringComparison.Ordinal) && rl.isRightHand == isRightHand)
                {

                    if (frame - rl.frame < ThresholdValues.regraspAllowedFrames)
                    {
                        releasListForPossibleRegrasps.Add(rl);
                    }
                }
            }
            else if (mot is Grasp)
            {
                Grasp g = mot as Grasp;
                if (obj.name.Equals(g.m_object.name, StringComparison.Ordinal) && g.isRightHand != isRightHand)
                {
                    if (frame - g.frame < ThresholdValues.handChangeAllowedFrames)
                    {
                        graspListOtherH.Add(g);
                    }

                }
                
            }
        }


        if (releasListForPossibleRegrasps.Count > 0)
        {
            return new Grasp(isRightHand, 2, 0, obj, frame); // Regrasp
            //obj was recently grasped!
        }

        if (graspListOtherH.Count > 0)
        {
            return new Grasp(isRightHand, 3, 0, obj, frame); //Hand change
        }
        InteractableObject interactionValues = obj.GetComponent<InteractableObject>();

        bool isSmall = interactionValues.isSmall;
        if (interactionValues.isInGroup)
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

        if (!interactionValues.isFullyGrasped)
        {
            return new Grasp(isRightHand, 5, 0, obj, frame); //contact Grip
        }

        if (isSmall)
        {
            return new Grasp(isRightHand, 1, 2, obj, frame); //Difficult Grasp
        }

        if (interactionValues.isCylindrical)
        {
            return new Grasp(isRightHand, 1, 3, obj, frame); //Cylindric grasp
        }

        return new Grasp(isRightHand, 1, 1, obj, frame); //Easy Grasp
    }
    Release CalculateRelease(bool isRightHand, GameObject obj, int frame)
    {
        bool graspfound = false;
        int tempFrame = 0;
        Grasp g = null;
        foreach (BasicMotion mot in MTMTranscription)
        {
            if (mot is Grasp)
            {
                Grasp tempG = mot as Grasp;
                if (tempG.isRightHand!=isRightHand){continue;}
                if (!obj.name.Equals(tempG.m_object.name, StringComparison.Ordinal))
                {
                    continue;
                }
                if (tempG.frame > tempFrame)
                {
                    g = tempG;
                    tempFrame = g.frame;
                    graspfound = true;
                }
            }
        }

        if (!graspfound)
        {
            Debug.Log("no grasp found");
            return new Release(isRightHand, obj, 2, frame);
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
    Reach CalculateReach(Grasp g, int frame)
    {
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
    Move CalculateMove(Release rl,bool involvedPositioning)
    {
        // find frames of motion
        List<Grasp> lastGraspsThisH = new List<Grasp>();
        for (int i = 0; i < MTMTranscription.Count; i++)
        {
            if (MTMTranscription[i] is Grasp)
            {
                Grasp tempGrasp = MTMTranscription[i] as Grasp;
                if (tempGrasp.isRightHand == rl.isRightHand)
                {
                    lastGraspsThisH.Add(tempGrasp);
                }
            }
        }
        int startFrame = 0;
        if (lastGraspsThisH.Count>0)
        {
            startFrame = lastGraspsThisH.Last().frame;
        }
        //get data from recorder
        Vector3[][] recorderData;
        int column = 1;
        if (rl.isRightHand)
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
        int[] distances = DistanceClassification(CreateSinglePath(recorderData, column, startFrame, rl.frame));
        int distance = distances.Last();

        if (distance < ThresholdValues.minMoveDistThreshold)
        {
            Debug.Log("no Move due to small distance");
            return null; 
        }
        InteractableObject interactionValues = rl.m_object.GetComponent<InteractableObject>();

        int weight = interactionValues.weight;
        
        if (involvedPositioning)
        {
            return new Move(3, distance, weight,rl.isRightHand,rl.m_object,rl.frame); // precise move
        }
        //check if object is in other Hand
        if (rl.isRightHand)
        {
            if (interactionValues.isInHandLH)
            {
                return new Move(1, distance, weight, rl.isRightHand, rl.m_object, rl.frame); // easy move
            }
        }
        else
        {
            if (interactionValues.isInHandRH)
            {
                return new Move(1, distance, weight, rl.isRightHand, rl.m_object, rl.frame); // easy move
            }
        }
        

        return new Move(2, distance, weight,rl.isRightHand,rl.m_object,rl.frame); // move to approximate location
        
    }
    Crank CalculateCrank(Release rl)
    {
        InteractableObject InteractionValues = rl.m_object.GetComponent<InteractableObject>();
        return new Crank(rl.isRightHand, InteractionValues.CrankSize, InteractionValues.crankAngleGrasp,
            InteractionValues.crankAngleRelease, rl.m_object, rl.frame);
        
    }
    Position CalculatePositioning(Release rl)
    {
        InteractableObject InteractionValues = rl.m_object.GetComponent<InteractableObject>();
            
        return new Position(rl.isRightHand,InteractionValues.positionForce,InteractionValues.positioningSpecification,rl.m_object,rl.frame);
        //Position(bool isRightHandIn,int differentiationIn, int specificationIn, GameObject objectIn,int frameIn)
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

    public void WriteMTMCSV(string folderDir)
    {
        string newpath = CreateUniqueFilePath(folderDir, "MTM", ".csv");
        csvWriter = new StreamWriter(newpath);
        
        for (int i = 0; i < MTMTranscription.Count; i++)
        {
            //Debug.Log(MTMTranscription[i].createOutputString(true));
            csvWriter.WriteLine(MTMTranscription[i].createOutputString(true));
        }
        csvWriter.Close();
        AssetDatabase.Refresh();
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