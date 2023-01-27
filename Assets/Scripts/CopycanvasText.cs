using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopycanvasText : MonoBehaviour
{
    public GameObject sourceobj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Text>().text = sourceobj.GetComponent<Text>().text;
    }
}
