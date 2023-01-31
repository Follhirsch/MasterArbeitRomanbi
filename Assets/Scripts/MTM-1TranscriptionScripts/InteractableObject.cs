using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public int weight;
    public bool isSmall;
    public bool isCylindrical;
    public bool isInGroup;
    public bool isAtKnownLocation;
    public bool gotPositioned;
    public bool gotDisengaged;
    public bool isCrank;
    public bool isInHandRH;
    public bool isInHandLH;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "InteractableObject";
        weight = (int)gameObject.GetComponent<Rigidbody>().mass;
        isInHandRH = false;
        isInHandLH = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
