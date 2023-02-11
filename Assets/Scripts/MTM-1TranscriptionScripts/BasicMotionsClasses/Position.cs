using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : BasicMotion
{
    public bool isRightHand;
    public int differentiation;
    public int specification = 0;
    public GameObject m_object;
    //public GameObject interactedObj;
    public static Dictionary<int, string> specificationDictionary;

    public Position(bool isRightHandIn,int differentiationIn, int specificationIn, GameObject objectIn,int frameIn)
    {
    bodyPart = "Hand";
    frame = frameIn;
    isRightHand = isRightHandIn;
    differentiation = differentiationIn;
    specification = specificationIn;
    m_object = objectIn;
    //interactedObj = interactedObjIn;
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
    specificationDictionary.Add(1,"S");
    specificationDictionary.Add(2,"SS");
    specificationDictionary.Add(3,"NS");
}
    
public override string createOutputString(bool forCSV)
{
    string BasicMotion = "P";
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
