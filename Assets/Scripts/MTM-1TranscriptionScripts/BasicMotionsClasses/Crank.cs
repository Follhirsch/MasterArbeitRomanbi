using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : BasicMotion
{
    public bool isRightHand;
    public int crankSize = 0;
    public int crankRotationAngle = 0;
    public GameObject m_object;
    private string motionStr = "C";
    
    public Crank(bool isRightHandIn,int crankSizeIn, int crankAngleGraspIn, int crankAngleReleaseIn, GameObject objectIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        isRightHand = isRightHandIn;
        crankSize = crankSizeIn;
        crankRotationAngle = crankAngleReleaseIn- crankAngleGraspIn;
        m_object = objectIn;
    }
    
    public override bool compareMotion(string[] motion)
    {
        if (motion.Length != 1) { return false;}
        bool motCorrect = motion[0].Equals(motionStr, StringComparison.Ordinal);
        return (motCorrect);
    }
    
    public override string createOutputString(bool forCSV)
    {
        string BasicMotion = motionStr;
        string side = "";
        if (isRightHand) { side = "Right"; }
        else { side = "Left"; }
        string diff = crankSize.ToString();
        string spec = crankRotationAngle.ToString();
        string obj = m_object.name;
         
        if (forCSV)
        {
            return (side + bodyPart + "," + BasicMotion +","+ diff +","+ spec +","+ obj);
        }
        else
        {
            return (side + bodyPart + ": " + BasicMotion + diff +"/"+ spec +" ,"+ obj);
        }
    }
}
