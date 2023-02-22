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
    GameObject hammerHead;

    private bool triggerOn = true;
    // Start is called before the first frame update
    void Start()
    {
        angle = 0f;
        maxAngle = gameObject.GetComponent<KnobCollisionHandler>().upperBound;
        screwdistance = transform.localPosition.z;
        hammerHead = gameObject.transform.parent.transform.gameObject;
        triggerOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            SwapHammers();
        }
        if (Input.GetKeyDown("h"))
        {
            Debug.Log(hammerHead.transform.position);
        }
        

        angle = gameObject.GetComponent<KnobCollisionHandler>().totalMoveDist;
        Vector3 localpositionvector = transform.localPosition;
        if (angle >= maxAngle)
        {
            gameObject.GetComponent<KnobCollisionHandler>().upperBound = maxAngle+1;
            maxAngle = gameObject.GetComponent<KnobCollisionHandler>().upperBound;
            SwapHammers();
        }
        localpositionvector.z = screwdistance*(1-angle/maxAngle);
        transform.localPosition = localpositionvector;
    }

    void SwapHammers()
    {
        Debug.Log("Handle fully screwed in");
        Debug.Log(newHammer.transform.parent.transform.position);
            
        Vector3 enclavePos = new Vector3(0.2f,-0.5f,0);//(newHammer.transform.position);
        /*Vector3 positioning1 = newHammer.transform.position;
        Vector3 positioning2 = newHammer.transform.GetChild(0).transform.position;
        Vector3 deltaPos = positioning2 - positioning1;
        Debug.Log(positioning1);*/

        Vector3 newPositionHammer = hammerHead.transform.position;
        Quaternion newRotHammer = hammerHead.transform.rotation;
        
        
        /*Vector3 newPositionHammerHead = hammer.transform.GetChild(0).transform.position;
        Quaternion newRotHammerHead = hammer.transform.GetChild(0).transform.rotation;
        Vector3 newPositionHammerHandle = hammer.transform.GetChild(0).transform.position;
        Quaternion newRotHammerHandle = hammer.transform.GetChild(0).transform.rotation;*/
        //hammer.GetComponent<MovablesCollisionHandler>().enabled = false;
        hammerHead.transform.position = enclavePos;
        hammerHead.transform.GetChild(0).transform.position = enclavePos;
        
        newHammer.transform.rotation = newRotHammer;
        newHammer.transform.position = newPositionHammer;
        Debug.Log(newHammer.transform.position);
        Debug.Log(newHammer.transform.GetChild(0).transform.position);
        

        /*newHammer.transform.GetChild(0).transform.rotation = newRotHammer;
        newHammer.transform.GetChild(1).transform.position = newPositionHammerHandle;
        newHammer.transform.GetChild(1).transform.rotation = newRotHammerHandle;*/
    }
}
