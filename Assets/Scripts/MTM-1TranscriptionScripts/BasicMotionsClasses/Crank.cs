using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : BasicMotion
{
    public bool isRightHand;
    public int crankSize = 0;
    public int crankRotationAngle = 0;
    public GameObject m_object;
    
    public Crank(bool isRightHandIn,int crankSizeIn, int crankAngleGraspIn, int crankAngleReleaseIn, GameObject objectIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        isRightHand = isRightHandIn;
        crankSize = crankSizeIn;
        crankRotationAngle = crankAngleReleaseIn- crankAngleGraspIn;
        m_object = objectIn;
    }
    
    public override string createOutputString()
    {
        string returnString = "C";
        string side = "";
        if (isRightHand) { side = "Right"; }
        else { side = "Left"; }
        returnString += crankSize.ToString() +"/"+ crankRotationAngle+", "+ m_object.name;
        returnString = side + bodyPart + ": " + returnString;
        return returnString;
    }
}
