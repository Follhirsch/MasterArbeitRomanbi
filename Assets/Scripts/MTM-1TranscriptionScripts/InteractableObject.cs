using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using VRfreePluginUnity;

public class InteractableObject : MonoBehaviour
{
    public int weight = 0;
    public bool isSmall = false;
    public bool isCylindrical = false;
    public bool isInGroup = false;
    public bool isAtKnownLocation = false;
    public bool gotPositioned = false;
    public int positionForce = 0;
    public int positioningSpecification = 0;
    public bool gotDisengaged = false;
    public int disengagingforce = 0;
    public bool isCrank = false;
    public int CrankSize = 0;
    public int crankAngleGrasp = 0;
    public int crankAngleRelease = 0;
    public bool isInHandRH;
    public bool isInHandLH;
    public bool isFullyGrasped = false;
    private bool isConstrainedMovable = false;
    private bool isKnob = false;
    public bool isnotParticipating = false;
    public RecorderMaster recMaster;
    public bool fakeInteractable = false;
    public string debugstring1;
    public string debugstring2;
    
    // Start is called before the first frame update
    void Start()
    {
        debugstring1 = "1/1/1/1/1/1/1/1/1/1/1/1/1";
        debugstring2 = "0/0/0/0/0/0/0/0/0/0/0/0/0";
        gameObject.tag = "InteractableObject";
        if (!isnotParticipating)
        {weight = (int)gameObject.GetComponent<Rigidbody>().mass;}
        isInHandRH = false;
        isInHandLH = false;
        isFullyGrasped = false;
        ConstrainedMovablesCollisionHandler handler1 = gameObject.GetComponent(typeof(ConstrainedMovablesCollisionHandler)) as ConstrainedMovablesCollisionHandler;
        if (handler1 != null)
        {
            isConstrainedMovable = true;
        }
        KnobCollisionHandler handler2 = gameObject.GetComponent(typeof(KnobCollisionHandler)) as KnobCollisionHandler;
        if (handler2 != null)
        {
            isKnob = true;
        }
        recMaster = GameObject.Find("Recorder/Player").GetComponent<RecorderMaster>();

}

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown("y"))
        {
            ReplayFromRecording(debugstring1);
        }
        if (Input.GetKeyDown("x"))
        {
            ReplayFromRecording(debugstring2);
        }*/
        //var handler = gameObject.GetComponent(typeof(CollisionHandler)) as MovablesCollisionHandler;
        
        if (isnotParticipating)
        {
            return;
        }
        else if (isConstrainedMovable || isCrank)
        { if (gameObject.name.Equals("VR Objects-Nail fixed")){return;}
            isFullyGrasped = gameObject.GetComponent<ConstrainedMovablesCollisionHandler>().isGrabbed;
        }
        else if (isKnob)
        {
            //TODO what am I doing with the knob collision Hanldler for grasping;
        }
        else
        {
            if (!recMaster.rePlaying)
            {
                isFullyGrasped = gameObject.GetComponent<MovablesCollisionHandler>().isGrabbed;
            }
        }
    }

    public void UpdateValues(bool isRightHandIn, bool isGraspIn)
    {
        if (isRightHandIn)
        {
            isInHandRH = isGraspIn;
        }
        else
        {
            isInHandLH = isGraspIn;
        }

        if (isCrank)
        {
            if (isGraspIn)
            {
                crankAngleGrasp = (int)gameObject.GetComponent<ConstrainedMovable>().movedDistance;
            }
            else
            {
                crankAngleRelease = (int)gameObject.GetComponent<ConstrainedMovable>().movedDistance;
            }
        }

        if (gameObject.transform.childCount > 3)
        {
            GameObject testObj = gameObject.transform.GetChild(3).gameObject;
            if (testObj.CompareTag("NailGroupTrigger"))
            {
                testObj.GetComponent<NailGroupingchange>().UpdateInteractableObject();
            }
                
        }
    }

    public void AddPositioning(int positionForceIn, int positioningSpecificationIn)
    {
        gotPositioned = true;
        positionForce = positionForceIn;
        positioningSpecification = positioningSpecificationIn;
    }

    public void RemovePositioning()
    {
        gotPositioned = false;
        positionForce = 0;
        positioningSpecification = 0;
    }

    public void AddDisengaging(int disengagingForceIn)
    {
        gotDisengaged = true;
        disengagingforce = disengagingForceIn;
    }

    public void RemoveDisengaging()
    {
        gotDisengaged = false;
        disengagingforce = 0;
    }

    public string CreateStringToRecord()
    {
        // syntax weight/isSmall/
        string returnString = "";
        returnString += weight.ToString() +"/" ;
        returnString += ((isInGroup)? "1" : "0") + "/";
        returnString += ((isAtKnownLocation)? "1" : "0") + "/"; 
        returnString += ((gotPositioned)? "1" : "0") + "/";
        returnString += positionForce.ToString()+ "/";
        returnString += positioningSpecification.ToString()+ "/";
        returnString += ((gotDisengaged)? "1" : "0") + "/";
        returnString += disengagingforce.ToString() + "/";
        returnString += crankAngleGrasp.ToString()+ "/";
        returnString += crankAngleRelease.ToString()+ "/";
        returnString += ((isInHandRH)? "1" : "0") + "/";
        returnString += ((isInHandLH)? "1" : "0") + "/";
        returnString += ((isFullyGrasped)? "1" : "0");
        return returnString;
    }

    public void ReplayFromRecording(string interactionString)
    {
        
        string[] values = interactionString.Split("/");
        if (values.Length != 13)
        {
            Debug.Log("Invalid InteractionString:" + interactionString);
            return;
        }

        bool outBool = false;

        if (!int.TryParse(values[0], out weight)){Debug.Log("error in parse1");}
        if (!ParseBool(values[1], out outBool)){Debug.Log("error in parse4");}
        else{ isInGroup = outBool;}
        if (!ParseBool(values[2], out outBool)){Debug.Log("error in parse5");}
        else{ isAtKnownLocation = outBool;}
        if (!ParseBool(values[3], out outBool)){Debug.Log("error in parse6");}
        else{ gotPositioned = outBool;}
        if(!int.TryParse(values[4], out positionForce)){Debug.Log("error in parse7");}
        if(!int.TryParse(values[5], out positioningSpecification)){Debug.Log("error in parse8");}
        if (!ParseBool(values[6], out outBool)){Debug.Log("error in parse9" +values[6]);}
        else{ gotDisengaged = outBool;}
        if(!int.TryParse(values[7], out disengagingforce)){Debug.Log("error in parse10"+values[7]);}
        if(!int.TryParse(values[8], out crankAngleGrasp)){Debug.Log("error in parse11"+values[8]);}
        if(!int.TryParse(values[9], out crankAngleRelease)){Debug.Log("error in parse12"+values[9]);}
        if (!ParseBool(values[10], out outBool)){Debug.Log("error in parse13"+values[10]);}
        else{ isInHandRH = outBool;}
        if (!ParseBool(values[11], out outBool)){Debug.Log("error in parse11 "+values[11]);}
        else{ isInHandLH = outBool;}
        if (!ParseBool(values[12], out outBool)){Debug.Log("error in parse12 " +values[12]);}
        else{ isFullyGrasped = outBool;}
    }

    private bool ParseBool(string str, out bool value)
    {
        value = new bool();
        bool returnBool = false;
        if (str.Contains("1"))
        {
            returnBool = true;
            value = true;}
        else if (str.Contains("0"))
        {
            returnBool = true;
            value = false;
        }
        return returnBool;
    }
}