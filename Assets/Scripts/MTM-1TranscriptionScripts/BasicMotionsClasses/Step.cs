using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : BasicMotion
{
    public int differentiation;
    public bool isRightFoot;

    //public GameObject interactedObj;
    public static Dictionary<int, string> differentiationDictionary;

    public Step(bool isRightFootIn,int differentiationIn,int frameIn)
    {
        bodyPart = "Foot";
        frame = frameIn;
        isRightFoot = isRightFootIn;
        differentiation = differentiationIn;
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
        string BasicMotionStr = "Step";
        string isRightfootStr = isRightFoot ? "Right" : "Left";
        
        if (forCSV)
        {
            return (isRightfootStr + bodyPart + "," + BasicMotionStr + "," +
                    differentiationDictionary[differentiation]);
        }
        else
        {
            return (isRightfootStr + bodyPart + ": " + differentiationDictionary[differentiation] + BasicMotionStr);
        }
    }
}
