using UnityEngine;
using System.Collections;
using System;

public class WaterTerrainCombiner : ComputeMeshModifier
{
    #region Inernal members
    private ComputeMeshPaintWaterPipe pipeSimulation;
    private ComputeHeightmapPainter terrainSimulation;
    #endregion

    #region Properties
    protected override int KERNEL_SIZE { get { return 32; } }

    protected const string KERNEL_METHOD_NAME = "Main";
    protected override string KERNEL_NAME { get { return KERNEL_METHOD_NAME; } }
    #endregion

    protected override void Start()
    {
        base.Start();

        pipeSimulation = FindObjectOfType<ComputeMeshPaintWaterPipe>();
        terrainSimulation = FindObjectOfType<ComputeHeightmapPainter>();

        if (pipeSimulation == null)
            Debug.LogError("No water simulation in scene!");
        if (terrainSimulation == null)
            Debug.LogError("No terrain simulation in scene!");
    }

    protected override void InitializeKernelHandle()
    {
        //base.InitializeKernelHandle();
    }

    protected override void ComputeValues()
    {
        // TODO: Maybe blur water height a bit? (With compute shader)
        objectMaterial.SetTexture("_WaterHeight", pipeSimulation.WaterHeight);
        objectMaterial.SetTexture("_TerrainHeight", terrainSimulation.HeightMapTexture);
    }

    protected override void InitializeRenderTextures()
    {
        // Nothing to do here
    }
}
