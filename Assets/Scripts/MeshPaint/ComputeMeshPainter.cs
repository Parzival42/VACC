using UnityEngine;

public class ComputeMeshPainter : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField]
    private Texture originalTexture;

    [SerializeField]
    private ComputeShader computeShader;

    [Header("Brush settings")]
    [SerializeField]
    private float brushRadius = 0.05f;

    [SerializeField]
    private float brushStrength = 0.02f;

    [Header("Render Texture settings")]
    [SerializeField]
    private int renderTextureWidth = 512;

    [SerializeField]
    private int renderTextureHeight = 512;

    #region Internal members
    private const int KERNEL_SIZE = 32;
    private const string KERNEL_NAME = "Main";

    private int kernelHandle;

    private Vector3 uvHit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    private Material objectMaterial;

    private RenderTexture renderTexture;

    private MeshFilter mesh;
    private MeshCollider meshCollider;

    private ComputeBuffer vertexBuffer;
    private ComputeBuffer uvBuffer;
    #endregion

    #region Properties
    #endregion

    private void Start()
    {
        kernelHandle = computeShader.FindKernel(KERNEL_NAME);
        InitializeComponents();
        InitializeRenderTextures();
    }

    private void InitializeComponents()
    {
        mesh = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        objectMaterial = GetComponent<Renderer>().material;

        vertexBuffer = new ComputeBuffer(mesh.mesh.vertices.Length, 3 * 4);      // 3 Floats * 4 Bytes
        vertexBuffer.SetData(mesh.mesh.vertices);
        uvBuffer = new ComputeBuffer(mesh.mesh.uv.Length, 2 * 4);                // 2 Floats * 4 Bytes
    }

    private void InitializeRenderTextures()
    {
        renderTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 32, RenderTextureFormat.RFloat);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        Graphics.Blit(originalTexture, renderTexture);
    }

    private void Update ()
    {
        // TODO: Replace strings with constant strings.
        computeShader.SetBuffer(kernelHandle, "MeshVertices", vertexBuffer);
        computeShader.SetBuffer(kernelHandle, "MeshUvs", uvBuffer);
        computeShader.SetInt("_TextureSize", renderTextureWidth);
        computeShader.SetVector("_UvHit", uvHit);
        computeShader.SetFloat("_Radius", brushRadius);
        computeShader.SetFloat("_BrushStrength", brushStrength);
        computeShader.SetTexture(kernelHandle, "Result", renderTexture);

        computeShader.Dispatch(kernelHandle, renderTexture.width / KERNEL_SIZE, renderTexture.height / KERNEL_SIZE, 1);

        objectMaterial.SetTexture("_Heightmap", renderTexture);
        objectMaterial.SetTexture("_MainTex", renderTexture);

        Vector3[] vertices = new Vector3[mesh.mesh.vertices.Length];
        vertexBuffer.GetData(vertices);
        mesh.mesh.vertices = vertices;
        //mesh.mesh.RecalculateBounds();
        //mesh.mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh.mesh;
    }

    #region Mouse methods
    private void OnMouseDrag()
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

    private void OnMouseUp()
    {
        uvHit.Set(float.MaxValue, float.MaxValue, float.MaxValue);
    }

    private void OnDestroy()
    {
        vertexBuffer.Dispose();
        uvBuffer.Dispose();
    }
    #endregion
}