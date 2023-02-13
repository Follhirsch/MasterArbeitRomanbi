using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BodyTranscription : MonoBehaviour
{
    public bool TranscriptionOn = false;
    
    public bool possibleBend;
    public int possibleBendFrame;
    
    public bool possibleSit;
    public int possibleSitFrame;
    
    public bool possibleKneel;
    public int possibleKneelFrame;
    public bool rKneel;
    public bool lKneel;

    public List<int> possibleLegmotions;
    
    public GameObject recorder;
    private RecorderMaster recMaster;
    public GameObject initalPlayer;


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

        if (!possibleBend)
        {
            if (head.transform.position.y < ThresholdValues.possibleBendingHightIn)
            {
                possibleBend = true;
                possibleBendFrame = recMaster.frame;
            }
        }
        else //possibleBend
        {
            if (!possibleSit)
            {
                if (hips.transform.position.y < ThresholdValues.possibleSitHightIn)
                {
                    possibleSit = true;
                    possibleSitFrame = recMaster.frame;
                }
            }
            else //possibleSit
            {

                if (rightFoot.transform.eulerAngles.x < ThresholdValues.footKneelAngle)
                {
                    possibleKneel = true;
                    rKneel = true;
                }

                if (leftFoot.transform.eulerAngles.x < ThresholdValues.footKneelAngle)
                {
                    possibleKneel = true;
                    lKneel = true;
                }
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

    void stepping(int frame)
    {
        
    }

    void calculateBend()
    {
        
    }

    void endBodyMotionInterval()
    {
        
    }
    


}
