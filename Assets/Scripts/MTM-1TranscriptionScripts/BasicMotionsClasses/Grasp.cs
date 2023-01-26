using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grasp : BasicMotion
{
    public bool isRightHand;
    public int differentiation;
    public int specification;
    public GameObject m_object;
    public static Dictionary<int, string> specificationDictionary;

    public Grasp(bool isRightHandIn,int differentiationIn, int specificationIn, GameObject objectIn)
    {
        bodyPart = "Hand";
        isRightHand = isRightHandIn;
        differentiation = differentiationIn;
        specification = specificationIn;
        m_object = objectIn;
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
         string side = "";
         if (isRightHand) { side = "Right"; }
         else { side = "Left"; }
         returnString += differentiation.ToString() + specificationDictionary[specification] + m_object.name;
         returnString = side + bodyPart + ":     " + returnString;
        return returnString;
    }
}

