using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneTemplate;
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
    private RecorderMaster recMaster;
    private BodyRecorder bodyRec;
    private HandPoseManipulation handMani;
    public GameObject TranscriptionCanvas;
    public GameObject TranscriptionTitle;
    public BodyTranscription BodyMTM;
    private DBSCANClusterer dbscan;
    private int lastGraspFrame = 0;

    private bool releaseToShort = false;
    private int mtmScrollin = 0;

    public List<BasicMotion> MTMTranscription;
    private int sequence = 0;
    public Dictionary<int,Svars> sequenceDict = new Dictionary<int, Svars>();
    public GameObject debugObj;
    private Quaternion debugQuat;

    // Start is called before the first frame update
    void Start()
    {
        
        InitialiseSequenceDict();
        recMaster = RecorderObject.GetComponent<RecorderMaster>();
        bodyRec = RecorderObject.GetComponent<BodyRecorder>();
        handMani = RecorderObject.GetComponent<HandPoseManipulation>();
        dbscan = gameObject.GetComponent<DBSCANClusterer>();

        MTMTranscription = new List<BasicMotion>();
        BasicMotion.initialzeDicts();
        transcribtionOn = false;
        BodyMTM = gameObject.GetComponent<BodyTranscription>();
        ChangeSequence(recMaster.sequence);
        //supressNextHandMotion = false;

        /*BasicMotion testMot = new Crank(true,10,10,10,new GameObject(),1);
        Debug.Log("nromal "+testMot.compareMotion(new string[] { "C",""}));
        Debug.Log("contact "+testMot.compareMotion(new string[] { "R"}));
        Debug.Log("contact "+testMot.compareMotion(new string[] { "C" }));
        */
        /*MTMTranscription.Add(new Reach(true,2,1,35,10));
        MTMTranscription.Add(new Grasp(true,1,1,new GameObject(),10));
        MTMTranscription.Add(new Release(true,new GameObject(),2,100));
        MTMTranscription.Add(new Reach(true,1,10,40,20));*/

        StartCoroutine(updateCanvas());

        /*new string[] { "R", "B" }, new string[] { "G", "1", "A" }, new string[] { "R", "A" },
        new string[] { "G", "3", "" }, new string[] { "M", "A" }, new string[] { "RL", "1" },
        new string[] { "M", "B" }, new string[] { "RL", "1" }*/
        debugQuat = debugObj.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("up"))
        {
            mtmScrollin++;
            StartCoroutine(updateCanvas());
        }
        if (Input.GetKeyDown("down"))
        {
            mtmScrollin--;
            if (mtmScrollin < 0) { mtmScrollin = 0;}
            
            StartCoroutine(updateCanvas());
        }
        if (Input.GetKeyDown("t"))
        {
            Quaternion newQuat = debugObj.transform.rotation;
            
            
            float asdf = Vector3.Angle(debugQuat * Vector3.up, newQuat * Vector3.up);
            debugQuat = newQuat;
            Debug.Log("angle: "+ asdf);

        }
        
        
    }
    public void turnTranscriptionOn()
    {
        MTMTranscription.Clear();
        StartCoroutine(updateCanvas());
        transcribtionOn = true;
    }

    public void turnTranscriptionOff(string sequenceDirIn)
    {
        transcribtionOn = false;
        WriteMTMCSV(sequenceDirIn);
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

        bool[] correctlyDetectedMotions = compareMTMList(out int missingMotions);

        int start = Math.Max(MTMTranscription.Count-10-mtmScrollin,0);
        int end = Math.Max(MTMTranscription.Count-mtmScrollin,0);
        for (int i = Math.Max(MTMTranscription.Count-10,0); i < end; i++)
        {
            string detected = correctlyDetectedMotions[i] ? "☑" : "☐";
            textOutput += detected +"  "+MTMTranscription[i].createOutputString(false) + "\n";
        }
        textOutput += missingMotions + " Missing Motions\n";

        TranscriptionCanvas.transform.GetChild(0).GetComponent<Text>().text = textOutput;
    }

    public IEnumerator CalculateGraspTransition(bool isRightHand, GameObject obj, int frame)
    {
        if (!transcribtionOn) {yield break;}
        if (!transcribeHands) { yield break;}
        yield return new WaitForSeconds(1f);
        Debug.Log("startcalculating grasp of " + obj.name);
        if (frame == lastGraspFrame)
        {
            Debug.Log("interupted because of of lastgraspframe");
            yield break;
        }
        if(releaseToShort)
        {
            Debug.Log("interupted because of of releaseToShort");
            releaseToShort = false;
            yield break;
        }
        lastGraspFrame = frame;
        //if (supressNextHandMotion){yield break;}
        //create list of objects thet should be in this hand
        List<GameObject> specificGraspedObj = new List<GameObject>();
        foreach (BasicMotion mot in MTMTranscription) // create release List
        {
            if (mot is Grasp tempG)
            {
                if (tempG.isRightHand == isRightHand)
                {
                    specificGraspedObj.Add(tempG.m_object);
                }
            }
        }
        Debug.Log("graspedobjs");
        foreach (var tempObj in specificGraspedObj)
        {
            Debug.Log(tempObj.name);
        }
        Debug.Log("releasedobj");

        foreach (BasicMotion mot in MTMTranscription) // create release List
        {
            if (mot is Release tempRl)
            {
                foreach (var asdf in specificGraspedObj)
                {
                    if (asdf.name.Split("-")[0].Equals(tempRl.m_object.name.Split("-")[0], StringComparison.Ordinal) &&
                        tempRl.isRightHand == isRightHand)
                    {
                        specificGraspedObj.Remove(asdf);
                        Debug.Log(tempRl.m_object.name);
                        break;
                    }
                }
            }
        }
        
        
        
        
        Debug.Log("graspedobjafter");
        foreach (var tempObj in specificGraspedObj)
        {
            Debug.Log(tempObj.name);
        }

        foreach (var tempObj in specificGraspedObj)
        {
            if (obj.name.Split("-")[0].Equals(tempObj.name.Split("-")[0], StringComparison.Ordinal))
                Debug.Log("interupted because of obj already grasped");
            {yield break;}
        }


        
        Grasp g = CalculateGrasp(isRightHand, obj, frame);
        
        if (g.differentiation != 2) // was not regrasp? -> add reach
        {
            Reach[] rB = CalculateReach(g);
            MTMTranscription.AddRange(rB);
        }
        
        MTMTranscription.Add(g);
        Debug.Log("ended grasp calculation");
        StartCoroutine( updateCanvas());
    }
    public IEnumerator CalculateReleaseTransition(bool isRightHand, GameObject obj, int frame)
    {
        if (!transcribtionOn) {yield break;}
        if (!transcribeHands) { yield break;}
        
        yield return new WaitForSeconds(0.9f);
        Debug.Log("start 1 calculating rl of " + obj.name);
        if (lastGraspFrame + 5 >= frame)
        {
            Debug.Log("interupted because of lastgraspframe");
            releaseToShort = true;
            yield break;
        }
        Debug.Log("end 1 calculating rl of " + obj.name);
        yield return new WaitForSeconds(0.1f);
        Debug.Log("start 2 calculating rl of " + obj.name);
        //calculate Release
        Release rl = CalculateRelease(isRightHand, obj, frame);

        InteractableObject interactionValues = rl.m_object.GetComponent<InteractableObject>();
        if (interactionValues.isCrank)
        {
            Debug.Log("interupted because of crank");
            Crank c = CalculateCrank(rl);
            MTMTranscription.Add(c);
            MTMTranscription.Add(rl);
            StartCoroutine(updateCanvas());
            yield break;
        }

        bool positioningInvolved = interactionValues.gotPositioned;
        interactionValues.gotPositioned = false;
        bool disengagingInvolved = interactionValues.gotDisengaged;
        interactionValues.gotDisengaged = false;

        if (disengagingInvolved)
        {
            Disengage d = CalculateDisengage(rl);
            MTMTranscription.Add(d);
        }
        
        Move[] ms = CalculateMoves(rl,positioningInvolved);

        if (ms is null)
        {
            releaseToShort = true;
            Debug.Log("same frame release");
            yield break;
        }
            
        if (ms[^1] != null)
        {
            MTMTranscription.AddRange(ms);
        }
        
        if (positioningInvolved)
        {
            Position p = CalculatePositioning(rl);
            MTMTranscription.Add(p);
            //TODO: how to takkle positioning and disengaging
        }
        
        
        
        MTMTranscription.Add(rl);
        Debug.Log("end2 rl");
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
                if (obj.name.Split("-")[0].Equals(rl.m_object.name.Split("-")[0], StringComparison.Ordinal) && rl.isRightHand == isRightHand)
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
                if (obj.name.Split("-")[0].Equals(g.m_object.name.Split("-")[0], StringComparison.Ordinal) && g.isRightHand != isRightHand)
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
                //Debug.Log(obj.name.Split("-")[0]+" "+tempG.m_object.name.Split("-")[0]+" "+obj.name.Split("-")[0].Equals(tempG.m_object.name.Split("-")[0], StringComparison.Ordinal));
                
                if (!obj.name.Split("-")[0].Equals(tempG.m_object.name.Split("-")[0], StringComparison.Ordinal))
                {
                    if (obj.name.Split("-")[0].Equals("Hammer"))
                    {
                        if (tempG.m_object.name.Split("-")[0].Equals("Handle", StringComparison.Ordinal))
                        {
                            g = tempG;
                            tempFrame = g.frame;
                            graspfound = true;
                        }
                    }
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
    Reach[] CalculateReach(Grasp g)
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
            Vector3[][] recorderDataPos;
            Quaternion[][] recorderDataRot;
            int columnFinger = 7;
            int columnWrist = 1;
            if (g.isRightHand)
            {
                if (transcribeFromReplay)
                {
                    recorderDataPos = handMani.rPosArray;
                    recorderDataRot = handMani.rOriArray;
                }
                else
                {
                    recorderDataPos = bodyRec.rPosVectors.ToArray();
                    recorderDataRot = bodyRec.rOriQuaternion.ToArray();
                }
            }
            else
            {
                if (transcribeFromReplay)
                {
                    recorderDataPos = handMani.lPosArray;
                    recorderDataRot = handMani.lOriArray;
                }
                else
                {
                    recorderDataPos = bodyRec.lPosVectors.ToArray();
                    recorderDataRot = bodyRec.lOriQuaternion.ToArray();
                }
            }
            Debug.Log("motion of rH: "+g.isRightHand +"from "+startFrame +"to "+ g.frame);
            Tuple<float, float>[] distancesAndAngles = DistanceClassification(
            CreateSinglePath(recorderDataPos, columnFinger, startFrame, g.frame),
            CreateSingleRotPath(recorderDataRot, columnWrist, startFrame, g.frame));
        
        float distance = distancesAndAngles.Last().Item1;
        float rotation = distancesAndAngles.Last().Item2;
        Reach[] returnArray = new Reach[distancesAndAngles.Length];
        for (int i = 0; i < returnArray.Length-1; i++)
        {
            returnArray[i] = new Reach(g.isRightHand, 5, distancesAndAngles[i].Item1, distancesAndAngles[i].Item2,
                g.frame);
        }
        
        InteractableObject intObj = g.m_object.GetComponent<InteractableObject>();
        if (intObj.isAtKnownLocation||g.differentiation==3)
        {
            returnArray[^1] = new Reach(g.isRightHand,1, distance,rotation ,g.frame);
            return returnArray;
        }


        if (g.differentiation == 1 && g.specification == 2) // precise Grasp
        {
            returnArray[^1] = new Reach(g.isRightHand,4, distance,rotation ,g.frame);
            return returnArray;
        }

        if (g.differentiation == 4)
        {
            returnArray[^1] = new Reach(g.isRightHand, 3, distance, rotation, g.frame);
            return returnArray;
        }
        
        returnArray[^1] = new Reach(g.isRightHand,2, distance, rotation,g.frame);
        

        //Todo: check if moving at start or end

        return returnArray;
    }
    Move[] CalculateMoves(Release rl,bool involvedPositioning)
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
        Vector3[][] recorderDataPos;
        Quaternion[][] recorderDataRot;
        int columnFinger = 7;
        int columnWrist = 1;
        if (rl.isRightHand)
        {
            if (transcribeFromReplay)
            {
                recorderDataPos = handMani.rPosArray;
                recorderDataRot = handMani.rOriArray;
            }
            else
            {
                recorderDataPos = bodyRec.rPosVectors.ToArray();
                recorderDataRot = bodyRec.rOriQuaternion.ToArray();
            }
        }
        else
        {
            if (transcribeFromReplay)
            {
                recorderDataPos = handMani.lPosArray;
                recorderDataRot = handMani.lOriArray;
            }
            else
            {
                recorderDataPos = bodyRec.lPosVectors.ToArray();
                recorderDataRot = bodyRec.lOriQuaternion.ToArray();
            }
        }
        InteractableObject interactionValues = rl.m_object.GetComponent<InteractableObject>();
        int weight = interactionValues.weight;
        Tuple<float, float>[] distancesAndAngles = DistanceClassification(
            CreateSinglePath(recorderDataPos, columnFinger, startFrame, rl.frame),
            CreateSingleRotPath(recorderDataRot, columnWrist, startFrame, rl.frame));
        if (distancesAndAngles is null)
        {
            return null;}
        
        float distance = distancesAndAngles.Last().Item1;
        float rotation = distancesAndAngles.Last().Item2;
        
        Move[] returnArray = new Move[distancesAndAngles.Length];
        for (int i = 0; i < returnArray.Length-1; i++)
        {
            returnArray[i] = new Move(rl.isRightHand, 2, distancesAndAngles[i].Item1, weight,
                distancesAndAngles[i].Item2, rl.m_object, rl.frame);
        }

        /*if (distance < ThresholdValues.minMoveDistThreshold)
        {
            Debug.Log("no Move due to small distance");
            return null; 
        }*/
        
        
        if (involvedPositioning)
        {
            returnArray[^1] = new Move(rl.isRightHand,3, distance, weight,rotation,rl.m_object,rl.frame); // precise move
            return returnArray;
        }
        //check if object is in other Hand
        if (rl.isRightHand)
        {
            if (interactionValues.isInHandLH)
            {
                returnArray[^1] = new Move(rl.isRightHand,1, distance, weight, rotation , rl.m_object, rl.frame); // easy move
                return returnArray;
            }
        }
        else
        {
            if (interactionValues.isInHandRH)
            {
                returnArray[^1] = new Move(rl.isRightHand,1, distance, weight, rotation, rl.m_object, rl.frame); // easy move
                return returnArray;
            }
        }

        returnArray[^1] = new Move(rl.isRightHand,2, distance, weight,rotation,rl.m_object,rl.frame); // move to approximate location
        return returnArray;
        
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
    
    
    

    Tuple<float,float>[] DistanceClassification(Vector3[] path,Quaternion[] rotPath) //TODO:basic distance, should be improved
    {
        Tuple<int,int>[] motions= dbscan.classifyMotionFrames(path);
        if (motions is null) { return null;}
        int amountOfMotions = motions.Length;
        //Debug.Log("motions");
        //int[] framesIntervals = new[] { 0, path.Length-1};
        Tuple<float, float>[] returnArray = new Tuple<float, float>[amountOfMotions];
        for (int i = 0; i < amountOfMotions; i++)
        {
            returnArray[i] =
                new Tuple<float, float>((path[motions[i].Item2] - path[motions[i].Item1]).magnitude * 100,
                    DetermineAngleChange(rotPath[motions[i].Item2], rotPath[motions[i].Item1]));
        }
        return returnArray;//<distance,anglechange>[]
    }

    float DetermineAngleChange(Quaternion start, Quaternion end)
    {
        return Vector3.Angle(start * Vector3.up, end * Vector3.up);
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
        Vector3[] outputArray = new Vector3[endFrame - startFrame];
        int ii = startFrame;
        for (int i = 0; i < outputArray.Length; i++)
        {
            Vector3 tempvector = recorderDataIn[ii][column];
            outputArray[i] = tempvector;
            ii++;
        }
        return outputArray;
    }
    Quaternion[] CreateSingleRotPath(Quaternion[][] recorderDataIn,int column,int startFrame, int endFrame)
    {
        Quaternion[] outputArray = new Quaternion[endFrame - startFrame];
        int ii = startFrame;
        for (int i = 0; i < outputArray.Length; i++)
        {
            Quaternion tempQuat = recorderDataIn[ii][column];
            outputArray[i] = tempQuat;
            ii++;
        }
        return outputArray;
    }
    
    
    
    public void WriteMTMCSV(string folderDir)
    {
        string newpath = CreateUniqueFilePath(folderDir, "MTM", ".csv");
        csvWriter = new StreamWriter(newpath);
        csvWriter.WriteLine(sequenceDict[sequence].title);
        
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
    public void ChangeSequence(int sequenceIn)
    {
        sequence = sequenceIn;
        TranscriptionTitle.GetComponent<Text>().text = sequenceDict[sequence].title;
        transcribeBody = sequenceDict[sequence].bodyOn;
        transcribeHands = sequenceDict[sequence].handOn;
    }
    void InitialiseSequenceDict()
    {
        sequenceDict.Add(0, new Svars(false, true, "Handpass", new List<string[]>
        {
            new[] { "R", "B" }, new[] { "G", "1", "A" }, new[] { "R", "A" }, new[] { "G", "3", "" }, new[] { "M", "A" },
            new[] { "RL", "1" }, new[] { "M", "B" }, new[] { "RL", "1" }
        }));
        sequenceDict.Add(1, new Svars(false, true, "Insert Handle", new List<string[]>
        {
            new[] { "R", "B" }, new[] { "G", "1", "C" }, new[] { "M", "C" }, new[] { "P" }, new[] { "RL", "1" },
        }));
        sequenceDict.Add(2, new Svars(false, true, "Screw Handle", new List<string[]>
        {
            new [] { "R", "B" }, new [] { "G", "1", "C" }, new [] { "M", "B" },
            new [] { "G", "2", "" }, new [] { "M", "B" }, new [] { "RL", "1" }
        }));
        sequenceDict.Add(3, new Svars(false, true, "Push Hammer", new List<string[]>
        {
            new [] { "R", "B" }, new [] { "G", "5", "" }, new [] { "M", "B" }, new [] { "RL", "2" }
        }));
        sequenceDict.Add(4, new Svars(false, true, "Insert Nail", new List<string[]>
        {
            new [] { "R", "C" }, new [] { "G", "4", "B" }, new [] { "M", "C" },new[] { "P" }, new [] { "RL", "1" }
        }));
        sequenceDict.Add(5, new Svars(false, true, "Hammer Nail", new List<string[]>
        {
            new[] { "R", "B" }, new[] { "G", "1", "A" }, new[] { "M", "B" }, new[] { "M", "B" }, new[] { "M", "B" },
            new[] { "M", "B" }, new[] { "RL", "1" }
        }));
        sequenceDict.Add(6, new Svars(false, true, "Disengage Nail", new List<string[]>
        {
            new[] { "R", "A" }, new[] { "G", "1", "B" }, new[] { "D" }, new[] { "M", "B" }, new[] { "RL", "1" }
        }));
        sequenceDict.Add(7, new Svars(false, true, "Use Crank", new List<string[]>
        {
            new [] { "R", "B" }, new [] { "G", "1", "A" }, new [] { "C" }, new [] { "RL", "1" }
        }));
        sequenceDict.Add(8, new Svars(false, true, "Wave & Press Button", new List<string[]>
        {
            new[] { "R", "E" }, new[] { "R", "A" }, new[] { "G", "5" },new[] { "M", "B" }, new[] { "RL", "2" }
        }));
        sequenceDict.Add(9, new Svars(true, false, "Press Pedal", new List<string[]>
        {
            new[] { "LM" }, new[] { "FM" }, new[] { "FM" }, new[] { "LM" }
        }));
        sequenceDict.Add(10, new Svars(true, false, "Sit & Stand up", new List<string[]>
        {
            new []{"SIT","0"},new []{"SIT","1"},new []{"SIT","0"},new []{"SIT","1"}
        }));
        sequenceDict.Add(11, new Svars(true, false, "Walk To withe Table", new List<string[]>
        {
            new []{"Step",""},new []{"Step",""},new []{"Step",""},new []{"Step",""},new []{"Step",""}
        }));
        sequenceDict.Add(12, new Svars(true, false, "Sidestep", new List<string[]>
        {
            new []{"Step","Side"},new []{"Step","Side"},new []{"Step","Side"},new []{"Step","Side"}
        }));
        sequenceDict.Add(13, new Svars(true, false, "Turn", new List<string[]>
        {
            new []{"Step","Turn"},new []{"Step","Turn"},new []{"Step","Turn"},new []{"Step","Turn"}
        }));
        sequenceDict.Add(14, new Svars(true, false, "Bend & arise", new List<string[]>
        {
            new []{"B","0"},new []{"B","1"}
        }));
        sequenceDict.Add(15, new Svars(true, false, "Kneel on one knee", new List<string[]>
        {
            new []{"KOK","0"},new []{"KOK","1"},new []{"KOK","0"},new []{"KOK","1"}
        }));
        sequenceDict.Add(16, new Svars(true, false, "Kneel on both knees", new List<string[]>
        {
            new []{"KBK","0"},new []{"KBK","1"},new []{"KBK","0"},new []{"KBK","1"}
        }));
    }
    public struct Svars
    {
        public bool bodyOn;
        public bool handOn;
        public string title;
        public List<string[]> expectedMotions;

        public Svars(bool bodyOnIn, bool handOnIn, string titleIn, List<string[]> expectedMotionsIn)
        {
            bodyOn = bodyOnIn;
            handOn = handOnIn;
            title = titleIn;
            expectedMotions = expectedMotionsIn;
        }
        
    }
    bool[] compareMTMList(out int amountOfMissingMotions)
    {
        List<string[]> compList = sequenceDict[sequence].expectedMotions;
        amountOfMissingMotions = compList.Count;
        bool[] expectedMotionsFound = Enumerable.Repeat(false, compList.Count).ToArray();
        bool[] motionsFoundInExpected = Enumerable.Repeat(false, MTMTranscription.Count).ToArray();
        if (compList.Count < 1) { return motionsFoundInExpected;}
        
        for (int i = 0; i < MTMTranscription.Count; i++)
        {
            for (int j = 0; j < compList.Count; j++)
            {
                if (expectedMotionsFound[j]){continue;}

                if (MTMTranscription[i].compareMotion(compList[j]))
                {
                    expectedMotionsFound[j] = true;
                    motionsFoundInExpected[i] = true;
                    amountOfMissingMotions--;
                    break;
                }
            }
        }
        return motionsFoundInExpected;
    }

}