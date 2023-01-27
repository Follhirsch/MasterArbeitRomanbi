using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMotion
{
    public int frame;
    public string bodyPart;

    public virtual string createOutputString()
    {
        return "";
    }

    public static void initialzeDicts()
    {
        Grasp.initializeDict();
        Reach.initializeDict();
        Move.initializeDict();
        Position.initializeDict();
        //Todo: add all dictionarrys
    }
}
