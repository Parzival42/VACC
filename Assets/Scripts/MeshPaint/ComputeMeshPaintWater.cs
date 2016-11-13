﻿using UnityEngine;

public class ComputeMeshPaintWater : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField]
    private Texture originalTexture;

    [SerializeField]
    private ComputeShader computeShader;

    [SerializeField]
    private Texture2D initTexture;

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

    private RenderTexture waterHeights;
    private RenderTexture waterVelocity;
    private RenderTexture tmpHeight;

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

        //vertexBuffer = new ComputeBuffer(mesh.mesh.vertices.Length, 3 * 4);      // 3 Floats * 4 Bytes
        //uvBuffer = new ComputeBuffer(mesh.mesh.uv.Length, 2 * 4);                // 2 Floats * 4 Bytes
    }

    private void InitializeRenderTextures()
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
        //Graphics.Blit(originalTexture, otherRenderTexture);
    }

    private void Update ()
    {
        // TODO: Replace strings with constant strings.
        /*computeShader.SetBuffer(kernelHandle, "MeshVertices", vertexBuffer);
        computeShader.SetBuffer(kernelHandle, "MeshUvs", uvBuffer);*/
        computeShader.SetInt("_TextureSize", renderTextureWidth);
        computeShader.SetVector("_UvHit", uvHit);
        computeShader.SetFloat("_Radius", brushRadius);
        computeShader.SetFloat("_BrushStrength", brushStrength);

        computeShader.SetTexture(kernelHandle, "WaterHeight", waterHeights);
        computeShader.SetTexture(kernelHandle, "VelocityField", waterVelocity);
        computeShader.SetTexture(kernelHandle, "TempHeight", tmpHeight);

        computeShader.Dispatch(kernelHandle, waterHeights.width / KERNEL_SIZE, waterHeights.height / KERNEL_SIZE, 1);

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

    private void OnDestroy()
    {
        //vertexBuffer.Dispose();
        //uvBuffer.Dispose();
    }
    #endregion
}