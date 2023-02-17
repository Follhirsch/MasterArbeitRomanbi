using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMotion
{
    public int frame;
    public string bodyPart;

    public virtual string createOutputString(bool forCSV)
    {
        return "not assigned";
    }
    public virtual bool compareMotion(string[] motion)
    {
        Debug.Log("not assigned Comparison");
        return false;
    }
    

    public static void initialzeDicts()
    {
        Grasp.initializeDict();
        Reach.initializeDict();
        Move.initializeDict();
        Position.initializeDict();
        LoweringMotion.initializeDict();
        Step.initializeDict();
        FootMotion.initializeDict();
        //Todo: add all dictionarrys
    }
}
