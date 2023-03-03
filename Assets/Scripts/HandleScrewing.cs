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
        Debug.Log("Motion Apply pressure: AP");
        TranscriptionMaster MTMmaster = FindObjectOfType<TranscriptionMaster>();
        if (MTMmaster.transcribtionOn)
        {
            if (MTMmaster.transcribeHands)
            {
                FindObjectOfType<ObjectInteractions>().supressNextHandMotion = true;
            }
        }

        Vector3 enclavePos = new Vector3(0.2f,-0.5f,0);
        
        Vector3 newPositionHammer = hammerHead.transform.position;
        Quaternion newRotHammer = hammerHead.transform.rotation;
        
        hammerHead.transform.position = enclavePos;
        hammerHead.transform.GetChild(0).transform.position = enclavePos;
        
        newHammer.transform.rotation = newRotHammer;
        newHammer.transform.position = newPositionHammer;
    }
}
