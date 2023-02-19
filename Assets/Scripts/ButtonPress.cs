using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    private Vector3 pressedPosition;
    private Vector3 defaultPosition;
    public float distanceToMove = 0.6f;
    public bool buttonOn = false;
    private float timePressed;
    private float timeToBePressed = 3f;
    public GameObject light;

    // Start is called before the first frame update
    void Start()
    {
        timePressed = Time.realtimeSinceStartup;
        defaultPosition = new Vector3(gameObject.transform.localPosition.x,gameObject.transform.localPosition.y,gameObject.transform.localPosition.z);
        pressedPosition = new Vector3(defaultPosition.x, defaultPosition.y + distanceToMove, defaultPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("b"))
        {
            pressedPosition = new Vector3(defaultPosition.x, defaultPosition.y + distanceToMove, defaultPosition.z);
            ButtonPressed();
        }
        
        if(!buttonOn){return;}
        if (timePressed + timeToBePressed < Time.realtimeSinceStartup)
        {
            ButtonReturn();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        ButtonPressed();
    }

    void ButtonPressed()
    {
        if (buttonOn) {return;}
        
        gameObject.transform.localPosition = pressedPosition;
        buttonOn = true;
        timePressed = Time.realtimeSinceStartup;
        ChangeLightning(2f);
    }

    void ButtonReturn()
    {
        gameObject.transform.localPosition = defaultPosition;
        buttonOn = false;
        ChangeLightning(1f);
    }

    void ChangeLightning(float newIntensity)
    {
        light.GetComponent<Light>().intensity = newIntensity;
    }
    
}
