using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public Vector3 pressedPosition = new Vector3(0,-0.2f,0);
    public Vector3 defaultPosition = new Vector3(0,-0.8f,0);
    public bool buttonOn = false;
    private float timePressed;
    private float timeToBePressed = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        timePressed = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if(!buttonOn){return;}
        if (timePressed + timeToBePressed < Time.realtimeSinceStartup)
        {
            gameObject.transform.localPosition = defaultPosition;
            buttonOn = false;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (buttonOn) {return;}
        
        gameObject.transform.localPosition = pressedPosition;
        buttonOn = true;
        timePressed = Time.realtimeSinceStartup;
    }
}
