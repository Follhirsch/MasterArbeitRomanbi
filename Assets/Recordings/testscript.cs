using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VRfreePluginUnity;

namespace testscriptprojekt
{
    public class testscript : MonoBehaviour
    {
        public GameObject Rod;
        public HandDimensionScript handDimensionScript;
        public Vector3 pos;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            pos = Rod.transform.position;
        }
    }
}