using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DBSCANClusterer : MonoBehaviour {
    
    //https://iopscience.iop.org/article/10.1088/1755-1315/31/1/012012/pdf to find good epsilon
    public float epsilon = 0.02f;  // Radius around each point that determines its neighborhood
    public int deltaFrames = 10;
    public int minPoints = 20;  // Minimum number of points required to form a dense region

    public bool displayRightHand = true;
    public float rad = 0.1f;
    public GameObject spherePrefab;
    
    
    public List<GameObject> dataVisualisationObjects = new List<GameObject>();
    
    public Vector3[][] demoPosArray;
    public int startFrame = 130;
    public int endFrame = 332;
    private Vector3[] posArray;
    private int index = 7;

    private int[] groupIDs;// = new[] { 1, 1, 2, 2 };
    private int nrOfGroups;// = 2;

    public int[][] colours = new int[][]
    {
        new int[] { 255, 0, 0 },
        new int[] { 255, 255, 0 },
        new int[] { 0, 234, 255 },
        new int[] { 170, 0, 255 },
        new int[] { 255, 127, 0 },
        new int[] { 191, 255, 0 },
        new int[] { 0, 149, 255 },
        new int[] { 255, 0, 170 },
        new int[] { 255, 212, 0 },
        new int[] { 106, 255, 0 },
        new int[] { 0, 64, 255 }
    };
    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            if (displayRightHand)
            {
                demoPosArray = GameObject.Find("Recorder/Player").GetComponent<HandPoseManipulation>().rPosArray;
            }
            else
            {
                demoPosArray = GameObject.Find("Recorder/Player").GetComponent<HandPoseManipulation>().lPosArray;
            }
            
            if (endFrame > demoPosArray.Length)
            {
                endFrame = demoPosArray.Length - 1;
            }
            if (startFrame > demoPosArray.Length)
            {
                startFrame = 0;
            }
            
            posArray = new Vector3[endFrame-startFrame];
            int ii = startFrame;
            for (int i = 0; i < posArray.Length; i++)
            {
                posArray[i] = demoPosArray[ii][index];
                ii++;
            }
            Cluster(posArray);
            drawData();


            Tuple<int, int>[] frames = classifyMotionFrames(posArray);
            for (int i = 0; i < frames.Length; i++)
            {
                Vector3 pos1 = posArray[frames[i].Item1];
                Vector3 pos2 = posArray[frames[i].Item2];
                float delta = (pos2 - pos1).magnitude * 100;
                Debug.Log("move" + (i+1) + " frame"+frames[i].Item1 +" to"+frames[i].Item2+" from " + pos1 + " to " + pos2 + " dist: " + delta);
            }


        }
        if (Input.GetKeyDown("d"))
        {
            drawData();
        }
    }


    void drawData()
    {
        float radius = rad;
        foreach(GameObject obj in dataVisualisationObjects)
        {
            Destroy(obj);
        }
        dataVisualisationObjects.Clear();
        Material[] materials = new Material[nrOfGroups];
        for (int i = 0; i < nrOfGroups; i++)
        {
         materials[i] = new Material(Shader.Find("Standard"));
         materials[i].color = new Color(colours[(int)Mathf.Repeat(i, colours.Length)][0],
             colours[(int)Mathf.Repeat(i, colours.Length)][1], colours[(int)Mathf.Repeat(i, colours.Length)][2]);
        }
        
        for (int i = 0; i < posArray.Length; i++)
        {
            // Instantiate the sphere prefab at the position of the sphere data
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = Vector3.one * radius;
            sphere.transform.position = posArray[i];
            dataVisualisationObjects.Add(sphere);

            // Set the color of the sphere based on the group identifier
            if (groupIDs[i]>0)
            {sphere.GetComponent<Renderer>().material = materials[groupIDs[i] - 1];}
        }
    }

    public void Cluster(Vector3[] posData) // output itermediate motions from frame to frame [framestart,frameend]
    {
        int numPoints = posData.Length;
        int[] clusterLabels = new int[numPoints];
        int clusterIndex = 0;

        for (int i = 0; i < numPoints; i++) {
            if (clusterLabels[i] != 0) {
                // Point already assigned to a cluster or marked as noise
                continue;
            }

            // Find all neighboring points within epsilon
            List<int> neighbors = GetNeighbors(i, posData);

            if (neighbors.Count < minPoints) {
                // Point is noise
                clusterLabels[i] = -1;
            }
            else {
                // Point belongs to a new cluster
                clusterIndex++;
                ExpandCluster(i, neighbors, clusterIndex, clusterLabels, posData);
            }
        }

        nrOfGroups = clusterIndex;
        groupIDs = clusterLabels;
    }

    public Tuple<int, int>[] classifyMotionFrames(Vector3[] posData)
    {
        if (posData.Length < 1) { return null;}
        int numPoints = posData.Length;
        int[] clusterLabels = new int[numPoints];
        int clusterIndex = 0;
      

        for (int i = 0; i < numPoints; i++) {
            if (clusterLabels[i] != 0) {
                // Point already assigned to a cluster or marked as noise
                continue;
            }

            // Find all neighboring points within epsilon
            List<int> neighbors = GetNeighbors(i, posData);

            if (neighbors.Count < minPoints) {
                // Point is noise
                clusterLabels[i] = -1;
            }
            else {
                // Point belongs to a new cluster
                clusterIndex++;
                ExpandCluster(i, neighbors, clusterIndex, clusterLabels, posData);
            }
        }

        bool startMissed = false;
        int startLabel = -1;
        bool endMissed = false;
        int endLabel = -1;
        for (int i = 0; i < Math.Min(10,clusterLabels.Length); i++)
        {
            bool tempBoolS = (clusterLabels[i] != -1);
            if (tempBoolS)
            {
                startMissed = false;
                startLabel = clusterLabels[i];
                break;
            }
            else
            {
                startMissed = true;
            }
        }
        for (int i = 0; i < Math.Min(10,clusterLabels.Length); i++)
        {
            bool tempBoolE = clusterLabels[^(i+1)] != -1;
            if (tempBoolE)
            {
                endMissed = false;
                endLabel = clusterLabels[^(i + 1)];
                break;
            }
            else
            {
                endMissed = true;
            }
        }
        //if both not missed check if they are in the same label if yes assign witch one is missed
        if (!startMissed && !endMissed)
        {
            if (startLabel == endLabel)
            {
                if (startLabel != 1)
                {
                    startMissed = true;
                }
                else
                {
                    endMissed = true;
                }
            }
        }
        
        
        int comparisonindex = clusterIndex;
        if (startMissed) { comparisonindex++;}
        if (endMissed) { comparisonindex++;}


        if (comparisonindex < 3)
        {
            Tuple<int, int>[] singleReturnTouples = new Tuple<int, int>[1];
            singleReturnTouples[0] = new Tuple<int, int>(0, posData.Length - 1);
            return singleReturnTouples;
        }
        
        //get detected frames
        List<int> indexframes = new List<int>();
        for (int i = 1; i < clusterIndex+1; i++)
        {
            for (int j = 0; j < clusterLabels.Length; j++)
            {
                if (clusterLabels[j] == i)
                {
                    indexframes.Add(j);
                    break;
                }
            }
        }
        
        List<int> indexEndFrames = new List<int>();
        for (int i = 0; i < indexframes.Count-1; i++)
        {
            indexEndFrames.Add(indexframes[i + 1]);
        }

        //int[] startFrames = new int[comparisonindex-1];
        //int[] endFrames = new int[startFrames.Length];

        List<int> startFrames = new List<int>();
        List<int> endFrames = new List<int>(); 

        if (startMissed)
        {
            startFrames.Add(0);
            endFrames.Add(indexframes[0]);
            startFrames = startFrames.Concat(indexframes).ToList();
            endFrames = endFrames.Concat(indexEndFrames).ToList();
        }
        else
        {
            startFrames = indexframes;
            endFrames = indexEndFrames;
        }
        if (endMissed)
        {
            endFrames.Add(posData.Length-1);
        }
        else
        {
            startFrames.RemoveAt(startFrames.Count-1);
        }
        
        
        startFrames[0] = 0;
        endFrames[^1] = posData.Length - 1;
        


        Tuple<int, int>[] returnTuples = new Tuple<int, int>[startFrames.Count];

        for (int i = 0; i < returnTuples.Length; i++)
        {
            returnTuples[i] = new Tuple<int, int>(startFrames[i], endFrames[i]);
        }
        
        return returnTuples;
    }

        private List<int> GetNeighbors(int pointIndex, Vector3[] posData) {
        List<int> neighbors = new List<int>();
        Vector3 point = posData[pointIndex];

        int start = Math.Max(0, pointIndex - deltaFrames);
        int end = Math.Min(posData.Length, pointIndex + deltaFrames);

        for (int i = start; i < end; i++) {
            if (i == pointIndex) {
                continue;
            }

            Vector3 otherPoint = posData[i];
            float distance = Vector3.Distance(point, otherPoint);

            if (distance <= epsilon) {
                neighbors.Add(i);
            }
        }

        return neighbors;
    }

    private void ExpandCluster(int pointIndex, List<int> neighbors, int clusterIndex, int[] clusterLabels, Vector3[] posData) {
        // Assign point to cluster
        clusterLabels[pointIndex] = clusterIndex;

        // Expand cluster to neighboring points
        for (int i = 0; i < neighbors.Count; i++) {
            int neighborIndex = neighbors[i];

            if (clusterLabels[neighborIndex] == 0) {
                // Neighboring point not yet assigned to a cluster
                List<int> newNeighbors = GetNeighbors(neighborIndex, posData);

                if (newNeighbors.Count >= minPoints) {
                    // Neighboring point is core point
                    neighbors.AddRange(newNeighbors);
                }
            }

            if (clusterLabels[neighborIndex] <= 0) {
                // Neighbor belongs to current cluster or is noise
                clusterLabels[neighborIndex] = clusterIndex;
            }
        }
    }
    
}

