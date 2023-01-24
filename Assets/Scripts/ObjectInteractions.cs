using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractions : MonoBehaviour
{
    public List<GameObject> currentlyGraspedLH = new List<GameObject>();
    public List<GameObject> currentlyGraspedRH = new List<GameObject>();
    public GameObject debugObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void addGraspedObject(GameObject graspedObj,bool isRightHand)
    {
        if (isRightHand) {currentlyGraspedRH.Add(graspedObj); }
        else { currentlyGraspedLH.Add(graspedObj); }
    }
    
    void removeGraspedObj(GameObject graspedObj,bool isRightHand)
    {
        if (isRightHand) {currentlyGraspedRH.Remove(graspedObj); }
        else { currentlyGraspedLH.Remove(graspedObj); }
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
