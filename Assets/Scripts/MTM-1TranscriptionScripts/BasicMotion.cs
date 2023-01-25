using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMotion
{
    public float timestamp;
    public string bodyPart;

    public virtual string createOutputString()
    {
        return "";
    }

    public static void initialzeDicts()
    {
        Grasp.initialiceDict();
        Reach.initialiceDict();
        //add all dictionarrys
    }
}
