using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoweringMotion : BasicMotion
{
    public int differentiation;

    public bool arise;

    //public GameObject interactedObj;
    public static Dictionary<int, string> differentiationDictionary;

    public LoweringMotion(bool ariseIn, int differentiationIn, int frameIn)
    {
        bodyPart = "Fullbody";
        frame = frameIn;
        differentiation = differentiationIn;
        arise = ariseIn;
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

        differentiationDictionary.Add(0, "");
        differentiationDictionary.Add(1, "B");
        differentiationDictionary.Add(2, "SIT");
        differentiationDictionary.Add(3, "KOK");
        differentiationDictionary.Add(4, "KBK");
    }
    
    public override bool compareMotion(string[] motion)
    {
        if (motion.Length != 2) { return false;}
        bool motCorrect = motion[0].Equals(differentiationDictionary[differentiation], StringComparison.Ordinal);
        string ariseStr = arise ? "1" : "0";
        bool furtherCorrect = motion[1].Equals(ariseStr, StringComparison.Ordinal);
        return (motCorrect && furtherCorrect);
    }

    public override string createOutputString(bool forCSV)
    {
        string BasicMotionStr = "";
        if (arise)
        {
            BasicMotionStr += "A";
        }

        BasicMotionStr += differentiationDictionary[differentiation];

        if (forCSV)
        {
            return (bodyPart + "," + BasicMotionStr);
        }
        else
        {
            return (bodyPart + ": " + BasicMotionStr);
        }
    }
}
