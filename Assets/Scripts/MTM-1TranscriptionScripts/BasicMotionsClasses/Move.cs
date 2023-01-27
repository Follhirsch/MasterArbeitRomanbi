using System.Collections;
using System.Collections.Generic;
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
    
    public static Dictionary<int, string> differentiationDictionary;

    public Move(int differentiationIn, int distanceIn, int weightIn, bool isRightHandIn,GameObject objectIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        differentiation = differentiationIn;
        distance = distanceIn;
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
    }
    
    public override string createOutputString()
    {
        string returnString = "M";
        returnString += distance.ToString() + differentiationDictionary[differentiation] + weight.ToString();
        if (movingAtStart) { returnString = "m" + returnString;}
        if (movingAtEnd) { returnString += "m";}
        returnString = bodyPart + ": " + returnString;
        
        return returnString;
    }

}
