using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.UIElements;

public class BodyTranscription : MonoBehaviour
{
    public bool TranscriptionOn = false;
    
    public bool possibleBend = false;
    public int possibleBendFrame;
    public bool isBent = false;
    public float startTimeLoweringMotion;

    public bool possibleSit = false;
    public int possibleSitFrame;
    public bool isSitting = false;
    
    public bool possibleKneel = false;
    public int possibleKneelFrame;
    public bool rKneel = false;
    public bool lKneel = false;
    public bool isKneeling = false;
    public bool fullKneel = false;

    public List<int> possibleLegmotions;
    
    public GameObject recorder;
    private RecorderMaster recMaster;
    private BodyRecorder bodyRec;
    private PlayerManipulator playerMani;
    public GameObject initalPlayer;
    public TranscriptionMaster MTM;


    private GameObject head;
    private GameObject hips;
    private GameObject rightFoot;
    private GameObject leftFoot;

    private bool rIsStepping = false;
    private int rStepStartFrame;
    //public Vector3 rStepPosStart;
    //public Vector3 rStepForwardStart;
    private bool lIsStepping = false;
    private int lStepStartFrame;
    //public Vector3 lStepPosStart;
    //public Vector3 lStepForwardStart;
    private int rIndex = 1;
    private int lIndex = 0;

    private bool rFootMoving = false;
    private int rFootMovingStartFrame;
    private bool lFootMoving = false;
    private int lFootMovingStartFrame;


    // Start is called before the first frame update
    void Start()
    {
        TranscriptionOn = false;
        assignPlayerObjects(initalPlayer);
        recMaster = recorder.GetComponent<RecorderMaster>();
        bodyRec = recorder.GetComponent<BodyRecorder>();
        playerMani = recorder.GetComponent<PlayerManipulator>();
        MTM = gameObject.GetComponent<TranscriptionMaster>();
    }
 


    // Update is called once per frame
    public void UpdateBodyTranscription()
    {
        if (!fullKneel && !possibleKneel && !isKneeling && !possibleSit && !isSitting)
        {
            CheckStepping(recMaster.frame, recMaster.rePlaying);
        }
        
        if (head.transform.position.y > ThresholdValues.possibleBendingHightOut)
        {
            endBodyMotionInterval();
            return;
        }

        if (Time.realtimeSinceStartup > startTimeLoweringMotion + ThresholdValues.maxTimeForLoweringMtion)
        {
            LoweringMotion down = endLoweringMotion();
            if (down != null)
            {
                MTM.MTMTranscription.Add(down);
                StartCoroutine(MTM.updateCanvas());
            }
        }
        if (isSitting && !possibleKneel)
        {
            CheckFootMoving(recMaster.frame,recMaster.rePlaying);
        }

        if (!possibleBend && !isBent && !possibleSit && !isSitting && !possibleKneel && !isKneeling && !fullKneel)
        {
            //check Head for bending
            if (head.transform.position.y < ThresholdValues.possibleBendingHightIn)
            {
                possibleBend = true;
                possibleBendFrame = recMaster.frame;
                startTimeLoweringMotion = Time.realtimeSinceStartup;
            }
        }
        

        if ((!possibleSit && !isSitting && !possibleKneel && !isKneeling && !fullKneel) && (isBent || possibleBend))
        {
            //Debug.Log("fist"+(!possibleSit || !isSitting || !possibleKneel || !isKneeling || fullKneel)+"and" +(isBent || possibleBend));
            //Debug.Log("notpossiblesitting "+!possibleSit + !isSitting +!possibleKneel +!isKneeling +!fullKneel);
            //Debug.Log("isbent "+isBent +", possibleBend "+possibleBend);
            
            //check Hip for sitting
            if (hips.transform.position.y < ThresholdValues.possibleSitHightIn)
            {
                //Debug.Log("possiblesit detected");
                possibleSit = true;
                possibleSitFrame = possibleBendFrame;
                possibleBend = false;
                rIsStepping = false;
                lIsStepping = false;
                startTimeLoweringMotion = Time.realtimeSinceStartup;
            }
        }

        if ((isSitting || possibleSit)&&(!isKneeling && !possibleKneel && !fullKneel))
        {
            
            if (!rKneel && FootAngleToUp(rightFoot) > ThresholdValues.footKneelAngle)
            {
                //Debug.Log("rkneel detected");
                possibleKneel = true;
                rKneel = true;
                possibleKneelFrame = possibleSitFrame;
                possibleSit = false;
                startTimeLoweringMotion = Time.realtimeSinceStartup;

                rFootMoving = false;
                lFootMoving = false;
            }

            if (!lKneel && FootAngleToUp(leftFoot) > ThresholdValues.footKneelAngle)
            {
                //Debug.Log("lkneel detected");
                possibleKneel = true;
                lKneel = true;
                possibleKneelFrame = possibleSitFrame;
                possibleSit = false;
                startTimeLoweringMotion = Time.realtimeSinceStartup;
                
                rFootMoving = false;
                lFootMoving = false;
            }
        }

        

        if ((isKneeling || possibleKneel) && !fullKneel)
        {
            if (!rKneel)
            {
                if(FootAngleToUp(rightFoot) > ThresholdValues.footKneelAngle)
                {
                    //Debug.Log("lkneel detected");
                    rKneel = true;
                }
            }
            if (!lKneel)
            {
                if(FootAngleToUp(leftFoot) > ThresholdValues.footKneelAngle)
                {
                    //Debug.Log("lkneel detected");
                    lKneel = true;
                }
            }
            

            if (lKneel && rKneel)
            {
                //Debug.Log("fullkneel detected");
                resetIsMotions();
                resetpossibleMotinos();
                fullKneel = true;
                rKneel = false;
                lKneel = false;
                LoweringMotion fullKneelmotion = new LoweringMotion(false,4,possibleKneelFrame);
                MTM.MTMTranscription.Add(fullKneelmotion);
                StartCoroutine(MTM.updateCanvas());
            }
        }

        /*if (isKneeling || possibleKneel)
        {
            Debug.Log("rkneelangle"+rightFoot.transform.eulerAngles.x);
            Debug.Log("lkneelangle"+leftFoot.transform.eulerAngles.x);
        }*/



    }
    void assignPlayerObjects(GameObject player)
    {
        head = player.transform.GetChild(4).gameObject;
        hips = player.transform.GetChild(2).gameObject;
        rightFoot = player.transform.GetChild(1).GetChild(0).gameObject;
        leftFoot = player.transform.GetChild(0).GetChild(0).gameObject;
    }
    
