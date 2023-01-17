using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleOnHammerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject hammerheadCollider;

    void OnTriggerEnter(Collider other)
    {
       
        Debug.Log("object entered trigger");
        Debug.Log(other);
        Debug.Log(other.tag);
        if(other.CompareTag(hammerheadCollider.tag))
        {Debug.Log("Hammerhead correctly detected");}
        
    }
}
