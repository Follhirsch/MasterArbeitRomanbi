using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRfreePluginUnity;

public class HammeringNail : MonoBehaviour
{
    public GameObject nail;
    public ConstrainedMovable movable;
    public GameObject hammerhead;
    public float deltaHammerhit = 0.0225f;
    public float maxDistance = -0.0225f;
    public float timeBetweenHammering = 1f;
    private float hammerTime;
    public GameObject rec;

    // Start is called before the first frame update
    void Start()
    {
        nail = gameObject.transform.parent.gameObject;
    }

    void OnEnable()
    {
        
        nail = gameObject.transform.parent.gameObject; 
        movable = nail.GetComponent<ConstrainedMovable>();
        hammerTime = Time.realtimeSinceStartup;
        if (rec.GetComponent<RecorderMaster>().sequence != 6)
        {
            movable.movedDistance = 0;
            movable.lowerBound = -0.1f;
        }
        else
        {
            movable.movedDistance = maxDistance;
            movable.lowerBound = maxDistance;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) //replay everything
        {
            movable.movedDistance = 0;
            movable.lowerBound = -0.1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(hammerhead.tag)){return;}
        if (hammerTime + timeBetweenHammering > Time.realtimeSinceStartup){return;}
        hammerTime = Time.realtimeSinceStartup;
        

        Vector3 pos = movable.transform.position;
        float positionchange = -deltaHammerhit;
        
        if (maxDistance - movable.movedDistance > -deltaHammerhit)
        {
            positionchange = maxDistance - movable.movedDistance;
        }
        
        pos.y += positionchange;
        movable.transform.position = pos;

        movable.movedDistance += positionchange;
        movable.lowerBound = movable.movedDistance;
    }
}
