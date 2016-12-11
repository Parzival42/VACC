using UnityEngine;
using System.Collections;
using System;

public class WaterTerrainCombiner : ComputeMeshModifier
{
    #region Inernal members
    private ComputeMeshPaintWaterPipe pipeSimulation;
    private ComputeHeightmapPainter terrainSimulation;

    RenderTexture combinedWaterTerrain;
    #endregion

    #region Properties
    protected override int KERNEL_SIZE { get { return 32; } }

    protected const string KERNEL_METHOD_NAME = "Main";
    protected override string KERNEL_NAME { get { return KERNEL_METHOD_NAME; } }
    #endregion

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
        //base.InitializeKernelHandle();
    }

    protected override void ComputeValues()
    {
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_WATER_HEIGHT, pipeSimulation.WaterHeight);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_TERRAIN_HEIGHT, terrainSimulation.HeightMapTexture);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.INPUT_COMBINED_TERRAIN_WATER, combinedWaterTerrain);

        computeShader.Dispatch(kernelHandleNumber, pipeSimulation.TextureSize / KERNEL_SIZE, pipeSimulation.TextureSize / KERNEL_SIZE, 1);

        // TODO: Maybe blur water height a bit? (With compute shader)
        objectMaterial.SetTexture(ShaderConstants.PARAM_WATER_HEIGHT, pipeSimulation.WaterHeight);
        objectMaterial.SetTexture(ShaderConstants.PARAM_TERRAIN_HEIGHT, terrainSimulation.HeightMapTexture);
        objectMaterial.SetTexture(ShaderConstants.PARAM_COMBINED_TERRAIN_WATER, combinedWaterTerrain);
        objectMaterial.SetTexture(ShaderConstants.PARAM_VELOCITY_X, pipeSimulation.VelocityX);
        objectMaterial.SetTexture(ShaderConstants.PARAM_VELOCITY_Y, pipeSimulation.VelocityY);
    }

    protected override void InitializeRenderTextures()
    {
        combinedWaterTerrain = GetComputeRenderTexture(pipeSimulation.TextureSize, 32);
        Graphics.Blit(originalTexture, combinedWaterTerrain);
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
}