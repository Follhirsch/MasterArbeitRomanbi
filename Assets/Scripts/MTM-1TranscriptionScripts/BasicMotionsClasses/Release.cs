using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class Release : BasicMotion
{
    public int differentiation;
    public bool isRightHand;
    public GameObject m_object;


    public Release(bool isRightHandIn,GameObject objectIn, int differentiationIn)
    {
        bodyPart = "Hand";
        isRightHand = isRightHandIn;
        differentiation = differentiationIn;
        m_object = objectIn;
    }
    
    public override string createOutputString()
    {
        string returnString = "RL";
        string side = "";
        if (isRightHand) { side = "Right"; }
        else { side = "Left"; }
        returnString += differentiation.ToString();
        returnString = side + bodyPart + ":     " + returnString + m_object.name;
        return returnString;
    }
    
    
}
