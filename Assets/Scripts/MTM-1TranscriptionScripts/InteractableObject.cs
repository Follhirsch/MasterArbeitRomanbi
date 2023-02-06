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
    public bool isFullyGrasped;
    public bool isConstrainedMovable = false;
    public bool isHammerHandleScrew = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "InteractableObject";
        weight = (int)gameObject.GetComponent<Rigidbody>().mass;
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
            isHammerHandleScrew = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //var handler = gameObject.GetComponent(typeof(CollisionHandler)) as MovablesCollisionHandler;

        if (isConstrainedMovable || isCrank)
        {
            isFullyGrasped = gameObject.GetComponent<ConstrainedMovablesCollisionHandler>().isGrabbed;
        }
        else if (isHammerHandleScrew)
        {
            //TODO what am I doing with the knob collision Hanldler for grasping;
        }
        else
        {
            isFullyGrasped = gameObject.GetComponent<MovablesCollisionHandler>().isGrabbed;
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
}