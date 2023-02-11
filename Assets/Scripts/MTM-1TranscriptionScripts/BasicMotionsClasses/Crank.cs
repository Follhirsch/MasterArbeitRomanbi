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
    
    public override string createOutputString(bool forCSV)
    {
        string BasicMotion = "C";
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
