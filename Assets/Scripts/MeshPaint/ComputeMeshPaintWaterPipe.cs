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
    protected int renderTextureWidth = 512;

    [SerializeField]
    protected int renderTextureHeight = 512;

    [Header("Flux settings")]
    [SerializeField]
    private ComputeShader fluxComputeShader;

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
    protected RenderTexture fluxDown;
    protected RenderTexture velocityX;
    protected RenderTexture velocityY;

    protected int fluxKernelHandle;
    #endregion
    protected override int KERNEL_SIZE { get { return 16; } }

    protected const string KERNEL_METHOD_NAME = "Main";
    protected override string KERNEL_NAME { get { return KERNEL_METHOD_NAME; } }

    protected override void InitializeKernelHandle()
    {
        base.InitializeKernelHandle();
        fluxKernelHandle = computeShader.FindKernel(KERNEL_NAME);
    }

    protected override void InitializeRenderTextures()
    {
        //base.InitializeRenderTextures();

        // Water
        waterHeights = GetComputeRenderTexture(renderTextureWidth, 32);
        Graphics.Blit(originalTexture, waterHeights);
        tmpWaterHeight = GetComputeRenderTexture(renderTextureWidth, 32);
        Graphics.Blit(initTexture, tmpWaterHeight);

        // Terrain
        terrainHeightmap = GetComputeRenderTexture(renderTextureWidth, 32);
        Graphics.Blit(staticTerrainHeightmap, terrainHeightmap);

        // Flux
        fluxLeft = GetComputeRenderTexture(renderTextureWidth, 32);
        fluxRight = GetComputeRenderTexture(renderTextureWidth, 32);
        fluxTop = GetComputeRenderTexture(renderTextureWidth, 32);
        fluxDown = GetComputeRenderTexture(renderTextureWidth, 32);

        // Velocity
        velocityX = GetComputeRenderTexture(renderTextureWidth, 32);
        velocityY = GetComputeRenderTexture(renderTextureWidth, 32);

        // Init flux and velocity with black
        Graphics.Blit(initTexture, fluxLeft);
        Graphics.Blit(initTexture, fluxRight);
        Graphics.Blit(initTexture, fluxTop);
        Graphics.Blit(initTexture, fluxDown);
        Graphics.Blit(initTexture, velocityX);
        Graphics.Blit(initTexture, velocityY);
    }

    protected override void ComputeValues()
    {
        // Calculate Flux
        

        // Calculate Water
    }
}