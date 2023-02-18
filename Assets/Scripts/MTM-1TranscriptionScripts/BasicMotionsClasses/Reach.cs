using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reach : BasicMotion
{
    public int differentiation = 0;
    public int distance;
    public bool movingAtStart = false;
    public bool movingAtEnd = false;
    public bool isRightHand;
    private string motionStr = "R";
    
    public static Dictionary<int, string> differentiationDictionary;
    public static int[] MTMdistances;

    public Reach(int differentiationIn, float distanceIn,bool isRightHandIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        differentiation = differentiationIn;
        distance = roundToMTMlength(distanceIn);
        isRightHand = isRightHandIn;
    }
    public static void initializeDict()
    {
        if(differentiationDictionary == null)
        {
            differentiationDictionary = new Dictionary<int, string>();
        }
        else
        {
            differentiationDictionary.Clear();
        }
        differentiationDictionary.Add(0, "");
        differentiationDictionary.Add(1, "A");
        differentiationDictionary.Add(2, "B");
        differentiationDictionary.Add(3, "C");
        differentiationDictionary.Add(4, "D");
        differentiationDictionary.Add(5, "E");
        
        MTMdistances = new[]
            { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80 };
        
    }
    public override bool compareMotion(string[] motion)
    {
        if (motion.Length != 2) { return false;}
        bool motCorrect = motion[0].Equals(motionStr, StringComparison.Ordinal);
        bool furtherCorrect = motion[1].Equals(differentiationDictionary[differentiation], StringComparison.Ordinal);
        return (motCorrect && furtherCorrect);
    }

    public override string createOutputString(bool forCSV)
    {
        string BasicMotion = motionStr;
        string side = "";
        if (isRightHand) { side = "Right"; }
        else { side = "Left"; }

        string dist = distance.ToString();
        string diff = differentiationDictionary[differentiation];

        if (forCSV)
        {
            return (side + bodyPart + "," + BasicMotion +"," +dist+ "," + diff);
        }
        else
        {
            return (side + bodyPart + ": " + BasicMotion + dist + diff);
        }
    }
    int roundToMTMlength(float input)
    {
        int ceiledValue = Mathf.CeilToInt(input);

        for (int i = 1; i < MTMdistances.Length; i++) {
            if (ceiledValue < MTMdistances[i])
            {
                return MTMdistances[i];
            }
        }
        return MTMdistances.Last();
    }
}
