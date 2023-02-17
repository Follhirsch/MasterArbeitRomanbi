using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grasp : BasicMotion
{
    public bool isRightHand;
    public int differentiation = 0;
    public int specification;
    public GameObject m_object;
    private string motionStr = "G";
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
    
    public override bool compareMotion(string[] motion)
    {
        if (motion.Length != 3) { return false;}
        bool motCorrect = motion[0].Equals(motionStr, StringComparison.Ordinal);
        bool furtherCorrect = motion[1].Equals(differentiation.ToString(), StringComparison.Ordinal);
        bool furtherCorrect2 = motion[2].Equals(specificationDictionary[specification], StringComparison.Ordinal);
        return (motCorrect && furtherCorrect && furtherCorrect2);
    }
    
     public override string createOutputString(bool forCSV)
     {
         string BasicMotion = motionStr;
         string side = "";
         if (isRightHand) { side = "Right"; }
         else { side = "Left"; }
         string diff = differentiation.ToString();
         string spec = specificationDictionary[specification];
         string obj = m_object.name;
         
         if (forCSV)
         {
             return (side + bodyPart + "," + BasicMotion +","+ diff +","+ spec +","+ obj);
         }
         else
         {
             return (side + bodyPart + ": " + BasicMotion + diff + spec +" ,"+ obj);
         }
     }
}

