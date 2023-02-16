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
        differentiation = (specificationIn > 0.1) ? 1 : 3;
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
            return (isRightfootStr + bodyPart + ": " + differentiationDictionary[differentiation] + BasicMotionStr + specificationStr);
        }
    }
    
    
}
