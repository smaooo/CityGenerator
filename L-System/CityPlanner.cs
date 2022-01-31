using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityPlanner : MonoBehaviour
{
    private Transform[] nodeLocations;
    [HideInInspector]
    public int numberOfTransforms = 0;
    public int[] a_numberOfTransforms;
    [HideInInspector]
    public Vector3[] v_numberOfVectors;
    public Vector3[] Positions;
    private GameObject[] points;
    Vector3 node1 = new Vector3(0, 0, 0);
    Vector3 node2 = new Vector3(100, 0, 100);

    public void Awake()
    {
        //define the number of items in the array of transforms
        numberOfTransforms = 2;
        
        //define the array of transforms based on the number of items in the array of transforms
        //a_numberOfTransforms = new int[numberOfTransforms];
        //populate the array of transforms with transforms
        v_numberOfVectors = new Vector3[] { node1, node2 };
        //Debug.Log(v_numberOfVectors);
        Positions = new Vector3[numberOfTransforms];
        Positions[0] = node1;
        Positions[1] = node2;
        //Debug.Log(Positions);

        Checker();
    }

    private void Checker() 
    {
        for (int i = 0; i < Positions.Length; i++) 
        {
            Debug.Log(Positions[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
