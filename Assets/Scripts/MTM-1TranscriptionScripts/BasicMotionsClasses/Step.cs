using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : BasicMotion
{
    public int differentiation;
    public bool isRightFoot;
    string motionStr = "Step";

    //public GameObject interactedObj;
    public static Dictionary<int, string> differentiationDictionary;

    public Step(bool isRightFootIn,int differentiationIn,int frameIn)
    {
        bodyPart = "Foot";
        frame = frameIn;
        isRightFoot = isRightFootIn;
        differentiation = differentiationIn;
    }
    
    public override bool compareMotion(string[] motion)
    {
        if (motion.Length != 2) { return false;}
        bool motCorrect = motion[0].Equals(motionStr, StringComparison.Ordinal);
        bool furtherCorrect = motion[1].Equals(differentiationDictionary[differentiation], StringComparison.Ordinal);
        return (motCorrect && furtherCorrect);
    }
    public static void initializeDict()
    {
        if (differentiationDictionary == null)
        {
            differentiationDictionary = new Dictionary<int, string>();
        }
        else
        {
            differentiationDictionary.Clear();
        }

        differentiationDictionary.Add(0, "invalid");
        differentiationDictionary.Add(1, "");
        differentiationDictionary.Add(2, "Turn");
        differentiationDictionary.Add(3, "Side");
    }
    

    public override string createOutputString(bool forCSV)
    {
        string BasicMotionStr = motionStr;
        string isRightfootStr = isRightFoot ? "Right" : "Left";
        
        if (forCSV)
        {
            return (isRightfootStr + bodyPart + "," + BasicMotionStr + "," +
                    differentiationDictionary[differentiation]+",frame," + frame);
        }
        else
        {
            return (isRightfootStr + bodyPart + ": " + differentiationDictionary[differentiation] + BasicMotionStr);
        }
    }
}
