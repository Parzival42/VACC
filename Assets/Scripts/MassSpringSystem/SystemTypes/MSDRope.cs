using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(TubeGenerator))]
public class MSDRope : BaseMSDSystem {

    private TubeGenerator tubeGenerator;
    private Tube tube;

    private int xAmount;
    private int yAmount;

    private Vector3[,] vertex2DRepresentation;
    private Vector3 changeVector;

    private List<Vector3> newVertexPositions;
    private Vector3[] vertexDirections;

    public override void SupplyMesh()
    {
        tubeGenerator = GetComponent<TubeGenerator>();
        tube = tubeGenerator.GenerateTube();
        mesh = tube.Mesh;

        xAmount = tube.Vertex2DRepresentation.GetLength(0);
        yAmount = tube.Vertex2DRepresentation.GetLength(1);

        vertex2DRepresentation = tube.Vertex2DRepresentation;
        changeVector = new Vector3();

        newVertexPositions = new List<Vector3>();
        vertexDirections = new Vector3[xAmount];

        for(int i = 0; i < mesh.vertexCount; i++)
        {
            newVertexPositions.Add(mesh.vertices[i]);
        }
    }


    public override void GeneratePointMasses()
    {
        Vector3 center = new Vector3();
      
        for (int j = 0; j < xAmount; j++)
        {
            center += vertex2DRepresentation[j, 0];
        }
       
        center /= (float)xAmount;

        for (int i = 0; i < xAmount; i++)
        {
            vertexDirections[i] = vertex2DRepresentation[i, 0] - center;

            if (i == 1)
            {
                Debug.Log("vertex: " + vertex2DRepresentation[i, 0]);
                Debug.Log("center: " + center);
                Debug.Log("vector: " + (vertex2DRepresentation[i, 0] - center));
            }
        }


        for (int i = 0; i < yAmount; i++)
        {
            center.Set(0, 0, 0);
            for (int j = 0; j < xAmount; j++)
            {
                center += vertex2DRepresentation[j, i];
            }
            center /= (float)xAmount;

            if (i == 0 || i == yAmount - 1)
            {
                pointList.Add(new FixedPointMass(gameObject.transform.TransformPoint(center)));
            }
            else
            {
                pointList.Add(new RegularPointMass(gameObject.transform.TransformPoint(center)));
            }
        }


       
    }   

    public override void GenerateConstraints()
    {
        for(int i = 1; i < pointList.Count; i++)
        {
            constraintList.Add(new RegularConstraint(pointList[i - 1], pointList[i]));
        }   
    }

    public override void UpdateMesh()
    {
        for(int i = 0; i < pointList.Count; i++)
        {           
            for(int j = 0; j < xAmount; j++)
            {
                newVertexPositions[i * xAmount + j] = gameObject.transform.InverseTransformPoint(pointList[i].Position)+vertexDirections[j]; 
            }
        }
        mesh.SetVertices(newVertexPositions);
    }
}
