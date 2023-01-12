using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class mirrorPositionScript : MonoBehaviour
{
    public GameObject sourceObject;
    public GameObject targetObject;
    // Start is called before the first frame update
    void Start()
    {
        targetObject = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        targetObject.transform.position = sourceObject.transform.position;
        targetObject.transform.rotation = sourceObject.transform.rotation;
    }
}
