using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipCalibration : MonoBehaviour
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
            ThresholdValues.hipHightStanding = gameObject.transform.position.y;
            ThresholdValues.possibleSitHightIn =
                ThresholdValues.hipHightStanding - ThresholdValues.deltaHipPossibleBendingIn;
        }
        
    }
}
