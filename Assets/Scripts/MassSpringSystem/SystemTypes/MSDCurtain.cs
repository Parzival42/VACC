using UnityEngine;
using System.Collections.Generic;
using System;

public class MSDCurtain : BaseMSDSystem
{
    #region variables
    private List<Vector3> newVertexPositions;
    #endregion
    #region methods
    public override void SupplyMesh()
    {
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
        newVertexPositions = new List<Vector3>();
        for (int i = 0; i < mesh.vertexCount; i++)
        {

            if (i < 11)
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
    }

    public override void PreSimulationStep()
    {
       //nothing to do here
    }
    #endregion

}
