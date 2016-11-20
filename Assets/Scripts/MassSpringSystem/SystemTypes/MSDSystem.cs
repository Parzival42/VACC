using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(MeshFilter))]
public class MSDSystem : MonoBehaviour {

    private MeshFilter meshFilter;
    private Mesh mesh;
    private MeshCollider meshCollider;

    private List<MassPoint> pointList;

    private List<Vector3> newVertexPositions;
    private bool isInitialized = false;

    private List<int> newIndices;


    [SerializeField]
    private MonoBehaviour gravitationForce;

    [SerializeField]
    private MonoBehaviour[] externalForces;

    private List<SphereCollider> colliders;

    private MassPoint massPoint;
    private Vector3 pinPoint = new Vector3();
    private Vector3 startPosition;

    [SerializeField]
    [Range(0.0f,1.0f)]
    private float slider = 0.0f;


    [SerializeField]
    private Vector3 collBoxSize = new Vector3();

    [SerializeField]
    private Vector3 collBoxCenter = new Vector3();

	// Use this for initialization
	void Start () {
        newIndices = new List<int>();
        pointList = new List<MassPoint>();
        newVertexPositions = new List<Vector3>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        //meshCollider = GetComponent<MeshCollider>();

        colliders = new List<SphereCollider>();

     
        for(int i = 0; i < mesh.vertexCount; i++)
        {
           // if(i%4==0)
            {
                //colliders.Add(gameObject.AddComponent<SphereCollider>());
                //colliders[colliders.Count - 1].center = (mesh.vertices[i]);
                //colliders[colliders.Count - 1].radius = 0.04f;
                //colliders[colliders.Count - 1].isTrigger = true;

            }

            //if (i == 0 || i == 10)
            //{
            //    pointList.Add(new MassPoint(gameObject.transform.TransformPoint(mesh.vertices[i]), 2 - (0.1f * i / 10), true));

            //    massPoint = pointList[pointList.Count - 1];
            //    startPosition = massPoint.Position;
            //}
            //else
            //{
            //    pointList.Add(new MassPoint(gameObject.transform.TransformPoint(mesh.vertices[i]), 2 - (0.1f * i / 10), false));
            //}

            if (i < 11)
            {
                pointList.Add(new MassPoint(gameObject.transform.TransformPoint(mesh.vertices[i]), 2 - (0.1f * i / 10), true));
            }
            else
            {
                pointList.Add(new MassPoint(gameObject.transform.TransformPoint(mesh.vertices[i]), 2 - (0.1f * i / 10), false));
            }

            //if (i %13 ==0 )
            //{
            //    pointList.Add(new MassPoint(gameObject.transform.TransformPoint(mesh.vertices[i]), 3, true));
            //}
            //else
            //{
            //    pointList.Add(new MassPoint(gameObject.transform.TransformPoint(mesh.vertices[i]), 3, false));
            //}
            newVertexPositions.Add(pointList[i].Position);
        }


        int[] indices = mesh.GetIndices(0);
        for(int i = 0; i < mesh.GetIndices(0).Length; i+=3)
        {
            pointList[indices[i]].ConnectTo(pointList[indices[i + 1]]);
            pointList[indices[i+1]].ConnectTo(pointList[indices[i + 2]]);
            pointList[indices[i+2]].ConnectTo(pointList[indices[i]]);

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
	void FixedUpdate () {

        pinPoint.x = 2* slider;
        //   massPoint.PinToPoint(startPosition + pinPoint);

        Vector3 accumulatedForces = new Vector3();


        for (int i = 0; i < externalForces.Length; i++)
        {
            accumulatedForces += ((Force)externalForces[i]).getForce();            
        }

        for (int j = 0; j < pointList.Count; j++)
        {
            pointList[j].ApplyForce(accumulatedForces);
            if(pointList[j].Position.y >= 0.35f)
            {
                pointList[j].ApplyForce(((Force)gravitationForce).getForce());
            }
        }

        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < pointList.Count; i++)
            {
                pointList[i].UpdateConnections();
            }
        }

        for (int i = 0; i < pointList.Count; i++)
        {
            if (pointList[i].Position.y < 0.01f)
            {

                //pointList[i].PinToPoint(pointList[i].Position);
                //pointList[i].ApplyForce(new Vector3(0, 9.81f *1.5f, 0));
                pointList[i].Position = new Vector3(pointList[i].Position.x, 0.15f, pointList[i].Position.z);

            }

            if(pointList[i].Position.z > 1.15f)
            {
                pointList[i].ApplyForce(new Vector3(0,0,-slider*50));
                //pointList[i].KillAcceleration();
            }
        }


        for (int i = 0; i < pointList.Count; i++)
        {
            pointList[i].Update(Time.deltaTime);
            newVertexPositions[i] = gameObject.transform.InverseTransformPoint( pointList[i].Position);
        }

        for(int i = 0; i < colliders.Count; i++)
        {
            colliders[i].center = newVertexPositions[i];
        }


       
        //colliders[0].center = newVertexPositions[0];
        //colliders[1].center = newVertexPositions[10];
        //colliders[2].center = newVertexPositions[110];
        //colliders[3].center = newVertexPositions[120];


        mesh.SetVertices(newVertexPositions);
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
	}

    void OnDrawGizmos()
    {
        if (isInitialized)
        {
            Vector3 size = new Vector3(0.05f,0.05f,0.05f);
            for(int i = 0; i < pointList.Count; i++)
            {
                Gizmos.DrawCube(pointList[i].Position, size);
            }
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        
    }

    
}
