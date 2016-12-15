//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class GridDeformation : MonoBehaviour {

//    //minimum and maxium coordinates of the mesh
//    public Vector3 minValues;
//    public Vector3 maxValues;

//    //how many particles should be generated in each direction
//    public int xSteps;
//    public int ySteps;
//    public int zSteps;

//    //bools that track certain states
//    public bool debugView;
//    public bool tesselateMesh;
//    public bool isInitialized;

//    //stores all masspoints properly arranged
//    private MassPoint[,,] masspoints;
//    //list of masspoints for faster iteration
//    private List<MassPoint> activeMassPoints;
//    //stores all springs
//    private List<Spring> springs;

//    //distances between the particles in all three directions
//    private float distX;
//    private float distY;
//    private float distZ;

//    //half the distances
//    private float halfX;
//    private float halfY;
//    private float halfZ;

//    //time meassure
//    private float startTime;
//    private float endTime;


//    //needed information about the mesh    
//    public List<Vector3> tempVertices;
//    public List<int>[] indices;
//    private List<Vector3> normals;
//    private List<Vector2> uvs;
//    private List<Vector3> verts;
//    private Mesh mesh;
//    private MeshFilter mf;
//    private MeshCollider meshCollider;
//    private List<float> influence;
//    private int biggestInfluence;

//    //the masspoint that is next to the projectile collision point
//    private MassPoint hitPoint;


//    //if the grid is set up, the debuggin view can be activated. 
//    void OnDrawGizmos()
//    {
//        if (isInitialized && debugView)
//        {
//            Gizmos.color = Color.black;

//            for (int x = 0; x < masspoints.GetLength(0); x++)
//            {
//                for (int y = 0; y < masspoints.GetLength(1); y++)
//                {
//                    for (int z = 0; z < masspoints.GetLength(2); z++)
//                    {
//                        if (x < masspoints.GetLength(0) - 1)
//                        {
//                            Gizmos.DrawLine(masspoints[x, y, z].Position, masspoints[x + 1, y, z].Position);
//                        }
//                        if (y < masspoints.GetLength(1) - 1)
//                        {
//                            Gizmos.DrawLine(masspoints[x, y, z].Position, masspoints[x, y + 1, z].Position);
//                        }
//                        if (z < masspoints.GetLength(2) - 1)
//                        {
//                            Gizmos.DrawLine(masspoints[x, y, z].Position, masspoints[x, y, z + 1].Position);
//                        }
//                    }
//                }
//            }
//        }
//    }



//    // Use this for initialization
//    void Start()
//    {

//        activeMassPoints = new List<MassPoint>();


//        isInitialized = false;

//        mf = gameObject.GetComponent<MeshFilter>();
//        if (mf != null)
//        {
//            mesh = mf.mesh;
//        }
//        else
//        {
//            Debug.Log("no mesh");
//        }

//        springs = new List<Spring>();

//        //getting the needed information from the mesh
//        indices = new List<int>[mesh.subMeshCount];
//        int[] tmpIndices;
//        for (int i = 0; i < mesh.subMeshCount; i++)
//        {
//            tmpIndices = mesh.GetTriangles(i);
//            indices[i] = new List<int>();
//            for (int j = 0; j < tmpIndices.Length; j++)
//            {
//                indices[i].Add(tmpIndices[j]);
//            }
//        }

//        normals = new List<Vector3>();
//        uvs = new List<Vector2>();
//        verts = new List<Vector3>();
//        tempVertices = new List<Vector3>();

//        for (int i = 0; i < mesh.vertexCount; i++)
//        {
//            tempVertices.Add(mesh.vertices[i]);
//            normals.Add(mesh.normals[i]);
//            uvs.Add(mesh.uv[i]);
//            verts.Add(mesh.vertices[i]);
//        }

//        meshCollider = GetComponent<MeshCollider>();
//    }




//    //this method searches for the surrounding vertices of a given position
//    //and return those vertices as well as an influence factor that is dependend on the distance 
//    private List<int> GetNearbyVertices(Vector3 position)
//    {
//        Vector3 point = new Vector3();
//        List<int> verts = new List<int>();
//        influence = new List<float>();

//        biggestInfluence = 0;

//        float currentInfluence = 0;
//        float currentBigestInfluence = 0.0f;
//        for (int i = tempVertices.Count - 1; i >= 0; i--)
//        {
//            point = tempVertices[i];
//            float x = Mathf.Abs(point.x - position.x);
//            float y = Mathf.Abs(point.y - position.y);
//            float z = Mathf.Abs(point.z - position.z);

//            if (x <= halfX && y <= halfY && z <= halfZ)
//            {
//                verts.Add(i);
//                currentInfluence = (1 - Mathf.Pow((x + y + z) / (halfX + halfY + halfZ), 2));
//                if (currentInfluence > currentBigestInfluence)
//                {
//                    currentBigestInfluence = currentInfluence;
//                    biggestInfluence = i;
//                }
//                influence.Add(currentInfluence);
//            }
//        }
//        return verts;
//    }

//    //set up method for the mass spring system. creates the masspoints
//    public void Generate3DGrid()
//    {
//        //time meassure
//        startTime = Time.realtimeSinceStartup;

//        int running = 0;
//        if (xSteps < 2 || ySteps < 2 || zSteps < 2)
//        {
//            Debug.Log("Select appropiate Values for the grid");
//            return;
//        }
//        masspoints = new MassPoint[xSteps, ySteps, zSteps];

