using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reach : BasicMotion
{
    public int differentiation;
    public int distance;
    public bool movingAtStart = false;
    public bool movingAtEnd = false;
    
    public static Dictionary<int, string> differentiationDictionary;

    public Reach(int differentiationIn, int distanceIn)
    {
        differentiation = differentiationIn;
        distance = distanceIn;
    }
    public static void initialiceDict()    {
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
        returnString = bodyPart + ":     " + returnString;
        
        return returnString;
    }
}
