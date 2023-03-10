using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grasp : BasicMotion
{
    public bool isRightHand;
    public int differentiation = 0;
    public int specification;
    public GameObject m_object;
    public static Dictionary<int, string> specificationDictionary;

    public Grasp(bool isRightHandIn,int differentiationIn, int specificationIn, GameObject objectIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        isRightHand = isRightHandIn;
        differentiation = differentiationIn;
        specification = specificationIn;
        m_object = objectIn;
    }
    public static void initializeDict()
    {
        if(specificationDictionary == null)
        {
            specificationDictionary = new Dictionary<int, string>();
        }
        else
        {
            specificationDictionary.Clear();
        }
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
         returnString += differentiation.ToString() + specificationDictionary[specification] +", "+ m_object.name;
         returnString = side + bodyPart + ": " + returnString;
        return returnString;
    }
}

