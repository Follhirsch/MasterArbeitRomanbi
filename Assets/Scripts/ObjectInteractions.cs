using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRfreePluginUnity;

public class ObjectInteractions : MonoBehaviour
{
    public List<GameObject> currentlyGraspedLH = new List<GameObject>();
    public Dictionary<GameObject, bool> currentlyGraspedRH = new Dictionary<GameObject, bool>();
    public GameObject debugObj;
    public GameObject debugObj2;
    public GameObject transcriptionDisplay;
    private Dictionary<GameObject, bool> test;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void addGraspedObject(GameObject graspedObj,bool isRightHand)
    {
 
        if (isRightHand)
        {   
            currentlyGraspedRH.Add(graspedObj,true);
            //transcriptionDisplay.transform.GetChild(0).GetComponent<Text>().text += "\n R \n G";
        }
        else
        {
            currentlyGraspedLH.Add(graspedObj); 
            //transcriptionDisplay.transform.GetChild(1).GetComponent<Text>().text += "\n R \nG";
        }
    }
    
    public void removeGraspedObj(GameObject graspedObj,bool isRightHand)
    {
        if (isRightHand)
        {
            currentlyGraspedRH.Remove(graspedObj); 
            //transcriptionDisplay.transform.GetChild(0).GetComponent<Text>().text += "\n RL";
        }
        else
        {
            currentlyGraspedLH.Remove(graspedObj); 
            //transcriptionDisplay.transform.GetChild(1).GetComponent<Text>().text += "\n RL";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            addGraspedObject(debugObj,true);
        }
        if (Input.GetKeyDown("z"))
        {
            addGraspedObject(debugObj2,true);
            //removeGraspedObj(debugObj,true);
        }
        
        
        /* add later for contact grasp

        foreach(GameObject tempObj in currentlyGraspedRH.Keys)
        {
            if (currentlyGraspedRH[tempObj])
            {
                if (tempObj.transform.TryGetComponent<MovablesCollisionHandler>(
                        out MovablesCollisionHandler tempHandler))
                {
                    currentlyGraspedRH[tempObj] = tempHandler.isGrabbed;
                }
                
            }
        }
        */
    }

}
