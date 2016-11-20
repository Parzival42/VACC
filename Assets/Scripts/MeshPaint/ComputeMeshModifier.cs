using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract base class for the mesh modification based on Compute Shader.
/// </summary>
public abstract class ComputeMeshModifier : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField]
    protected Texture originalTexture;

    [SerializeField]
    protected ComputeShader computeShader;

    #region Internal used fields
    protected Material objectMaterial;
    protected int kernelHandleNumber;
    #endregion

    #region Properties
    protected abstract int KERNEL_SIZE { get; }
    protected abstract string KERNEL_NAME { get; }
    #endregion

    #region Abstract Methods
    protected abstract void InitializeRenderTextures();

    /// <summary>
    /// Do all compute shader calculations here.
    /// </summary>
    protected abstract void ComputeValues();
    #endregion

    protected virtual void Start ()
    {
        InitializeKernelHandle();
        InitializeComponents();
        InitializeRenderTextures();
	}

    private void Update()
    {
        ComputeValues();
    }

    /// <summary>
    /// Used to get components of the GameObject (Material, MeshFilter, Collider, ...)
    /// </summary>
    protected virtual void InitializeComponents()
    {
        // Get material of object
        objectMaterial = GetComponent<Renderer>().material;
        if (objectMaterial == null)
            Debug.LogError("Error during material fetching.");
    }

    protected virtual void InitializeKernelHandle()
    {
        kernelHandleNumber = computeShader.FindKernel(KERNEL_NAME);
    }
}