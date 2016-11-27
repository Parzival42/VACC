using UnityEngine;
using System.Collections.Generic;

public class BaseMSDSystem : MonoBehaviour {

    #region variables
    private MeshFilter meshFilter;
    private Mesh mesh;
    private MeshCollider meshCollider;

    private List<PointMass> pointList;
    private List<Constraint> constraintList;

    private List<Vector3> newVertexPositions;
    private bool isInitialized = false;

    private List<int> newIndices;

    private MonoBehaviour gravitationForce;
    #endregion




    #region methods

    // Use this for initialization
    void Start()
    {
        newIndices = new List<int>();
        pointList = new List<PointMass>();
        constraintList = new List<Constraint>();

        newVertexPositions = new List<Vector3>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        //meshCollider = GetComponent<MeshCollider>();

        gravitationForce = gameObject.AddComponent<Gravitation>();


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


        int[] indices = mesh.GetIndices(0);
        for (int i = 0; i < mesh.GetIndices(0).Length; i += 3)
        {
            constraintList.Add(new RegularConstraint(pointList[indices[i]], pointList[indices[i+1]]));
            constraintList.Add(new RegularConstraint(pointList[indices[i+1]], pointList[indices[i + 2]]));
            constraintList.Add(new RegularConstraint(pointList[indices[i+2]], pointList[indices[i]]));
           
            newIndices.Add(indices[i]);
            newIndices.Add(indices[i + 1]);
            newIndices.Add(indices[i + 2]);
            newIndices.Add(indices[i + 2]);
            newIndices.Add(indices[i + 1]);
            newIndices.Add(indices[i]);
        }

        mesh.SetIndices(newIndices.ToArray(), MeshTopology.Triangles, 0);

        isInitialized = true;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        for (int j = 0; j < pointList.Count; j++)
        {
            pointList[j].ApplyForce(((Force)gravitationForce).getForce());
        }

        for (int j = 0; j < 15; j++)
        {
            for (int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].Solve();
            }
        }


        for (int i = 0; i < pointList.Count; i++)
        {
            pointList[i].Simulate(Time.deltaTime);

            newVertexPositions[i] = gameObject.transform.InverseTransformPoint(pointList[i].Position);
        }

        mesh.SetVertices(newVertexPositions);
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    void OnDrawGizmos()
    {
        if (isInitialized)
        {
            Vector3 size = new Vector3(0.05f, 0.05f, 0.05f);
            for (int i = 0; i < pointList.Count; i++)
            {
                pointList[i].DrawPoint();
            }

            for (int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].DrawConnection();

            }
        }
    }
    #endregion

}
