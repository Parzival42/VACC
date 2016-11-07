using UnityEngine;
using System.Collections;

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
    private RenderTexture copyTexture;
    #endregion

    #region Properties
    #endregion

    private void Start ()
    {
        kernelHandle = computeShader.FindKernel(KERNEL_NAME);
        objectMaterial = GetComponent<Renderer>().material;
        InitializeRenderTextures();

    }

    private void InitializeRenderTextures()
    {
        renderTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 32, RenderTextureFormat.RFloat);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        copyTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 32, RenderTextureFormat.RFloat);
        copyTexture.enableRandomWrite = true;
        copyTexture.Create();

        Graphics.Blit(originalTexture, renderTexture);
    }

    private void Update ()
    {
        // TODO: Replace strings with constant strings.
        computeShader.SetInt("_TextureSize", renderTextureWidth);
        computeShader.SetVector("_UvHit", uvHit);
        computeShader.SetFloat("_Radius", brushRadius);
        computeShader.SetFloat("_BrushStrength", brushStrength);
        computeShader.SetTexture(kernelHandle, "Result", renderTexture);

        computeShader.Dispatch(kernelHandle, renderTexture.width / KERNEL_SIZE, renderTexture.height / KERNEL_SIZE, 1);

        objectMaterial.SetTexture("_Heightmap", renderTexture);
        objectMaterial.SetTexture("_MainTex", renderTexture);
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
    #endregion
}