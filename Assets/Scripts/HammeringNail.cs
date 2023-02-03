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
    public float deltaHammerhit;
    public float maxDistance = -0.0225f;
    public float timeBetweenHammering = 1f;
    private float hammerTime;

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
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(hammerhead.tag)){return;}
        Debug.Log(hammerTime + timeBetweenHammering);
        Debug.Log(Time.realtimeSinceStartup);
        if (hammerTime + timeBetweenHammering > Time.realtimeSinceStartup){return;}
        hammerTime = Time.realtimeSinceStartup;
        Debug.Log("good");
        
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
