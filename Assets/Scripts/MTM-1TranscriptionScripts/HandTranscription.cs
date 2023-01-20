using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTranscription : MonoBehaviour
{
    public List<int> lHstartGraspFrame = new List<int>();
    public List<int> lHstopGraspFrame = new List<int>();
    public List<int> rHstartGraspFrame = new List<int>();
    public List<int> rHstopGraspFrame = new List<int>();

    public bool rightHandOnGrasp;
    public bool leftHandOnGrasp;
    
    // Start is called before the first frame update
    void Start()
    {
        rightHandOnGrasp = false;
        leftHandOnGrasp = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void rHGraspChange()
    {
        
    }
}
