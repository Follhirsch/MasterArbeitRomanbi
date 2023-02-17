using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMotion : BasicMotion
{
    public int differentiation;
    public bool isRightFoot;
    public int specification;
    
    public static Dictionary<int, string> differentiationDictionary;
    
    public FootMotion(bool isRightFootIn,int specificationIn,int frameIn)
    {
        bodyPart = "Foot";
        frame = frameIn;
        isRightFoot = isRightFootIn;
        differentiation = (specificationIn > 10) ? 3 : 1;
        specification = specificationIn;
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
        differentiationDictionary.Add(1, "FM");
        differentiationDictionary.Add(2, "FMP");
        differentiationDictionary.Add(3, "LM");
    }
    
    public override bool compareMotion(string[] motion)
    {
        if (motion.Length != 1) { return false;}
        bool motCorrect = motion[0].Equals(differentiationDictionary[differentiation], StringComparison.Ordinal);
        return (motCorrect);
    }
    public override string createOutputString(bool forCSV)
    {
        string BasicMotionStr = differentiationDictionary[differentiation];
        string isRightfootStr = isRightFoot ? "Right" : "Left";
        string specificationStr = (differentiation == 3) ? specification.ToString() : "";
        
        if (forCSV)
        {
            return (isRightfootStr + bodyPart + "," + BasicMotionStr + "," +
                    specificationStr);
        }
        else
        {
            return (isRightfootStr + bodyPart + ": " + BasicMotionStr + specificationStr);
        }
    }
    
    
}
