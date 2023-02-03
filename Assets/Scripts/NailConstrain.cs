using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailConstrain : MonoBehaviour
{
    public GameObject TargetCollider;
    public GameObject fixedNail;
    public GameObject MTMobj;
    GameObject Nail;
    // Start is called before the first frame update
    void Start()
    {
        Nail = gameObject.transform.parent.gameObject;
        TargetCollider = GameObject.Find("NailHole");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (!other.CompareTag(TargetCollider.tag)){return;}
        
        //supress the next hand moton
        if (MTMobj.GetComponent<TranscriptionMaster>().transcribtionOn)
        {
            if (MTMobj.GetComponent<TranscriptionMaster>().transcribeHands)
            {
                MTMobj.GetComponent<TranscriptionMaster>().supressNextHandMotion = true;
            }
        }
        
        //fixedNail.GetComponent<InteractableObject>().gotPositioned = true;
        
        Debug.Log("Nail in Hole detected");
        //Debug.Log("Hammerhead correctly detected");
        Vector3 enclavePos = new Vector3(1, -0.5f, 0);

        // Nail.transform.position = enclavePos;
        Nail.SetActive(false);
        fixedNail.SetActive(true);
        
        /*Vector3 newPositionHammerHead = hammer.transform.GetChild(0).transform.position;
        Quaternion newRotHammerHead = hammer.transform.GetChild(0).transform.rotation;

        handle.GetComponent<MovablesCollisionHandler>().enabled = false;
        hammerHead.GetComponent<MovablesCollisionHandler>().enabled = false;
        
        hammer.SetActive(false);
        
        
        hammer.transform.position = enclavePos;
        
        newHammer.transform.GetChild(0).transform.position = newPositionHammerHead;
        newHammer.transform.GetChild(0).transform.rotation = newRotHammerHead;
        
        newHammer.SetActive(true);
        */

    }
}
