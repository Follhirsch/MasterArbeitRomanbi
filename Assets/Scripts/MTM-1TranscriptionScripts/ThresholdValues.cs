using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ThresholdValues
{
    public static int regraspAllowedFrames = 5*60;
    public static int handChangeAllowedFrames = 5*60;
    public static int movementThresholdHands = 1;

    public static float minMoveDistThreshold = 0.1f;


    public static float headHightStanding = 1.6f;
    public static float deltaHeadPossibleBendingIn = 0.2f;
    public static float deltaHeadPossibleBendingOut = 0.1f;
    public static float possibleBendingHightIn = 1.4f;
    public static float possibleBendingHightOut = 1.5f;
    
    public static float hipHightStanding = 1f;
    public static float deltaHipPossibleBendingIn = 0.2f;
    public static float possibleSitHightIn = 0.8f;

    public static float footKneelAngle = 140f;

    public static int stepMinMovingFrames = 5;
    public static int stepFrameDelayCompensation = 30;
    public static float stepVelocityIn = 0.3f;
    public static float stepVelocityOut = 0.05f;
    public static float stepAngleTurnMin = 45f;
    public static float stepAngleTurnMax = 90f;
    public static float stepSideStepMaxForewardPercentage = 0.6f;
    public static float stepMaxTime = 1f;

    public static int feetMinMovingFrames = 10;
    public static int feetFrameDelayCompensation = 30;
    public static float feetVelocityIn = 0.1f;
    public static float feetVelocityOut = 0.05f;
    


    public static float maxTimeForLoweringMtion = 3f;



}