//        distX = (maxValues.x - minValues.x) / (xSteps - 1);
//        distY = (maxValues.y - minValues.y) / (ySteps - 1);
//        distZ = (maxValues.z - minValues.z) / (zSteps - 1);

//        halfX = distX / 2.0f;
//        halfY = distY / 2.0f;
//        halfZ = distZ / 2.0f;

//        //world position
//        Vector3 position = new Vector3();
//        //local position
//        Vector3 posLocal = new Vector3();



//        for (int x = 0; x < masspoints.GetLength(0); x++)
//        {
//            for (int y = 0; y < masspoints.GetLength(1); y++)
//            {
//                for (int z = 0; z < masspoints.GetLength(2); z++)
//                {

//                    position.Set(minValues.x + (x * distX), minValues.y + (y * distY), minValues.z + (z * distZ));
//                    posLocal = position;
//                    position.Scale(transform.localScale);
//                    position = transform.rotation * position;
//                    position += transform.position;

//                    //creating a new masspoint
//                    masspoints[x, y, z] = new MassPoint(position, this, running);

//                    running++;

//                    //determine near vertices and the influence
//                    masspoints[x, y, z].InfluencedVertices = GetNearbyVertices(posLocal);
//                    masspoints[x, y, z].Influence = influence;
//                    masspoints[x, y, z].BiggestInfluence = biggestInfluence;
//                    activeMassPoints.Add(masspoints[x, y, z]);
//                }
//            }
//        }
//        GenerateSprings();
//        isInitialized = true;
//        endTime = Time.realtimeSinceStartup;
//        Debug.Log("Generating 3d grid method took: " + (endTime - startTime) + " seconds");
//    }

//    //when a hit is registered, the nearest masspoint is given the impact force
//    public void HandleHit(Vector3[] hitInfo)
//    {
//        if (isInitialized)
//        {
//            hitPoint = GetClosestPoint(hitInfo[0]);
//            hitPoint.AddExternalForce(hitInfo[1]);
//        }
//    }


//    //this method connects the masspoints by creating the needed springs
//    private void GenerateSprings()
//    {
//        Spring spring;
//        for (int x = 0; x < masspoints.GetLength(0); x++)
//        {
//            for (int y = 0; y < masspoints.GetLength(1); y++)
//            {
//                for (int z = 0; z < masspoints.GetLength(2); z++)
//                {
//                    if (x < masspoints.GetLength(0) - 1)
//                    {
//                        spring = new Spring(masspoints[x, y, z], masspoints[x + 1, y, z]);
//                        springs.Add(spring);
//                    }
//                    if (y < masspoints.GetLength(1) - 1)
//                    {
//                        spring = new Spring(masspoints[x, y, z], masspoints[x, y + 1, z]);
//                        springs.Add(spring);
//                    }
//                    if (z < masspoints.GetLength(2) - 1)
//                    {
//                        spring = new Spring(masspoints[x, y, z], masspoints[x, y, z + 1]);
//                        springs.Add(spring);
//                    }
//                }
//            }
//        }
//    }


//    void FixedUpdate()
//    {

//        if (isInitialized)
//        {
//            //update masspoints
//            for (int i = 0; i < activeMassPoints.Count; i++)
//            {
//                activeMassPoints[i].UpdatePoint();
//            }

//            //update springs
//            for (int i = 0; i < springs.Count; i++)
//            {
//                springs[i].UpdateSpring();
//            }


//            //update mesh vertices
//            mesh.SetVertices(tempVertices);
//            mesh.RecalculateNormals();
//            //update mesh
//            mf.mesh = mesh;
//            meshCollider.sharedMesh = mesh;
//        }
//        else
//        {
//            if (tesselateMesh)
//            {
//                tesselateMesh = false;
//                TesselateMesh();
//            }
//        }

//    }


//    //this method tesselates the whole mesh by running through all traingles and splitting them up into four smaller ones
//    public void TesselateMesh()
//    {
//        for (int i = 0; i < mesh.subMeshCount; i++)
//        {
//            for (int j = 0; j < mesh.GetTriangles(i).Length; j += 3)
//            {
//                tesselator.Tesselate(mesh.GetTriangles(i)[j], mesh.GetTriangles(i)[j + 1], mesh.GetTriangles(i)[j + 2], i, tempVertices, indices, normals, uvs);
//            }
//        }

//        mesh.SetVertices(tempVertices);
//        for (int i = 0; i < indices.Length; i++)
//        {
//            mesh.SetTriangles(indices[i], i);
//        }
//        mesh.SetNormals(normals);
//        mesh.SetUVs(0, uvs);

//        mesh.RecalculateBounds();
//        mf.mesh = mesh;

//        meshCollider.sharedMesh = mesh;
//    }

//    //this method searches for the nearest shape to any given position
//    private MassPoint GetClosestPoint(Vector3 position)
//    {
//        //time meassure
//        startTime = Time.realtimeSinceStartup;


//        float distance = 0.0f;
//        bool first = true;
//        MassPoint toReturn = masspoints[0, 0, 0];

//        for (int i = 0; i < activeMassPoints.Count; i++)
//        {
//            float dist = (Vector3.SqrMagnitude(activeMassPoints[i].Position - position));

//            if (first)
//            {
//                first = false;
//                distance = dist;
//                toReturn = activeMassPoints[i];
//            }
//            else if (dist < distance)
//            {
//                distance = dist;
//                toReturn = activeMassPoints[i];
//            }
//        }

//        //time meassure
//        endTime = Time.realtimeSinceStartup;
//        Debug.Log("closest point method took: " + (endTime - startTime) + " seconds");

//        return toReturn;
//    }
//}

//}
