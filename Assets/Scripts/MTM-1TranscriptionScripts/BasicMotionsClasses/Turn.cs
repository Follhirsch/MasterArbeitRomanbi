using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : BasicMotion
{
    public int differentiation = 0;
    public int angle;
    public GameObject m_object;
    public bool isRightHand;
    
    public static Dictionary<int, string> differentiationDictionary;

    public Turn(int differentiationIn, int angleIn, bool isRightHandIn,GameObject objectIn)
    {
        bodyPart = "Hand";
        differentiation = differentiationIn;
        angle = angleIn;
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
        string returnString = "T";
        returnString +=  differentiationDictionary[differentiation] + angle.ToString();
        returnString = bodyPart + ": " + returnString;
        
        return returnString;
    }
}
