using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class Release : BasicMotion
{
    public int differentiation;


    public Release(string bodyPartIn, int differentiationIn)
    {
        bodyPart = bodyPartIn;
        differentiation = differentiationIn;
        
    }
    
    public override string createOutputString()
    {
        string returnString = "RL";
        returnString += differentiation.ToString();
        returnString = bodyPart + ":     " + returnString;
        return returnString;
    }
    
    
}
