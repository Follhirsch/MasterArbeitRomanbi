using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPressure : BasicMotion
{
    // Start is called before the first frame update
    
    //for the csvoutput
    //: +",frame," + frame
    
    /*
     sequenceDict.Add(16, new Svars(true, true, "Everyting", new List<string[]>
        {
            new[] { "R", "B" }, new[] { "G", "1", "A" }, new[] { "R", "A" }, new[] { "G", "3", "" }, new[] { "M", "A" },
            new[] { "RL", "1" }, new[] { "M", "B" }, new[] { "RL", "1" },
            new[] { "R", "B" }, new[] { "G", "1", "C" }, new[] { "M", "C" }, new[] { "P" }, new[] { "RL", "1" },
            new [] { "R", "B" }, new [] { "G", "1", "C" }, new [] { "M", "B" },
            new [] { "G", "2", "" }, new [] { "M", "B" }, new [] { "RL", "1" },new[]{"AP"},
            new [] { "R", "B" }, new [] { "G", "5", "" }, new [] { "M", "B" }, new [] { "RL", "2" },
            new [] { "R", "C" }, new [] { "G", "4", "B" }, new [] { "M", "C" },new[] { "P" }, new [] { "RL", "1" },
            new[] { "R", "B" }, new[] { "G", "1", "A" }, new[] { "M", "B" }, new[] { "M", "B" }, new[] { "M", "B" },
            new[] { "M", "B" }, new[] { "RL", "1" },
            new[] { "R", "A" }, new[] { "G", "1", "B" }, new[] { "D" }, new[] { "M", "B" }, new[] { "RL", "1" },
            new [] { "R", "B" }, new [] { "G", "1", "A" }, new [] { "C" }, new [] { "RL", "1" },
            new[] { "R", "E" }, new[] { "R", "A" }, new[] { "G", "5",""},new[] { "M", "B" }, new[] { "RL", "2" },
            new[] { "LM" }, new[] { "FM" }, new[] { "FM" }, new[] { "LM" },
            new []{"SIT","0"},new []{"SIT","1"},new []{"SIT","0"},new []{"SIT","1"},
            new []{"Step",""},new []{"Step",""},new []{"Step",""},new []{"Step",""},new []{"Step",""},
            new []{"Step","Side"},new []{"Step","Side"},new []{"Step","Side"},new []{"Step","Side"},
            new []{"Step","Turn"},new []{"Step","Turn"},new []{"Step","Turn"},new []{"Step","Turn"},
            new []{"B","0"},new []{"B","1"},
            new []{"KOK","0"},new []{"KOK","1"},new []{"KOK","0"},new []{"KOK","1"},
            new []{"KBK","0"},new []{"KBK","1"}
        }));
     */


    public bool isRightHand;
    private string motionStr = "AP";
    public ApplyPressure(bool isRightHandIn,int frameIn)
    {
        bodyPart = "Hand";
        frame = frameIn;
        isRightHand = isRightHandIn;
    }
    public override bool compareMotion(string[] motion)
    {
        if (motion.Length != 1) { return false;}
        bool motCorrect = motion[0].Equals(motionStr, StringComparison.Ordinal);
        return (motCorrect);
    }
    public override string createOutputString(bool forCSV)
    {
        string BasicMotion = motionStr;
        string side = "";
        if (isRightHand) { side = "Right"; }
        else { side = "Left"; }

        if (forCSV)
        {
            return (side + bodyPart + "," + BasicMotion+",frame," + frame);
        }
        else
        {
            return (side + bodyPart + ": " + BasicMotion);
        }
    }
    
}
