using System;
using UnityEngine;

public class ComputeMeshPaintWaterPipe : ComputeMeshModifier
{
    [Header("Water Painter Inputs")]
    [SerializeField]
    protected Texture2D initTexture;

    [SerializeField]
    protected Texture2D boundaryTexture;

    [Header("Brush settings")]
    [SerializeField]
    private float brushRadius = 0.25f;

    [SerializeField]
    private float brushStrength = 0.03f;

    [SerializeField]
    private float brushFalloff = 0.8f;

    [Header("Render Texture settings")]
    [SerializeField]
    protected int textureSize = 512;

    [Header("Flux settings")]
    [SerializeField]
    private ComputeShader fluxComputeShader;

    [SerializeField]
    protected float dampingFactor = 0.995f;

    [SerializeField]
    protected float heightToFluxFactor = 1f;

    [SerializeField]
    protected float segmentSize = 1f;
    private float squaredSegmentSize;

    [SerializeField]
    protected float minWaterHeight = 1f;

    [Header("Terrain")]
    [SerializeField]
    private Texture2D staticTerrainHeightmap;

    #region Internal members
    private ComputeHeightmapPainter heightmapPainter;

    // Render textures
    protected RenderTexture waterHeights;
    protected RenderTexture terrainHeightmap;
    protected RenderTexture fluxLeft;
    protected RenderTexture fluxRight;
    protected RenderTexture fluxTop;
    protected RenderTexture fluxBottom;
    protected RenderTexture velocityX;
    protected RenderTexture velocityY;

    protected int fluxKernelHandle;
    protected Vector3 uvHit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    #endregion
    #region Properties
    public int TextureSize { get { return textureSize; } }
    public Vector3 UVHit { get { return uvHit; } set { uvHit = value; } }
    protected override int KERNEL_SIZE { get { return 32; } }

    protected const string KERNEL_METHOD_NAME = "Main";
    protected override string KERNEL_NAME { get { return KERNEL_METHOD_NAME; } }

    public RenderTexture FluxLeft { get { return fluxLeft; } }
    public RenderTexture FluxRight { get { return fluxRight; } }
    public RenderTexture FluxBottom { get { return fluxBottom; } }
    public RenderTexture FluxTop { get { return fluxTop; } }

    public RenderTexture WaterHeight { get { return waterHeights; } }
    public RenderTexture TerrainHeight { get { return heightmapPainter == null ? terrainHeightmap : heightmapPainter.HeightMapTexture; } }

    public RenderTexture VelocityX { get { return velocityX; } }
    public RenderTexture VelocityY { get { return velocityY; } }
    #endregion

    protected override void Start()
    {
        base.Start();
        heightmapPainter = FindObjectOfType<ComputeHeightmapPainter>();
        squaredSegmentSize = segmentSize * segmentSize;
    }

