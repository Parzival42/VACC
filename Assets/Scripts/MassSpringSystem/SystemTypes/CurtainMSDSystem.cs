using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
public class CurtainMSDSystem : CoreMSDSystem
{
    #region variables
    [SerializeField]
    private int fixedRowPoints = 11;

    [SerializeField]
    [Range(1,11)]
    private int skip = 0;

    private MonoBehaviour gravitationForce;
    private List<Vector3> newVertexPositions;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private float distance;
    #endregion

    #region methods
    public void SupplyMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        int[] indices = mesh.GetIndices(0);
        List<int> newIndices = new List<int>();
        for (int i = 0; i < mesh.GetIndices(0).Length; i += 3)
        {
            newIndices.Add(indices[i]);
            newIndices.Add(indices[i + 1]);
            newIndices.Add(indices[i + 2]);
            newIndices.Add(indices[i + 2]);
            newIndices.Add(indices[i + 1]);
            newIndices.Add(indices[i]);
        }
        mesh.SetIndices(newIndices.ToArray(), MeshTopology.Triangles, 0);        
    }
    
    public override void GeneratePointMasses()
    {
        pointList = new List<PointMass>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        newVertexPositions = new List<Vector3>();
        for (int i = 0; i < mesh.vertexCount; i++)
        {

            if (i < fixedRowPoints && i % skip == 0)
            {
                pointList.Add(new FixedPointMass(gameObject.transform.TransformPoint(mesh.vertices[i])));
            }
            else
            {
                pointList.Add(new RegularPointMass(gameObject.transform.TransformPoint(mesh.vertices[i])));
            }

            newVertexPositions.Add(pointList[i].Position);
        }
    }

    public override void GenerateConstraints()
    {
        constraintList = new List<Constraint>();
        int[] indices = mesh.GetIndices(0);
        for (int i = 0; i < mesh.GetIndices(0).Length; i += 3)
        {
            constraintList.Add(new RegularConstraint(pointList[indices[i]], pointList[indices[i + 1]]));
            constraintList.Add(new RegularConstraint(pointList[indices[i + 1]], pointList[indices[i + 2]]));
            constraintList.Add(new RegularConstraint(pointList[indices[i + 2]], pointList[indices[i]]));
        }
    }

    public override void PostSimulationStep()
    {
        for (int i = 0; i < pointList.Count; i++)
        {
            newVertexPositions[i] = gameObject.transform.InverseTransformPoint(pointList[i].Position);
        }
        mesh.SetVertices(newVertexPositions);
        mesh.RecalculateBounds();
    }

    public override void PreSimulationStep()
    {
        for (int i = 0; i < fixedRowPoints; i++)
        {
            if(i % skip == 0)
            {
                pointList[i].Position = transform.position + transform.right * i * distance;
            }
        }

        //apply external forces
        for (int j = 0; j < pointList.Count; j++)
        {
            pointList[j].ApplyForce(((Force)gravitationForce).getForce());
        }
    }

    public override void Initialize()
    {
        //simply add backfaces to have a plane that is visible from both sides
        int[] indices = mesh.GetIndices(0);
        List<int> newIndices = new List<int>();
        for (int i = 0; i < mesh.GetIndices(0).Length; i += 3)
        {
            newIndices.Add(indices[i]);
            newIndices.Add(indices[i + 1]);
            newIndices.Add(indices[i + 2]);
            newIndices.Add(indices[i + 2]);
            newIndices.Add(indices[i + 1]);
            newIndices.Add(indices[i]);
        }
        mesh.SetIndices(newIndices.ToArray(), MeshTopology.Triangles, 0);

        distance = Vector3.Distance(transform.TransformPoint(mesh.vertices[0]), transform.TransformPoint(mesh.vertices[1]));

        gravitationForce = gameObject.AddComponent<Gravitation>();
    }
    #endregion

}
