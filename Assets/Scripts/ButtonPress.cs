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
    private Color onColor = new Color(124, 252, 0);
    private Color offColor;
    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        offColor = rend.material.color;
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
        changeColor(true);
    }

    void ButtonReturn()
    {
        gameObject.transform.localPosition = defaultPosition;
        buttonOn = false;
        changeColor(false);
    }

    void changeColor(bool on)
    {
        if (on)
        {
            rend.material.color = onColor;
        }
        else
        {
            rend.material.color = offColor;
        }
    }
    
}
