using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reach : BasicMotion
{
    public int differentiation = 0;
    public int distance;
    public int rotationAngle;
    public bool movingAtStart = false;
    public bool movingAtEnd = false;
    public bool isRightHand;
    private string motionStr = "R";
    
    public static Dictionary<int, string> differentiationDictionary;
    public static int[] MTMdistances;
    public static int[] MTMturnRotationAngles;

    public Reach(bool isRightHandIn,int differentiationIn, float distanceIn,float rotationIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        differentiation = differentiationIn;
        distance = RoundToMTMlength(distanceIn,MTMdistances);
        rotationAngle = RoundToMTMlength(rotationIn, MTMturnRotationAngles);
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
        MTMturnRotationAngles = new[]
            { 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180 };

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
            return (side + bodyPart + "," + BasicMotion + "," + dist + "," + diff + ",/T,S," + rotationAngle);
        }
        else
        {
            return (side + bodyPart + ": " + BasicMotion + dist + diff + " or TS" + rotationAngle);
        }
    }
    int RoundToMTMlength(float input,int[] lengths)
    {
        int ceiledValue = Mathf.CeilToInt(input);

        for (int i = 0; i < lengths.Length; i++) {
            if (ceiledValue < lengths[i])
            {
                return lengths[i];
            }
        }
        return lengths.Last();
    }
}
