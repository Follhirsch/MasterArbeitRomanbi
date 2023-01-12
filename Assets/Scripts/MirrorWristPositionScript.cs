using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class MirrorWristPositionScript : MonoBehaviour
{
    public GameObject HandSourceObject;
    public GameObject targetObject;
    // Start is called before the first frame update
    void Start()
    {
        targetObject = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        targetObject.transform.position = HandSourceObject.transform.GetChild(4).GetChild(0).position;
        targetObject.transform.rotation = HandSourceObject.transform.GetChild(4).GetChild(0).rotation;
    }
}
