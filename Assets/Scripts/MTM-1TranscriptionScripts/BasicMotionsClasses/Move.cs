using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move : BasicMotion
{
    public int differentiation = 0;
    public int distance;
    public bool movingAtStart = false;
    public bool movingAtEnd = false;
    public GameObject m_object;
    public int weight;
    public bool isRightHand;
    private string motionStr = "M";
    
    public static Dictionary<int, string> differentiationDictionary;
    public static int[] MTMdistances;

    public Move(int differentiationIn, float distanceIn, int weightIn, bool isRightHandIn,GameObject objectIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        differentiation = differentiationIn;
        distance = roundToMTMlength(distanceIn);
        weight = weightIn;
        isRightHand = isRightHandIn;
        m_object = objectIn;
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
        //if (movingAtStart) { returnString = "m" + returnString;}
        //if (movingAtEnd) { returnString += "m";}
        
        string BasicMotion = motionStr;
        string side = "";
        if (isRightHand) { side = "Right"; }
        else { side = "Left"; }
        string dist = distance.ToString();
        string diff = differentiationDictionary[differentiation];
        string weightStr = weight.ToString();
        string obj = m_object.name;
         
        if (forCSV)
        {
            return (side + bodyPart + "," + BasicMotion +","+ dist+"," + diff +","+ weightStr +","+ obj);
        }
        else
        {
            return (side + bodyPart + ": " + BasicMotion + dist + diff + weightStr +" ,"+ obj);
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
