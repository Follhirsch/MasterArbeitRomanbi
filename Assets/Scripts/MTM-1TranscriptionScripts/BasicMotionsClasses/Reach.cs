using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reach : BasicMotion
{
    public int differentiation = 0;
    public int distance;
    public bool movingAtStart = false;
    public bool movingAtEnd = false;
    public bool isRightHand;
    
    public static Dictionary<int, string> differentiationDictionary;

    public Reach(int differentiationIn, int distanceIn,bool isRightHandIn)
    {
        bodyPart = "Hand";
        differentiation = differentiationIn;
        distance = distanceIn;
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
    }

    public override string createOutputString()
    {
        string returnString = "R";
        returnString += distance.ToString() + differentiationDictionary[differentiation];
        if (movingAtStart) { returnString = "m" + returnString;}
        if (movingAtEnd) { returnString += "m";}
        returnString = bodyPart + ": " + returnString;
        
        return returnString;
    }
}
