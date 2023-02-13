using System.Collections;
using System.Collections.Generic;
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
            ThresholdValues.headHightStanding = gameObject.transform.position.y;
            ThresholdValues.possibleBendingHightIn =
                ThresholdValues.headHightStanding - ThresholdValues.deltaHeadPossibleBendingIn;
            ThresholdValues.possibleBendingHightOut =
                ThresholdValues.headHightStanding - ThresholdValues.deltaHeadPossibleBendingOut;
        }
    }
}
