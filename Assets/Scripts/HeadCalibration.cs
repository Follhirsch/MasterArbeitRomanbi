using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class HeadCalibration : MonoBehaviour
{
    public GameObject Human;

    public VRIK vrikSolver;
    // Start is called before the first frame update
    void Start()
    {
        vrikSolver = Human.GetComponent<VRIK>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c")) //calibrate Head Hight
        {
            CalibrateHeadThresholds(gameObject.transform.position.y);
            CalibrateLegLength(gameObject.transform.position.y);
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

    public void CalibrateLegLength(float neckHight)
    {
        float mlp = (neckHight - 0.5555f) / 0.9146f;
        vrikSolver.solver.rightLeg.legLengthMlp = mlp;
        vrikSolver.solver.leftLeg.legLengthMlp = mlp;
    }
}
