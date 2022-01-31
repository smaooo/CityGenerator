using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManyCityGenerator : MonoBehaviour
{
    public GameObject nodePrefab;
    private Vector3[] nodeLocations;
    private int[] numberOfTransforms;
    private Vector3[] numberOfVectors;
    private GameObject origin;
    private Vector3 originPos;
    private int numCities;
    /*
    private Vector3 node1 = new Vector3(0, 0, 0);
    private Vector3 node2 = new Vector3(100, 0, 100);
    */
    private void Start()
    {
        //numCities = gameObject.GetComponent<CityPlanner>().numberOfTransforms;
        //numCities = 2;
        //nodeLocations = gameObject.GetComponent<CityPlanner>().v_numberOfVectors;
        //nodeLocations = new Vector3[] { node1, node2 };

        numCities = GameObject.FindGameObjectWithTag("Planner").GetComponent<CityPlanner>().numberOfTransforms;
        nodeLocations = GameObject.FindGameObjectWithTag("Planner").GetComponent<CityPlanner>().Positions;
        GenerateCity(numCities,nodeLocations);
        
    }

    private void Checker()
    {
        for (int i = 0; i < nodeLocations.Length; i++)
        {
            Debug.Log(nodeLocations[i]);
        }
    }
    /*

       void Start()
       {
           //
           //int numCities = 2;


           //GenerateCity(numCities, nodeLocations);
           Debug.Log(numCities);
           //for (int i = 0; i < nodeLocations.Length; i++) { Debug.Log(nodeLocations[i]); }
           //Debug.Log(nodeLocations);

           origin = GameObject.FindGameObjectWithTag("Origin");
           originPos = origin.transform.position;
           numberOfTransforms = gameObject.GetComponent<CityPlanner>().a_numberOfTransforms;
           numberOfVectors = gameObject.GetComponent<CityPlanner>().v_numberOfVectors;
           //instantiate prefabs based on number of nodes, transform of nodes
           //for each number of numbers in a_numberOfTransforms, instantiate prefab based on teh corresponding vector
           for (int i = 0; i < numberOfTransforms.Length; i++) 
           {
               Instantiate(nodePrefab, numberOfVectors[i] + originPos, Quaternion.identity);
               nodePrefab.GetComponent<SimpleVisualizer>().Start();
           }

       }
     */

    private void GenerateCity(int numberOfCities, Vector3[] locationsOfThoseCities) 
    {
        origin = GameObject.FindGameObjectWithTag("Origin");
        originPos = origin.transform.position;
        for (int i = 0; i < numberOfCities; i++) 
        {
            Debug.Log(i);
            Instantiate(nodePrefab, locationsOfThoseCities[i], Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
