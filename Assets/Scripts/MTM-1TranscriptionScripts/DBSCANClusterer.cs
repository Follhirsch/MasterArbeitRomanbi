using System;
using System.Collections.Generic;
using UnityEngine;

public class DBSCANClusterer : MonoBehaviour {
    public float epsilon = 0.05f;  // Radius around each point that determines its neighborhood
    public int minPoints = 5;  // Minimum number of points required to form a dense region
    public GameObject obj1;
    public GameObject obj2;


    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Cluster(GameObject.Find("Recorder/Player").GetComponent<HandPoseManipulation>().rPosArray);
        }
        if (Input.GetKeyDown("l"))
        {
            Debug.Log(Vector3.Distance(obj1.transform.position,obj2.transform.position));
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

