using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailGroupingchange : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "NailGroupTrigger";
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown("t"))
        {
            UpdateInteractableObject();
        }*/


    }

    public void UpdateInteractableObject()
    {
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 0.03f);
        bool isNowInGroup = false;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag(gameObject.tag))
            {
                if (collider.gameObject.transform.parent.name != gameObject.transform.parent.name)
                {
                    isNowInGroup = true;
                    break;
                }
            }
        }
        //Debug.Log(isNowInGroup);
        gameObject.transform.parent.GetComponent<InteractableObject>().isInGroup = isNowInGroup;
    }
}
