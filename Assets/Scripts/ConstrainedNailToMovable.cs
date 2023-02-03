using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRfreePluginUnity;

public class ConstrainedNailToMovable : MonoBehaviour
{
    public GameObject inputNail;
    public GameObject fixedNail;
    private Vector3 startPos;

    public ConstrainedMovable fixedNailMovableHandler;
    // Start is called before the first frame update
    void Start()
    {
        fixedNail = gameObject;
        fixedNailMovableHandler = fixedNail.GetComponent<ConstrainedMovable>();
        startPos = fixedNail.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (fixedNailMovableHandler.movedDistance == fixedNailMovableHandler.upperBound)
        {
            Vector3 pos = fixedNail.transform.position;
            Quaternion ori = fixedNail.transform.rotation;
            fixedNail.transform.position = startPos;
            fixedNail.GetComponent<ConstrainedMovable>().movedDistance = 0;
            fixedNail.SetActive(false);
            
            inputNail.SetActive(true);
            inputNail.transform.position = pos;
            inputNail.transform.rotation = ori;
            inputNail.transform.GetChild(2).GetComponent<NailConstrain>().exitTime = Time.realtimeSinceStartup;
            //Debug.Log(inputNail.GetComponent<NailConstrain>().exitTime);
        }
        
    }
    
}
