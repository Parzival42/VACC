using UnityEngine;

public class ComputeMeshPaintWater : ComputeMeshModifier
{
    [Header("Water Painter Inputs")]
    [SerializeField]
    protected Texture2D initTexture;

    [SerializeField]
    protected float simulationSpeed = 0.3f;

    [Header("Brush settings")]
    [SerializeField]
    protected float brushRadius = 0.05f;

    [SerializeField]
    protected float brushStrength = 0.02f;

    [Header("Render Texture settings")]
    [SerializeField]
    protected int renderTextureWidth = 512;

    [SerializeField]
    protected int renderTextureHeight = 512;

    #region Internal members
    protected Vector3 uvHit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    protected RenderTexture waterHeights;
    protected RenderTexture waterVelocity;
    protected RenderTexture tmpWaterHeight;

    protected MeshFilter mesh;
    protected MeshCollider meshCollider;

    protected override int KERNEL_SIZE { get { return 16; } }

    protected const string KERNEL_METHOD_NAME = "Main";
    protected override string KERNEL_NAME { get { return KERNEL_METHOD_NAME; } }
    #endregion

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        mesh = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    protected override void InitializeRenderTextures()
    {
        waterHeights = GetComputeRenderTexture(renderTextureWidth, 32);
        waterVelocity = GetComputeRenderTexture(renderTextureWidth, 32);
        tmpWaterHeight = GetComputeRenderTexture(renderTextureWidth, 32);

        Graphics.Blit(originalTexture, waterHeights);
        Graphics.Blit(initTexture, waterVelocity);
        Graphics.Blit(initTexture, tmpWaterHeight);
    }

    protected override void ComputeValues()
    {
        // TODO: Replace strings with constant strings.
        computeShader.SetInt("_TextureSize", renderTextureWidth);
        computeShader.SetVector("_UvHit", uvHit);
        computeShader.SetFloat("_Radius", brushRadius);
        computeShader.SetFloat("_BrushStrength", brushStrength);
        computeShader.SetFloat("_DeltaTime", Time.deltaTime);
        computeShader.SetFloat("_Speed", simulationSpeed);


        computeShader.SetTexture(kernelHandleNumber, "WaterHeight", waterHeights);
        computeShader.SetTexture(kernelHandleNumber, "VelocityField", waterVelocity);
        computeShader.SetTexture(kernelHandleNumber, "TempHeight", tmpWaterHeight);

        computeShader.Dispatch(kernelHandleNumber, waterHeights.width / KERNEL_SIZE, waterHeights.height / KERNEL_SIZE, 1);

        // Copy temporary result back to the main water heightmap
        Graphics.Blit(tmpWaterHeight, waterHeights);
        objectMaterial.SetTexture("_Heightmap", waterHeights);
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
        }
    }

    private void OnMouseUp()
    {
        uvHit.Set(float.MaxValue, float.MaxValue, float.MaxValue);
    }
    #endregion
}