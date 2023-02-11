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
    
    public override string createOutputString(bool forCSV)
    {
        string BasicMotion = "RL";
        string side = "";
        if (isRightHand) { side = "Right"; }
        else { side = "Left"; }
        string diff = differentiation.ToString();
        string obj = m_object.name;
         
        if (forCSV)
        {
            return (side + bodyPart + "," + BasicMotion +","+ diff +","+ obj);
        }
        else
        {
            return (side + bodyPart + ": " + BasicMotion + diff +" ,"+ obj);
        }
    }
    
    
}
