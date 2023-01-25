using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grasp : BasicMotion
{
    public int differentiation;
    public int specification;
    public string objectName;
    public static Dictionary<int, string> specificationDictionary;

    public Grasp(string bodyPartIn,int differentiationIn, int specificationIn, string objectNameIn)
    {
        bodyPart = bodyPartIn;
        differentiation = differentiationIn;
        specification = specificationIn;
        objectName = objectNameIn;
    }
    public static void initialiceDict()
    {
        specificationDictionary.Add(0,"");
        specificationDictionary.Add(1,"A");
        specificationDictionary.Add(2,"B");
        specificationDictionary.Add(3,"C");
        specificationDictionary.Add(4,"D");
    }
    
     public override string createOutputString()
     {
         string returnString = "G";
         returnString += differentiation.ToString() + specificationDictionary[specification] + objectName;
         returnString = bodyPart + ":     " + returnString;
        return returnString;
    }
}

