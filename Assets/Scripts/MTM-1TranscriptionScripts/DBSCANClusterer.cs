using System;
using System.Collections.Generic;
using UnityEngine;

public class DBSCANClusterer : MonoBehaviour {
    public float epsilon = 0.05f;  // Radius around each point that determines its neighborhood
    public int minPoints = 5;  // Minimum number of points required to form a dense region

    public float rad = 0.1f;
    public GameObject spherePrefab;
    
    
    public List<GameObject> dataVisualisationObjects = new List<GameObject>();

    private Vector3[] posArray = new []
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(1, 1, 0)
    };

    private int[] groupIDs = new[] { 1, 1, 2, 2 };
    private int nrOfGroups = 2;

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

    private void Start()
    {
        rad = 0.1f;
        drawData();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Cluster(GameObject.Find("Recorder/Player").GetComponent<HandPoseManipulation>().rPosArray);
        }
        if (Input.GetKeyDown("m"))
        {
            drawData();
        }
    }

    public void Cluster(Vector3[][] posData) {
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
        Debug.Log(clusterIndex);

        // Do something with the resulting cluster labels
        // For example, you could use them to color-code the points in a visualization
    }

    private List<int> GetNeighbors(int pointIndex, Vector3[][] posData) {
        List<int> neighbors = new List<int>();
        Vector3 point = posData[pointIndex][0];

        for (int i = 0; i < posData.Length; i++) {
            if (i == pointIndex) {
                continue;
            }

            Vector3 otherPoint = posData[i][0];
            float distance = Vector3.Distance(point, otherPoint);

            if (distance <= epsilon) {
                neighbors.Add(i);
            }
        }

        return neighbors;
    }

    private void ExpandCluster(int pointIndex, List<int> neighbors, int clusterIndex, int[] clusterLabels, Vector3[][] posData) {
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

