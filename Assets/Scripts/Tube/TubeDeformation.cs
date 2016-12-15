using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
public class TubeDeformation : MonoBehaviour {

    private MeshFilter meshFilter;
    private Mesh mesh;


    private Vector3[,] sortedVertices;


	// Use this for initialization
	void Start () {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;


        AnalyzeMesh();

	
	}

    private void AnalyzeMesh()
    {
     

    }

    // Update is called once per frame
    void Update () {
	
	}
}
