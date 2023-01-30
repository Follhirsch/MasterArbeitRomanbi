using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRfreePluginUnity;

public class HandleScrewing : MonoBehaviour
{
    private float angle;
    private float maxAngle; 
    public float screwdistance;
    public GameObject newHammer;
    GameObject hammer;
    // Start is called before the first frame update
    void Start()
    {
        angle = 0f;
        maxAngle = gameObject.GetComponent<KnobCollisionHandler>().upperBound;
        screwdistance = transform.localPosition.z;
        hammer = gameObject.transform.parent.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        angle = gameObject.GetComponent<KnobCollisionHandler>().totalMoveDist;
        Vector3 localpositionvector = transform.localPosition;
        if (angle == maxAngle)
        {
            Debug.Log("Handle fully screwed in");
            
            Vector3 enclavePos = new Vector3(0.2f,-0.5f,0);//(newHammer.transform.position);
            Vector3 newPositionHammer = hammer.transform.position;
            Quaternion newRotHammer = hammer.transform.rotation;
            Vector3 newPositionHammerHead = hammer.transform.GetChild(0).transform.position;
            Quaternion newRotHammerHead = hammer.transform.GetChild(0).transform.rotation;
            Vector3 newPositionHammerHandle = hammer.transform.GetChild(0).transform.GetChild(0).transform.position;
            Quaternion newRotHammerHandle = hammer.transform.GetChild(0).transform.GetChild(0).transform.rotation;
            //hammer.GetComponent<MovablesCollisionHandler>().enabled = false;
            hammer.SetActive(false);

            hammer.transform.position = enclavePos;
        
            hammer.transform.position = enclavePos;
            newHammer.transform.position = newPositionHammer;
            newHammer.transform.rotation = newRotHammer;
            newHammer.transform.GetChild(0).transform.position = newPositionHammerHead;
            newHammer.transform.GetChild(0).transform.rotation = newRotHammerHead;
            newHammer.transform.GetChild(1).transform.position = newPositionHammerHandle;
            newHammer.transform.GetChild(1).transform.rotation = newRotHammerHandle;

        }
        localpositionvector.z = screwdistance*(1-angle/maxAngle);
        transform.localPosition = localpositionvector;
        newHammer.SetActive(true);
    }
}