    protected override void InitializeKernelHandle()
    {
        base.InitializeKernelHandle();
        fluxKernelHandle = fluxComputeShader.FindKernel(KERNEL_NAME);
        // Only upload the boundary texture once
        fluxComputeShader.SetTexture(fluxKernelHandle, ShaderConstants.INPUT_BOUNDARY_TEXTURE, boundaryTexture);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_BOUNDARY_TEXTURE, boundaryTexture);
    }

    protected override void InitializeRenderTextures()
    {
        // Water
        waterHeights = GetComputeRenderTexture(textureSize, 32);
        Graphics.Blit(originalTexture, waterHeights);

        // Terrain
        terrainHeightmap = GetComputeRenderTexture(textureSize, 32);
        Graphics.Blit(staticTerrainHeightmap, terrainHeightmap);

        // Flux
        fluxLeft = GetComputeRenderTexture(textureSize, 32);
        fluxRight = GetComputeRenderTexture(textureSize, 32);
        fluxTop = GetComputeRenderTexture(textureSize, 32);
        fluxBottom = GetComputeRenderTexture(textureSize, 32);

        // Velocity
        velocityX = GetComputeRenderTexture(textureSize, 32);
        velocityY = GetComputeRenderTexture(textureSize, 32);

        // Init flux and velocity with black
        Graphics.Blit(initTexture, fluxLeft);
        Graphics.Blit(initTexture, fluxRight);
        Graphics.Blit(initTexture, fluxTop);
        Graphics.Blit(initTexture, fluxBottom);
        Graphics.Blit(initTexture, velocityX);
        Graphics.Blit(initTexture, velocityY);
    }

    protected override void ComputeValues()
    {
        // Calculate Flux
        ComputeFlux();

        // Calculate Water
        ComputeWater();
    }

    private void ComputeFlux()
    {
        fluxComputeShader.SetTexture(fluxKernelHandle, ShaderConstants.INPUT_WATER_HEIGHT, waterHeights);
        
        if (heightmapPainter == null)
            fluxComputeShader.SetTexture(fluxKernelHandle, ShaderConstants.INPUT_TERRAIN_HEIGHT, terrainHeightmap);
        else
            fluxComputeShader.SetTexture(fluxKernelHandle, ShaderConstants.INPUT_TERRAIN_HEIGHT, heightmapPainter.HeightMapTexture);

        fluxComputeShader.SetTexture(fluxKernelHandle, ShaderConstants.INPUT_FLUX_LEFT, fluxLeft);
        fluxComputeShader.SetTexture(fluxKernelHandle, ShaderConstants.INPUT_FLUX_RIGHT, fluxRight);
        fluxComputeShader.SetTexture(fluxKernelHandle, ShaderConstants.INPUT_FLUX_BOTTOM, fluxBottom);
        fluxComputeShader.SetTexture(fluxKernelHandle, ShaderConstants.INPUT_FLUX_TOP, fluxTop);

        fluxComputeShader.SetFloat(ShaderConstants.PARAM_DELTA_TIME, Time.deltaTime);
        fluxComputeShader.SetFloat(ShaderConstants.PARAM_DAMPING_FACTOR, dampingFactor);
        fluxComputeShader.SetFloat(ShaderConstants.PARAM_HEIGHT_TO_FLUX_FACTOR, heightToFluxFactor);
        fluxComputeShader.SetFloat(ShaderConstants.PARAM_SEGMENT_SIZE_SQUARED, squaredSegmentSize);
        fluxComputeShader.SetFloat(ShaderConstants.PARAM_MIN_WATER_HEIGHT, minWaterHeight);
        fluxComputeShader.SetInt(ShaderConstants.PARAM_TEXTURE_SIZE, textureSize);

        fluxComputeShader.Dispatch(fluxKernelHandle, textureSize / KERNEL_SIZE, textureSize / KERNEL_SIZE, 1);
    }

    private void ComputeWater()
    {
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_WATER_HEIGHT, waterHeights);

        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_FLUX_LEFT, fluxLeft);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_FLUX_RIGHT, fluxRight);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_FLUX_BOTTOM, fluxBottom);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_FLUX_TOP, fluxTop);

        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_VELOCITY_X, velocityX);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_VELOCITY_Y, velocityY);
        
        computeShader.SetVector(ShaderConstants.PARAM_UV_HIT, uvHit);
        computeShader.SetFloat(ShaderConstants.PARAM_SEGMENT_SIZE_SQUARED, squaredSegmentSize);
        computeShader.SetFloat(ShaderConstants.PARAM_SEGMENT_SIZE, segmentSize);
        computeShader.SetFloat(ShaderConstants.PARAM_DELTA_TIME, Time.deltaTime);

        computeShader.SetFloat(ShaderConstants.PARAM_BRUSH_FALLOFF, brushFalloff);
        computeShader.SetFloat(ShaderConstants.PARAM_BRUSH_STRENGTH, brushStrength);
        computeShader.SetFloat(ShaderConstants.PARAM_RADIUS, brushRadius);
        computeShader.SetFloat(ShaderConstants.PARAM_MIN_WATER_HEIGHT, minWaterHeight);
        computeShader.SetInt(ShaderConstants.PARAM_TEXTURE_SIZE, textureSize);

        computeShader.Dispatch(kernelHandleNumber, textureSize / KERNEL_SIZE, textureSize / KERNEL_SIZE, 1);

        if(!onlyCompute)
            objectMaterial.SetTexture(ShaderConstants.PARAM_HEIGHTMAP, waterHeights);
    }

    public override void InvertMeshModification()
    {
        brushStrength *= -1f;
    }
}