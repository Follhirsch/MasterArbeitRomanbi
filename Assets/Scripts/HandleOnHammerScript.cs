using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRfreePluginUnity;

public class HandleOnHammerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject hammerheadCollider;
    public GameObject newHammer;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(hammerheadCollider.tag)){return;}
        
        GameObject handle = gameObject.transform.parent.gameObject;
        GameObject hammer = handle.transform.parent.gameObject;
        GameObject hammerHead = hammerheadCollider.transform.parent.gameObject;

        if (handle.transform.localRotation.eulerAngles.x>10 ||handle.transform.localRotation.eulerAngles.y>10){return;}


            Debug.Log("Hammerhead correctly detected");

        handle.GetComponent<MovablesCollisionHandler>().enabled = false;
        hammerHead.GetComponent<MovablesCollisionHandler>().enabled = false;
        
       
        
        newHammer.transform.GetChild(0).transform.position = hammer.transform.GetChild(0).transform.position;
        newHammer.transform.GetChild(0).transform.rotation = hammer.transform.GetChild(0).transform.rotation;
        
        newHammer.SetActive(true);
        hammer.SetActive(false);
        
        
        
        
        
    }
    
    
}
