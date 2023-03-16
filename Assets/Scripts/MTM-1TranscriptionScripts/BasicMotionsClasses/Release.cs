using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class Release : BasicMotion
{
    public int differentiation;
    public bool isRightHand;
    public GameObject m_object;
    private string motinoStr = "RL";


    public Release(bool isRightHandIn,GameObject objectIn, int differentiationIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        isRightHand = isRightHandIn;
        differentiation = differentiationIn;
        m_object = objectIn;
    }
    
    public override bool compareMotion(string[] motion)
    {
        if (motion.Length != 2) { return false;}
        bool motCorrect = motion[0].Equals(motinoStr, StringComparison.Ordinal);
        bool furtherCorrect = motion[1].Equals(differentiation.ToString(), StringComparison.Ordinal);
        return (motCorrect && furtherCorrect);
    }
    
    public override string createOutputString(bool forCSV)
    {
        string BasicMotion = motinoStr;
        string side = "";
        if (isRightHand) { side = "Right"; }
        else { side = "Left"; }
        string diff = differentiation.ToString();
        string obj = m_object.name;
         
        if (forCSV)
        {
            return (side + bodyPart + "," + BasicMotion +","+ diff +","+ obj+",frame," + frame);
        }
        else
        {
            return (side + bodyPart + ": " + BasicMotion + diff +" ,"+ obj);
        }
    }
    
    
}
