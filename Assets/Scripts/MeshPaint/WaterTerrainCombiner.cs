using UnityEngine;

public class WaterTerrainCombiner : ComputeMeshModifier
{
    #region Inspector variables
    [Header("Mesh collider computation")]
    [SerializeField]
    private int colliderTextureSize = 32;

    [SerializeField]
    private float colliderMeshHeightMultiplier = 6f;

    [SerializeField]
    private float waterColliderDecreaseOffset = 0.6f;

    [SerializeField]
    private ComputeShader colliderComputeShader;
    #endregion

    #region Inernal members
    private ComputeMeshPaintWaterPipe pipeSimulation;
    private ComputeHeightmapPainter terrainSimulation;

    RenderTexture combinedWaterTerrain;

    // Is used to compute the mesh collider vertices. Only use the RT with the same size of the mesh plane!
    // Example: Mesh Plane --> 32 x 32 -> RT must also have the same size!
    private RenderTexture colliderRenderTexture;
    private RenderTexture colliderWaterRenderTexture;
    #endregion

    #region Properties
    private MeshFilter mesh;
    private Mesh tempMesh;
    private MeshCollider meshCollider;
    private ComputeBuffer colliderVertexBuffer;

    private int colliderComputeKernelHandle;

    protected override int KERNEL_SIZE { get { return 32; } }

    protected const string KERNEL_METHOD_NAME = "Main";
    protected override string KERNEL_NAME { get { return KERNEL_METHOD_NAME; } }
    #endregion

    private void Awake()
    {
        tempMesh = new Mesh();
    }

    protected override void Start()
    {
        pipeSimulation = FindObjectOfType<ComputeMeshPaintWaterPipe>();
        terrainSimulation = FindObjectOfType<ComputeHeightmapPainter>();

        if (pipeSimulation == null)
            Debug.LogError("No water simulation in scene!");
        if (terrainSimulation == null)
            Debug.LogError("No terrain simulation in scene!");

        base.Start();
    }

    protected override void InitializeKernelHandle()
    {
        colliderComputeKernelHandle = colliderComputeShader.FindKernel(KERNEL_NAME);
        //base.InitializeKernelHandle();
    }

    protected override void ComputeValues()
    {
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_WATER_HEIGHT, pipeSimulation.WaterHeight);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_TERRAIN_HEIGHT, terrainSimulation.HeightMapTexture);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_COMBINED_TERRAIN_WATER, combinedWaterTerrain);

        colliderComputeShader.SetBuffer(colliderComputeKernelHandle, ShaderConstants.INPUT_MESH_VERTICES, colliderVertexBuffer);
        colliderComputeShader.SetFloat(ShaderConstants.PARAM_TERRAIN_HEIGHT, colliderMeshHeightMultiplier);
        colliderComputeShader.SetFloat(ShaderConstants.PARAM_WATER_DECREASE_OFFSET, waterColliderDecreaseOffset);
        colliderComputeShader.SetTexture(colliderComputeKernelHandle, ShaderConstants.INPUT_COLLIDER_RESULT, colliderRenderTexture);
        colliderComputeShader.SetTexture(colliderComputeKernelHandle, ShaderConstants.INPUT_WATER_HEIGHT, colliderWaterRenderTexture);

        computeShader.Dispatch(kernelHandleNumber, pipeSimulation.TextureSize / KERNEL_SIZE, pipeSimulation.TextureSize / KERNEL_SIZE, 1);
        colliderComputeShader.Dispatch(colliderComputeKernelHandle, colliderRenderTexture.width / KERNEL_SIZE, colliderRenderTexture.height / KERNEL_SIZE, 1);

        // TODO: Maybe blur water height a bit? (With compute shader)
        objectMaterial.SetTexture(ShaderConstants.PARAM_WATER_HEIGHT, pipeSimulation.WaterHeight);
        objectMaterial.SetTexture(ShaderConstants.PARAM_TERRAIN_HEIGHT, terrainSimulation.HeightMapTexture);
        objectMaterial.SetTexture(ShaderConstants.PARAM_COMBINED_TERRAIN_WATER, combinedWaterTerrain);
        objectMaterial.SetTexture(ShaderConstants.PARAM_FLUX_LEFT, pipeSimulation.FluxLeft);
        objectMaterial.SetTexture(ShaderConstants.PARAM_FLUX_RIGHT, pipeSimulation.FluxRight);
        objectMaterial.SetTexture(ShaderConstants.PARAM_FLUX_TOP, pipeSimulation.FluxTop);
        objectMaterial.SetTexture(ShaderConstants.PARAM_FLUX_BOTTOM, pipeSimulation.FluxBottom);

        if (!onlyCompute)
        {
            AssignMeshColliderData();
            Graphics.Blit(combinedWaterTerrain, colliderRenderTexture);
            Graphics.Blit(pipeSimulation.WaterHeight, colliderWaterRenderTexture);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pipeSimulation.InvertMeshModification();
            terrainSimulation.InvertMeshModification();
        }
    }

    protected void AssignMeshColliderData()
    {
        Vector3[] vertices = new Vector3[tempMesh.vertices.Length];
        colliderVertexBuffer.GetData(vertices);
        tempMesh.vertices = vertices;
        meshCollider.sharedMesh = tempMesh;
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        mesh = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        colliderVertexBuffer = new ComputeBuffer(mesh.mesh.vertices.Length, 3 * 4);      // 3 Floats * 4 Bytes
        colliderVertexBuffer.SetData(mesh.mesh.vertices);

        tempMesh.vertices = mesh.mesh.vertices;
        tempMesh.triangles = mesh.mesh.triangles;
        tempMesh.uv = mesh.mesh.uv;
    }

    protected override void InitializeRenderTextures()
    {
        combinedWaterTerrain = GetComputeRenderTexture(pipeSimulation.TextureSize, 32);
        // + 1 because of the use of Plane Generator script (Generates 32 x 32 cells -> 1 Vertex more)
        colliderRenderTexture = GetComputeRenderTexture(colliderTextureSize + 1, 32);
        colliderWaterRenderTexture = GetComputeRenderTexture(colliderTextureSize + 1, 32);

        // Blit original texture into RT's
        Graphics.Blit(originalTexture, combinedWaterTerrain);
        Graphics.Blit(originalTexture, colliderRenderTexture);
        Graphics.Blit(pipeSimulation.WaterHeight, colliderWaterRenderTexture);
    }

    private void OnMouseDrag()
    {
        Vector3 hit;
        RaycastHit rayHit;
        if (Camera.main.CollisionFor(Camera.main.ScreenPointToRayFor(Input.mousePosition), 8, out hit, out rayHit))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                pipeSimulation.UVHit = rayHit.textureCoord;
            }
            else
            {
                terrainSimulation.UVHit = rayHit.textureCoord;
            }
        }
    }

    private void OnMouseUp()
    {
        terrainSimulation.UVHit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        pipeSimulation.UVHit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    }

    public override void InvertMeshModification()
    {
        // Nothing to do here.
    }

    private void OnDestroy()
    {
        colliderVertexBuffer.Dispose();
    }
}