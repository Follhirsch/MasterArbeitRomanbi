using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class Release : BasicMotion
{
    public int differentiation;
    public bool isRightHand;
    public GameObject m_object;


    public Release(bool isRightHandIn,GameObject objectIn, int differentiationIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        isRightHand = isRightHandIn;
        differentiation = differentiationIn;
        m_object = objectIn;
    }
    
    public override string createOutputString()
    {
        string returnString = "RL";
        string side = "Left";
        if (isRightHand) { side = "Right"; }
        returnString += differentiation.ToString();
        returnString = side + bodyPart + ": " + returnString +", "+ m_object.name;
        return returnString;
    }
    
    
}
