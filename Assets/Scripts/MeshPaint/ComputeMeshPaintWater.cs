using System;
using UnityEngine;

public class ComputeMeshPaintWater : ComputeMeshModifier
{
    [Header("Water Painter Inputs")]
    [SerializeField]
    private Texture2D initTexture;

    [SerializeField]
    private float simulationSpeed = 0.3f;

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
    private Vector3 uvHit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    private RenderTexture waterHeights;
    private RenderTexture waterVelocity;
    private RenderTexture tmpHeight;

    private MeshFilter mesh;
    private MeshCollider meshCollider;

    protected override int KERNEL_SIZE { get { return 16; } }

    private const string KERNEL_METHOD_NAME = "Main";
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
        waterHeights = new RenderTexture(renderTextureWidth, renderTextureHeight, 32, RenderTextureFormat.RFloat);
        waterHeights.enableRandomWrite = true;
        waterHeights.Create();

        waterVelocity = new RenderTexture(renderTextureWidth, renderTextureHeight, 32, RenderTextureFormat.RFloat);
        waterVelocity.enableRandomWrite = true;
        waterVelocity.Create();

        tmpHeight = new RenderTexture(renderTextureWidth, renderTextureHeight, 32, RenderTextureFormat.RFloat);
        tmpHeight.enableRandomWrite = true;
        tmpHeight.Create();

        Graphics.Blit(originalTexture, waterHeights);
        Graphics.Blit(initTexture, waterVelocity);
        Graphics.Blit(initTexture, tmpHeight);
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
        computeShader.SetTexture(kernelHandleNumber, "TempHeight", tmpHeight);

        computeShader.Dispatch(kernelHandleNumber, waterHeights.width / KERNEL_SIZE, waterHeights.height / KERNEL_SIZE, 1);

        // Copy temporary result back to the main water heightmap
        Graphics.Blit(tmpHeight, waterHeights);
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