using System;
using UnityEngine;

public class ComputeColorPainter : ComputeMeshModifier, PaintDataReceiver
{
    [Header("Brush settings")]
    [SerializeField]
    private float brushRadius = 0.05f;

    [SerializeField]
    private float brushStrength = 0.02f;

    [SerializeField]
    private float brushFalloff = 0.8f;

    [Header("Render Texture settings")]
    [SerializeField]
    private int renderTextureWidth = 512;

    [SerializeField]
    private int renderTextureHeight = 512;

    #region Internal members
    private Vector3 uvHit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    private RenderTexture renderTexture;

    private MeshFilter mesh;
    private MeshCollider meshCollider;

    protected override int KERNEL_SIZE { get { return 32; } }

    private const string KERNEL_METHOD_NAME = "Main";
    protected override string KERNEL_NAME { get { return KERNEL_METHOD_NAME; } }

    public float Radius
    {
        get { return brushRadius; }
        set { brushRadius = value; }
    }

    public float BrushStrength
    {
        get { return brushStrength; }
        set { brushStrength = value; }
    }
    #endregion

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        mesh = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    protected override void InitializeRenderTextures()
    {
        renderTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 32, RenderTextureFormat.RFloat);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        Graphics.Blit(originalTexture, renderTexture);
    }

    protected override void ComputeValues()
    {
        // TODO: Replace strings with constant strings.
        computeShader.SetInt("_TextureSize", renderTextureWidth);
        computeShader.SetVector("_UvHit", uvHit);
        computeShader.SetFloat("_Radius", brushRadius);
        computeShader.SetFloat("_BrushStrength", brushStrength);
        computeShader.SetFloat("_Falloff", brushFalloff);
        computeShader.SetTexture(kernelHandleNumber, "Result", renderTexture);

        computeShader.Dispatch(kernelHandleNumber, renderTexture.width / KERNEL_SIZE, renderTexture.height / KERNEL_SIZE, 1);

        objectMaterial.SetTexture("_Mask", renderTexture);
        //objectMaterial.SetTexture("_MainTex", renderTexture);
    }

    #region Mouse methods
    private void OnMouseDrag()
    {
        if (mouseInput)
        {
            Vector3 hit;
            RaycastHit rayHit;
            if (Camera.main.CollisionFor(Camera.main.ScreenPointToRayFor(Input.mousePosition), 8, out hit, out rayHit))
            {
                // This only works with a Mesh Collider!!!
                uvHit = rayHit.textureCoord;
            }
        }
    }

    private void OnMouseUp()
    {
        if(mouseInput)
            uvHit.Set(float.MaxValue, float.MaxValue, float.MaxValue);
    }

    public void SetUVHitPosition(Vector2 uvHit)
    {
        // Only if external input is activated.
        if (!mouseInput)
            this.uvHit.Set(uvHit.x, uvHit.y, 0);
    }
    #endregion
}