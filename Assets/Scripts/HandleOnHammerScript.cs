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
        if (other.CompareTag(hammerheadCollider.tag)) 
        { Debug.Log("Hammerhead correctly detected"); }

        GameObject handle = gameObject.transform.parent.gameObject;
        GameObject hammer = handle.transform.parent.gameObject;
        //GameObject hammerHead = hammerheadCollider.transform.parent.gameObject;
        hammer.SetActive(false);
        for (int i = 0; i < newHammer.transform.childCount; i++)
        {
            newHammer.transform.GetChild(i).transform.position = hammer.transform.GetChild(i).transform.position;
            newHammer.transform.GetChild(i).transform.rotation = hammer.transform.GetChild(i).transform.rotation;

        }
        
        
        
        
    }
    
    
}
