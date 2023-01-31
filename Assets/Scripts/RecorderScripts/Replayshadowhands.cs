using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replayshadowhands : MonoBehaviour
{
    public int sequence;
    public string shadowhandFolder = "Assets/Resources/ShadowHands";

    public Dictionary<int, string> animationFolderDict = new Dictionary<int, string>();
    // Start is called before the first frame update
    void Start()
    {
        sequence = 0;
        animationFolderDict.Add(0,"/Handpass");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            PlayShadow();
        }
    }

    void PlayShadow()
    {
        string animationFolder = shadowhandFolder+animationFolderDict[sequence];
        GameObject recorderObject = gameObject;
        recorderObject.GetComponent<ObjectManipulator>().loadFromCSVFile("ShadowHands/HandPass");
        recorderObject.GetComponent<HandPoseManipulation>().loadFromCSVFile("ShadowHands/HandPass");
        
        recorderObject.GetComponent<ObjectManipulator>().startreplay();
        recorderObject.GetComponent<HandPoseManipulation>().startreplay();
    }
}
