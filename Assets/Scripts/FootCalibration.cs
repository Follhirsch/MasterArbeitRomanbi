using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FootCalibration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c")) //calibrate Head Hight
        {
            //Debug.Log(gameObject.transform.rotation);
            //Debug.Log(gameObject.transform.position);
            Quaternion rot = gameObject.transform.rotation;
            Vector3 pos = gameObject.transform.position;
            float x = pos.x;
            float y = 0.05f;
            float z = pos.z;
            
            float rx = rot.eulerAngles.x;
            float ry = rot.eulerAngles.y;
            float rz = rot.eulerAngles.z;
            rot.eulerAngles = new Vector3(90,ry,rz);
            pos = new Vector3(x, y, z);
            gameObject.transform.rotation = rot;
            gameObject.transform.position = pos;
            //gameObject.transform.position = new Vector3()
        }
        if (Input.GetKeyDown("v")) //calibrate Head Hight
        {
            if (gameObject.name == "Leftfoot"){return;}
            //Debug.Log(gameObject.transform.rotation.eulerAngles.x);
            //Quaternion rRot = gameObject.transform.rotation;
            //double angle = Mathf.Atan(2 * (rRot.w * rRot.z + rRot.y * rRot.x)/( 1 - 2 * ((rRot.y * rRot.y) + (rRot.z * rRot.z))))*180/Math.PI;
            //Debug.Log(angle);
            Vector3 forward = gameObject.transform.up;
            Debug.Log(forward);
            Debug.Log(Vector3.Angle(new Vector3(0,1,0),forward));
            
        }
        
        
    }
}