    void CheckFootMoving(int frame,bool transcribeFromReplay)
    {
        int frameDelta = ThresholdValues.feetMinMovingFrames;
        if (frame<Math.Max(ThresholdValues.feetFrameDelayCompensation,frameDelta)){return;}// need at least 10 frames to create average
        
        Vector3 lFootDisp;
        Vector3 rFootDisp;
        if (transcribeFromReplay)
        {
            rFootDisp = playerMani.posArray[frame][rIndex] -
                        playerMani.posArray[frame - frameDelta][rIndex];
            lFootDisp = playerMani.posArray[frame][lIndex] -
                        playerMani.posArray[frame - frameDelta][lIndex];
        }
        else
        {
            rFootDisp = bodyRec.bodyRestPosVectors[frame][rIndex] -
                        bodyRec.bodyRestPosVectors[frame - frameDelta][rIndex];
            lFootDisp = bodyRec.bodyRestPosVectors[frame][lIndex] -
                        bodyRec.bodyRestPosVectors[frame - frameDelta][lIndex];
        }
        float rvelo = (rFootDisp.magnitude)* recMaster.framerate / frameDelta;
        float lvelo = (lFootDisp.magnitude )* recMaster.framerate / frameDelta;
        
        if (rvelo > ThresholdValues.feetVelocityIn)
        {
            rFootMoving = true;
            rFootMovingStartFrame = frame-ThresholdValues.feetFrameDelayCompensation;
        }

        if (rFootMoving)
        {
            if (rvelo < ThresholdValues.feetVelocityOut)
            {
                rFootMoving = false;
                MTM.MTMTranscription.Add(DetermineFootMotion(true,rFootMovingStartFrame,frame,transcribeFromReplay));
                StartCoroutine(MTM.updateCanvas());
            }
        }
        
        if (lvelo > ThresholdValues.feetVelocityIn)
        {
            lFootMoving = true;
            lFootMovingStartFrame = frame-ThresholdValues.feetFrameDelayCompensation;
        }

        if (lFootMoving)
        {
            if (lvelo < ThresholdValues.feetVelocityOut)
            {
                lFootMoving = false;
                MTM.MTMTranscription.Add(DetermineFootMotion(false,lFootMovingStartFrame,frame,transcribeFromReplay));
                StartCoroutine(MTM.updateCanvas());
            }
        }
    }
    FootMotion DetermineFootMotion(bool isRightfoot, int startFrame,int currentFrame,bool transcribeFromReplay)
    {
        int index = isRightfoot ? rIndex : lIndex;
        Vector3 posStart;
        Vector3 posEnd;
        //Quaternion rotStart;
        //Quaternion rotEnd;
        if (transcribeFromReplay)
        {
            posStart = playerMani.posArray[startFrame][index];
            //rotStart = playerMani.oriArray[startFrame][index];
            posEnd = playerMani.posArray[currentFrame][index];
            //rotEnd = playerMani.oriArray[currentFrame][index];
        }
        else
        {
            posStart = bodyRec.bodyRestPosVectors[startFrame][index];
            //rotStart = bodyRec.bodyRestOriQuaternions[startFrame][index];
            posEnd = bodyRec.bodyRestPosVectors[currentFrame][index];
            //rotEnd = bodyRec.bodyRestOriQuaternions[currentFrame][index];
        }

        Vector3 distanceFootMovedVector = posEnd - posStart;
        Debug.Log("foot motoinwith vector"+distanceFootMovedVector + "and distance"+ distanceFootMovedVector.magnitude);
        float distanceFootMoved = (distanceFootMovedVector.magnitude*100);

        return new FootMotion(isRightfoot,(int)distanceFootMoved,startFrame);
    }
    void CheckStepping(int frame,bool transcribeFromReplay)
    {
        int frameDelta = ThresholdValues.stepMinMovingFrames;
        if (frame < Math.Max(ThresholdValues.stepFrameDelayCompensation, frameDelta)){ return;} // need at least 10 frames to create average
        
        
        Vector3 lFootDisp;
        Vector3 rFootDisp;
        if (transcribeFromReplay)
        {
            rFootDisp = playerMani.posArray[frame][rIndex] -
                        playerMani.posArray[frame - frameDelta][rIndex];
            lFootDisp = playerMani.posArray[frame][lIndex] -
                        playerMani.posArray[frame - frameDelta][lIndex];
        }
        else
        {
            rFootDisp = bodyRec.bodyRestPosVectors[frame][rIndex] -
                        bodyRec.bodyRestPosVectors[frame - frameDelta][rIndex];
            lFootDisp = bodyRec.bodyRestPosVectors[frame][lIndex] -
                        bodyRec.bodyRestPosVectors[frame - frameDelta][lIndex];
        }

        float rvelo = (rFootDisp.magnitude )* recMaster.framerate / frameDelta;
        float lvelo = (lFootDisp.magnitude )* recMaster.framerate / frameDelta;
        

        if (rvelo > ThresholdValues.stepVelocityIn)
        {
            //Debug.Log("isstepping velo = "+rvelo);
            rIsStepping = true;
            rStepStartFrame = frame-ThresholdValues.stepFrameDelayCompensation;
        }

        if (rIsStepping)
        {
            if (rvelo < ThresholdValues.stepVelocityOut)
            {
               // Debug.Log("step ended velo = "+ rvelo);
                rIsStepping = false;
                MTM.MTMTranscription.Add(DetermineStep(true,rStepStartFrame,frame,transcribeFromReplay));
                StartCoroutine(MTM.updateCanvas());
            }
        }
        
        if (lvelo > ThresholdValues.stepVelocityIn)
        {
            lIsStepping = true;
            lStepStartFrame = frame-ThresholdValues.stepFrameDelayCompensation;
            
        }

        if (lIsStepping)
        {
            if (lvelo < ThresholdValues.stepVelocityOut)
            {
                lIsStepping = false;
                MTM.MTMTranscription.Add(DetermineStep(false,lStepStartFrame,frame,transcribeFromReplay));
                StartCoroutine(MTM.updateCanvas());
            }
        }
    }
    Step DetermineStep(bool isRightfoot, int startFrame,int currentFrame,bool transcribeFromReplay)
    {
        int index = isRightfoot ? rIndex : lIndex;
        Vector3 posStart;
        Vector3 posEnd;
        Quaternion rotStart;
        Quaternion rotEnd;
        if (transcribeFromReplay)
        {
            posStart = playerMani.posArray[startFrame][index];
            rotStart = playerMani.oriArray[startFrame][index];
            posEnd = playerMani.posArray[currentFrame][index];
            rotEnd = playerMani.oriArray[currentFrame][index];
        }
        else
        {
            posStart = bodyRec.bodyRestPosVectors[startFrame][index];
            rotStart = bodyRec.bodyRestOriQuaternions[startFrame][index];
            posEnd = bodyRec.bodyRestPosVectors[currentFrame][index];
            rotEnd = bodyRec.bodyRestOriQuaternions[currentFrame][index];
        }
        
        // determine tunring
        Vector3 forwardStart = Vector3.Normalize(Vector3.ProjectOnPlane(rotStart * Vector3.up,Vector3.up));
        Vector3 forwardEnd = Vector3.Normalize(Vector3.ProjectOnPlane(rotEnd * Vector3.up, Vector3.up));
        float angle = Math.Abs(Vector3.Angle(forwardStart, forwardEnd));
        Debug.Log("turn" + forwardStart + ""+ forwardEnd +""+ angle);

        if (angle > ThresholdValues.stepAngleTurnMin)
        {
            if (angle < ThresholdValues.stepAngleTurnMax)
            {
                return new Step(isRightfoot, 2, startFrame);
            }
            //return new Step(isRightfoot, 1, startFrame);
            
        }
        //determine if sidestep
        Vector3 changevektor = Vector3.Normalize(Vector3.ProjectOnPlane(posEnd - posStart, Vector3.up));
        float forwardPrc = Math.Abs(Vector3.Dot(changevektor, forwardStart));
        if (forwardPrc < ThresholdValues.stepSideStepMaxForewardPercentage)
        {
            return new Step(isRightfoot, 3, startFrame);
        }
        return new Step(isRightfoot, 1, startFrame);
    }
    void endBodyMotionInterval()
    {
        LoweringMotion down = endLoweringMotion();
        LoweringMotion up = ariseMotion(recMaster.frame);
        
        if (down != null)
        {
            MTM.MTMTranscription.Add(down);
         StartCoroutine(MTM.updateCanvas());
        }
        if (up != null)
        {
            MTM.MTMTranscription.Add(up);
            StartCoroutine(MTM.updateCanvas());
        }
    }
    LoweringMotion ariseMotion(int frameIn)
    {
        if (fullKneel)
        {
            resetIsMotions();
            return new LoweringMotion(true,4,frameIn);
        }

        if (isKneeling)
        {
            resetIsMotions();
            return new LoweringMotion(true,3,frameIn);
        }

        if (isSitting)
        {
            resetIsMotions();
            return new LoweringMotion(true,2,frameIn);
        }

        if (isBent)
        {
            resetIsMotions();
            return new LoweringMotion(true,1,frameIn);
        }

        return null;
    }
    LoweringMotion endLoweringMotion()
    {
        if (possibleKneel)
        {
            isKneeling = true;
            isSitting = false;
            isBent = false;
            resetpossibleMotinos();
            return new LoweringMotion(false,3,possibleKneelFrame);
        }

        if (possibleSit)
        {
            isSitting = true;
            isBent = false;
            resetpossibleMotinos();
            return new LoweringMotion(false,2,possibleSitFrame);
        }

        if (possibleBend)
        {
            isBent = true;
            resetpossibleMotinos();
            return new LoweringMotion(false, 1, possibleBendFrame);
        }

        return null;
    }
    float FootAngleToUp(GameObject foot)
    {
        Vector3 forward = foot.transform.up;
        float angleToUp = Vector3.Angle(new Vector3(0, 1, 0), forward);
        return angleToUp;
    }
    void resetpossibleMotinos()
    {
        possibleBend = false;
        possibleSit = false;
        possibleKneel = false;
        if (!isKneeling)
        {
            rKneel = false;
            lKneel = false;
        }
    }
    void resetIsMotions()
    {
        isSitting = false;
        isBent = false;
        isKneeling = false;
        fullKneel = false;
    }

    /*bool CheckFootIsMoving(int frame,Vector3[] positions)
    {
        if (frame < ThresholdValues.movingFramesForStep) { return false; }// no average before frame 10

        float averagedVelocity = 0;
        Vector3[] velo = new Vector3[ThresholdValues.movingFramesForStep];
        for (int i = 0; i < ThresholdValues.movingFramesForStep; i++)
        {
            //path[i]
        }
        

    }*/


}
