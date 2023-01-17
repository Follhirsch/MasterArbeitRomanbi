using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRfreePluginUnity;

public class HandleScrewing : MonoBehaviour
{
    private float angle;
    private float maxAngle; 
    public float screwdistance;
    // Start is called before the first frame update
    void Start()
    {
        angle = 0f;
        maxAngle = gameObject.GetComponent<KnobCollisionHandler>().upperBound;
        screwdistance = transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        angle = gameObject.GetComponent<KnobCollisionHandler>().totalMoveDist;
        Vector3 localpositionvector = transform.localPosition;
        localpositionvector.z = screwdistance*(1-angle/maxAngle);
        transform.localPosition = localpositionvector;
    }
}
