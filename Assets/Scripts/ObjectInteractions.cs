using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractions : MonoBehaviour
{
    public List<GameObject> currentlyGraspedLH = new List<GameObject>();
    public List<GameObject> currentlyGraspedRH = new List<GameObject>();
    public GameObject debugObj;
    public GameObject TranscriptionDisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void addGraspedObject(GameObject graspedObj,bool isRightHand)
    {
        if (isRightHand)
        {
            currentlyGraspedRH.Add(graspedObj);
            TranscriptionDisplay.transform.GetChild(0).GetComponent<Text>().text += "\n R \n G";
        }
        else
        {
            currentlyGraspedLH.Add(graspedObj); 
            TranscriptionDisplay.transform.GetChild(1).GetComponent<Text>().text += "\n R \nG";
        }
    }
    
    public void removeGraspedObj(GameObject graspedObj,bool isRightHand)
    {
        if (isRightHand)
        {
            currentlyGraspedRH.Remove(graspedObj); 
            TranscriptionDisplay.transform.GetChild(0).GetComponent<Text>().text += "\n RL";
        }
        else
        {
            currentlyGraspedLH.Remove(graspedObj); 
            TranscriptionDisplay.transform.GetChild(1).GetComponent<Text>().text += "\n RL";
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
            removeGraspedObj(debugObj,true);
        }
    }
}
