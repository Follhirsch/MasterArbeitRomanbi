using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disengage : BasicMotion
{
    public bool isRightHand;
    public int differentiation;
    public GameObject m_object;
    //public GameObject interactedObj;

    public Disengage(bool isRightHandIn,int differentiationIn, GameObject objectIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        isRightHand = isRightHandIn;
        differentiation = differentiationIn;
        m_object = objectIn;
        //interactedObj = interactedObjIn;
    }

    public override string createOutputString(bool forCSV)
    {
        string BasicMotion = "D";
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
            return (side + bodyPart + ": " + BasicMotion + diff + " ,"+ obj);
        }
    }
    
    
}
