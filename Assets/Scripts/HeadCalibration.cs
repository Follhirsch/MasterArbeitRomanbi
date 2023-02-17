using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class HeadCalibration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c")) //calibrate Head Hight
        {
            CalibrateHeadThresholds(gameObject.transform.position.y);
        }
    }

    public void CalibrateHeadThresholds(float neckHight)
    {
        ThresholdValues.headHightStanding = neckHight;
        ThresholdValues.possibleBendingHightIn =
            ThresholdValues.headHightStanding - ThresholdValues.deltaHeadPossibleBendingIn;
        ThresholdValues.possibleBendingHightOut =
            ThresholdValues.headHightStanding - ThresholdValues.deltaHeadPossibleBendingOut;
    }

 
}
