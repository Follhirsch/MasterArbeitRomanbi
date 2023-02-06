using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailConstrain : MonoBehaviour
{
    public GameObject TargetCollider;
    public GameObject fixedNail;
    public GameObject MTMobj;
    public GameObject Nail;
    public float reenterTimeDelay = 1f;
    public float exitTime; 
    // Start is called before the first frame update
    void Start()
    {
        Nail = gameObject.transform.parent.gameObject;
        TargetCollider = GameObject.Find("TriggerForNail");
        exitTime = Time.realtimeSinceStartup;
    }

    void OnEnable()
    {
        exitTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (exitTime+reenterTimeDelay>Time.realtimeSinceStartup){return;}
        if (!other.CompareTag(TargetCollider.tag)){return;}

        //suppress the next hand moton
        if (MTMobj.GetComponent<TranscriptionMaster>().transcribtionOn)
        {
            if (MTMobj.GetComponent<TranscriptionMaster>().transcribeHands)
            {
                MTMobj.GetComponent<ObjectInteractions>().supressNextHandMotion = true;
            }
        }//suppress next transcription
        
        //fixedNail.GetComponent<InteractableObject>().gotPositioned = true;

        Vector3 enclavePos = new Vector3(1, -0.5f, 0);

        
        //GameObject triggerNailObj = Nail.transform.GetChild(2).transform.gameObject;
        //triggerNailObj.SetActive(false);
        exitTime = exitTime * 1000;
        Nail.SetActive(false);
        fixedNail.SetActive(true);
        fixedNail.GetComponent<ConstrainedNailToMovable>().inputNail = Nail;
        
    }
}
