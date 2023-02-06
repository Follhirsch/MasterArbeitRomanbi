using System.Collections;
using System.Collections.Generic;
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
    public bool gotDisengaged = false;
    public bool isCrank = false;
    public int CrankSize = 0;
    public int crankAngleGrasp = 0;
    public int crankAngleRelease = 0;
    public bool isInHandRH;
    public bool isInHandLH;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "InteractableObject";
        weight = (int)gameObject.GetComponent<Rigidbody>().mass;
        isInHandRH = false;
        isInHandLH = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateValues(bool isRightHandIn,bool isGraspIn)
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
}
