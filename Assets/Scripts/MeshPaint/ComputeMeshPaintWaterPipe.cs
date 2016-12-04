using UnityEngine;
using System.Collections;
using System;

public class ComputeMeshPaintWaterPipe : ComputeMeshModifier
{
    [Header("Water Painter Inputs")]
    [SerializeField]
    protected Texture2D initTexture;

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
    // Render textures
    protected RenderTexture waterHeights;
    protected RenderTexture tmpWaterHeight;
    protected RenderTexture terrainHeightmap;
    protected RenderTexture fluxLeft;
    protected RenderTexture fluxRight;
    protected RenderTexture fluxTop;
    protected RenderTexture fluxBottom;
    protected RenderTexture velocityX;
    protected RenderTexture velocityY;

    protected int fluxKernelHandle;
    #endregion
    #region Properties
    protected override int KERNEL_SIZE { get { return 32; } }

    protected const string KERNEL_METHOD_NAME = "Main";
    protected override string KERNEL_NAME { get { return KERNEL_METHOD_NAME; } }

    public RenderTexture FluxLeft { get { return fluxLeft; } }
    public RenderTexture FluxRight { get { return fluxRight; } }
    public RenderTexture FluxBottom { get { return fluxBottom; } }
    public RenderTexture FluxTop { get { return fluxTop; } }

    public RenderTexture WaterHeight { get { return waterHeights; } }
    public RenderTexture TerrainHeight { get { return terrainHeightmap; } }

    public RenderTexture VelocityX { get { return velocityX; } }
    public RenderTexture VelocityY { get { return velocityY; } }
    #endregion

    protected override void Start()
    {
        base.Start();
        squaredSegmentSize = segmentSize * segmentSize;
    }

    protected override void InitializeKernelHandle()
    {
        base.InitializeKernelHandle();
        fluxKernelHandle = fluxComputeShader.FindKernel(KERNEL_NAME);
    }

    protected override void InitializeRenderTextures()
    {
        //base.InitializeRenderTextures();

        // Water
        waterHeights = GetComputeRenderTexture(textureSize, 32);
        Graphics.Blit(originalTexture, waterHeights);
        tmpWaterHeight = GetComputeRenderTexture(textureSize, 32);
        Graphics.Blit(initTexture, tmpWaterHeight);

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
        fluxComputeShader.SetTexture(fluxKernelHandle, "WaterHeight", waterHeights);
        fluxComputeShader.SetTexture(fluxKernelHandle, "TerrainHeight", terrainHeightmap);

        fluxComputeShader.SetTexture(fluxKernelHandle, "FluxLeft", fluxLeft);
        fluxComputeShader.SetTexture(fluxKernelHandle, "FluxRight", fluxRight);
        fluxComputeShader.SetTexture(fluxKernelHandle, "FluxBottom", fluxBottom);
        fluxComputeShader.SetTexture(fluxKernelHandle, "FluxTop", fluxTop);

        fluxComputeShader.SetFloat("_DampingFactor", dampingFactor);
        fluxComputeShader.SetFloat("_HeightToFluxFactor", heightToFluxFactor);
        fluxComputeShader.SetFloat("_SegmentSizeSquared", squaredSegmentSize);
        fluxComputeShader.SetFloat("_MinWaterHeight", minWaterHeight);
        fluxComputeShader.SetInt("_TextureSize", textureSize);

        fluxComputeShader.Dispatch(fluxKernelHandle, textureSize / KERNEL_SIZE, textureSize / KERNEL_SIZE, 1);
    }

    private void ComputeWater()
    {
        computeShader.SetTexture(kernelHandleNumber, "WaterHeight", waterHeights);
        computeShader.SetTexture(kernelHandleNumber, "TempHeight", tmpWaterHeight);

        computeShader.SetTexture(kernelHandleNumber, "FluxLeft", fluxLeft);
        computeShader.SetTexture(kernelHandleNumber, "FluxRight", fluxRight);
        computeShader.SetTexture(kernelHandleNumber, "FluxBottom", fluxBottom);
        computeShader.SetTexture(kernelHandleNumber, "FluxTop", fluxTop);

        computeShader.SetTexture(kernelHandleNumber, "VelocityX", velocityX);
        computeShader.SetTexture(kernelHandleNumber, "VelocityY", velocityY);

        computeShader.SetFloat("_SegmentSizeSquared", squaredSegmentSize);
        computeShader.SetFloat("_SegmentSize", segmentSize);

        computeShader.SetFloat("_MinWaterHeight", minWaterHeight);
        computeShader.SetInt("_TextureSize", textureSize);

        computeShader.Dispatch(kernelHandleNumber, textureSize / KERNEL_SIZE, textureSize / KERNEL_SIZE, 1);

        // Copy temporary result back to the main water heightmap
        //Graphics.Blit(tmpWaterHeight, waterHeights);
        objectMaterial.SetTexture("_Heightmap", waterHeights);
    }
}