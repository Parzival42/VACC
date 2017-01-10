using System;
using UnityEngine;

public class ComputeHeightmapPainter : ComputeMeshModifier
{
    [Header("Brush settings")]
    [SerializeField]
    private float brushRadius = 0.05f;

    [SerializeField]
    private float brushStrength = 0.02f;

    [SerializeField]
    private float brushFalloff = 0.8f;

    [Header("Render Texture settings")]
    [SerializeField]
    private int textureSize = 512;

    [Header("Collider compute Shader")]
    [SerializeField]
    private ComputeShader colliderCompute;

    #region Internal members
    private int colliderComputerKernelHandle;

    private Mesh tempMesh;
    private Vector3 uvHit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    private RenderTexture renderTexture;
    private RenderTexture colliderRenderTexture;

    private MeshFilter mesh;
    private MeshCollider meshCollider;

    private ComputeBuffer vertexBuffer;
    private ComputeBuffer uvBuffer;

    protected override int KERNEL_SIZE { get { return 32; } }

    private const string KERNEL_METHOD_NAME = "Main";
    protected override string KERNEL_NAME { get { return KERNEL_METHOD_NAME; } }
    public RenderTexture HeightMapTexture { get { return renderTexture; } }
    public Vector3 UVHit { get { return uvHit; } set { uvHit = value; } }
    #endregion

    private void Awake()
    {
        tempMesh = new Mesh();
    }

    protected override void InitializeKernelHandle()
    {
        base.InitializeKernelHandle();
        colliderComputerKernelHandle = colliderCompute.FindKernel(KERNEL_NAME);
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        mesh = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        vertexBuffer = new ComputeBuffer(mesh.mesh.vertices.Length, 3 * 4);      // 3 Floats * 4 Bytes
        vertexBuffer.SetData(mesh.mesh.vertices);

        tempMesh.vertices = mesh.mesh.vertices;
        tempMesh.triangles = mesh.mesh.triangles;
        tempMesh.uv = mesh.mesh.uv;

        uvBuffer = new ComputeBuffer(mesh.mesh.uv.Length, 2 * 4);                // 2 Floats * 4 Bytes
    }

    protected override void InitializeRenderTextures()
    {
        renderTexture = GetComputeRenderTexture(textureSize, 32);
        colliderRenderTexture = GetComputeRenderTexture(32 + 1, 32);

        Graphics.Blit(originalTexture, renderTexture);
        Graphics.Blit(originalTexture, colliderRenderTexture);
    }

    protected override void ComputeValues()
    {
        // TODO: Replace strings with constant strings.
        computeShader.SetInt("_TextureSize", textureSize);
        computeShader.SetVector("_UvHit", uvHit);
        computeShader.SetFloat("_Radius", brushRadius);
        computeShader.SetFloat("_Falloff", brushFalloff);
        computeShader.SetFloat("_BrushStrength", brushStrength);
        computeShader.SetTexture(kernelHandleNumber, "Result", renderTexture);

        colliderCompute.SetBuffer(colliderComputerKernelHandle, "MeshVertices", vertexBuffer);
        colliderCompute.SetInt("_TextureSize", colliderRenderTexture.width);
        colliderCompute.SetTexture(colliderComputerKernelHandle, "ColliderResult", colliderRenderTexture);

        computeShader.Dispatch(kernelHandleNumber, renderTexture.width / KERNEL_SIZE, renderTexture.height / KERNEL_SIZE, 1);
        colliderCompute.Dispatch(colliderComputerKernelHandle, colliderRenderTexture.width / KERNEL_SIZE, colliderRenderTexture.height / KERNEL_SIZE, 1);

        objectMaterial.SetTexture("_Heightmap", renderTexture);
        objectMaterial.SetTexture("_MainTex", renderTexture);

        Vector3[] vertices = new Vector3[tempMesh.vertices.Length];
        vertexBuffer.GetData(vertices);
        tempMesh.vertices = vertices;
        meshCollider.sharedMesh = tempMesh;

        Graphics.Blit(renderTexture, colliderRenderTexture);
    }

    #region Mouse methods
    private void OnMouseDrag()
    {
        if (!onlyCompute)
        {
            Vector3 hit;
            RaycastHit rayHit;
            if (Camera.main.CollisionFor(Camera.main.ScreenPointToRayFor(Input.mousePosition), 8, out hit, out rayHit))
            {
                // This only works with a Mesh Collider!!!
                uvHit = rayHit.textureCoord;
                //Debug.Log(uvHit);
            }
        }
    }

    private void OnMouseUp()
    {
        if (!onlyCompute)
        {
            uvHit.Set(float.MaxValue, float.MaxValue, float.MaxValue);
        }
    }

    private void OnDestroy()
    {
        vertexBuffer.Dispose();
        uvBuffer.Dispose();
    }

    public override void InvertMeshModification()
    {
        brushStrength *= -1f;
    }
    #endregion
}