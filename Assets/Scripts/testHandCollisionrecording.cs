using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRfreePluginUnity;

public class testHandCollisionrecording : MonoBehaviour
{
    public GameObject hands;
    public GameObject rightHand;
    public GameObject leftHand;

    // Start is called before the first frame update
    void Start()
    {
        rightHand = hands.transform.GetChild(0).gameObject;
        leftHand = hands.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Debug.Log(leftHand.GetComponent<HandCollisionMaster>().collisionLists[16]);
            
        }
        int nrOfTouchedObjects = leftHand.GetComponent<HandCollisionMaster>().collisionLists.Count;
        if (nrOfTouchedObjects > 16)
        {
            for (int i = 16; i < nrOfTouchedObjects; i++)
            {
                CollisionHandler touchedObject =
                    leftHand.GetComponent<HandCollisionMaster>().collisionLists[i].collisionHandler;
                Debug.Log(touchedObject.gameObject.name);
            }
        }

    }
}
