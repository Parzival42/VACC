using UnityEngine;
using System.Collections.Generic;

public abstract class BaseMSDSystem : MonoBehaviour {

    #region variables
    protected MeshFilter meshFilter;
    protected SkinnedMeshRenderer skinnedMeshRenderer;
    protected Mesh mesh; 

    protected List<PointMass> pointList;
    protected List<Constraint> constraintList;
    
    private bool isInitialized = false;
    private bool usesMeshFilter = true;
    private MonoBehaviour gravitationForce;
    #endregion

    #region methods
    public abstract void SupplyMesh();
    public abstract void GeneratePointMasses();
    public abstract void GenerateConstraints();
    public abstract void PreSimulationStep();
    public abstract void PostSimulationStep();

    // Use this for initialization
    void Start()
    {
        pointList = new List<PointMass>();
        constraintList = new List<Constraint>();
        meshFilter = GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            usesMeshFilter = false;
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

            if (skinnedMeshRenderer == null)
            {
                Debug.LogError("No Meshfilter/SkinnedMeshRenderer attached!");
                return;
            }
        }


        gravitationForce = gameObject.AddComponent<Gravitation>();
         
        SupplyMesh();
        GeneratePointMasses();
        GenerateConstraints();

        isInitialized = true;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        PreSimulationStep();

        //apply external forces
        for (int j = 0; j < pointList.Count; j++)
        {
            pointList[j].ApplyForce(((Force)gravitationForce).getForce());
        }

        //update constraints
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].Solve();
            }
        }

        //update pointmasses
        for (int i = 0; i < pointList.Count; i++)
        {
            pointList[i].Simulate(Time.deltaTime);     
        }

        //update the underlying mesh
        PostSimulationStep();      
        mesh.RecalculateBounds();
        if (usesMeshFilter)
        {
            meshFilter.mesh = mesh;
        }else
        {
            skinnedMeshRenderer.sharedMesh = mesh;
            
        }
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
