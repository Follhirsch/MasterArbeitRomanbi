using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BodyTranscription : MonoBehaviour
{
    public bool TranscriptionOn = false;
    
    public bool possibleBend;
    public int possibleBendFrame;
    public bool isBent;
    public float startTimeLoweringMotion;

    public bool possibleSit;
    public int possibleSitFrame;
    public bool isSitting;
    
    public bool possibleKneel;
    public int possibleKneelFrame;
    public bool rKneel;
    public bool lKneel;
    public bool isKneeling;
    public bool fullKneel;

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
    
    // Start is called before the first frame update
    void Start()
    {
        TranscriptionOn = false;
        assignPlayerObjects(initalPlayer);
        recMaster = recorder.GetComponent<RecorderMaster>();
        bodyRec = recorder.GetComponent<BodyRecorder>();
        playerMani = recorder.GetComponent<PlayerManipulator>();
        gameObject.GetComponent<TranscriptionMaster>();
    }

    // Update is called once per frame
    void UpdateBodyTranscription(GameObject player)
    {
        if (!TranscriptionOn) {return;}

        if (head.transform.position.y > ThresholdValues.possibleBendingHightOut)
        {
            endBodyMotionInterval();
            return;
        }

        if (Time.realtimeSinceStartup > startTimeLoweringMotion + ThresholdValues.maxTimeForLoweringMtion)
        {
        }

        if (!possibleBend || !isBent || !possibleSit || !isSitting || !possibleKneel || !isKneeling || fullKneel)
        {
            //check Head for bending
            if (head.transform.position.y < ThresholdValues.possibleBendingHightIn)
            {
                possibleBend = true;
                possibleBendFrame = recMaster.frame;
                startTimeLoweringMotion = Time.realtimeSinceStartup;
            }
        }

        if ((!possibleSit || !isSitting || !possibleKneel || !isKneeling || fullKneel) && (isBent || possibleBend))
        {
            //check Hip for sitting
            if (hips.transform.position.y < ThresholdValues.possibleSitHightIn)
            {
                possibleSit = true;
                possibleSitFrame = possibleBendFrame;
                possibleBend = false;
                startTimeLoweringMotion = Time.realtimeSinceStartup;
            }
        }

        if ((isSitting||possibleSit)&&(!isKneeling||!possibleKneel||!fullKneel))
        {
            if (!rKneel && rightFoot.transform.eulerAngles.x < ThresholdValues.footKneelAngle)
            {
                possibleKneel = true;
                rKneel = true;
                possibleKneelFrame = possibleSitFrame;
                possibleSit = false;
                startTimeLoweringMotion = Time.realtimeSinceStartup;
            }

            if (!lKneel && leftFoot.transform.eulerAngles.x < ThresholdValues.footKneelAngle)
            {
                possibleKneel = true;
                lKneel = true;
                possibleKneelFrame = possibleSitFrame;
                possibleSit = false;
                startTimeLoweringMotion = Time.realtimeSinceStartup;
            }
        }

        if ((isKneeling || possibleKneel) && !fullKneel)
        {
            if (!rKneel && rightFoot.transform.eulerAngles.x < ThresholdValues.footKneelAngle)
            {
                rKneel = false;
                
            }

            if (!lKneel && leftFoot.transform.eulerAngles.x < ThresholdValues.footKneelAngle)
            {
                lKneel = false;
            }

            if (lKneel && rKneel)
            {
                resetIsMotions();
                resetpossibleMotinos();
                fullKneel = true;
                LoweringMotion fullKneelmotion = new LoweringMotion(false,4,possibleKneelFrame);
                MTM.MTMTranscription.Add(fullKneelmotion);
                StartCoroutine(MTM.updateCanvas());
            }
        }



    }
    void assignPlayerObjects(GameObject player)
    {
        head = player.transform.GetChild(4).gameObject;
        hips = player.transform.GetChild(2).gameObject;
        rightFoot = player.transform.GetChild(1).gameObject;
        leftFoot = player.transform.GetChild(0).gameObject;
    }

    void stepping(int frame,bool transcribeFromReplay)
    {
        Vector3[] lFoot;
        Vector3[] rFoot;
        if (transcribeFromReplay)
        {
            //playerMani.posArray
        }
        else
        {
            
        }
        // arry values of lfoot =0 and right foot =1

    }

    void endBodyMotionInterval()
    {
        LoweringMotion down = endLoweringMotion();
        LoweringMotion up = ariseMotion(recMaster.frame);
        
        if (down is not null)
        {
         MTM.MTMTranscription.Add(down);
         StartCoroutine(MTM.updateCanvas());
        }
        if (up is not null)
        {
            MTM.MTMTranscription.Add(up);
            Debug.Log("error, no up motion captured");
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
