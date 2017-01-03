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

    [Header("Offset Material")]
    [SerializeField]
    private Material offsetMaterial;

    #region Internal members
    private Vector2 maskOffset;
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

    public float BrushFalloff {
        get { return brushFalloff; }
        set { brushFalloff = value; }
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

        originalTexture.wrapMode = TextureWrapMode.Repeat;
        float randomOffset = Random.Range(-10f, 10f);
        offsetMaterial.SetVector(ShaderConstants.PARAM_MASK_OFFSET, new Vector2(randomOffset, randomOffset));
        Graphics.Blit(originalTexture, renderTexture, offsetMaterial);
    }

    protected override void ComputeValues()
    {
        computeShader.SetInt(ShaderConstants.PARAM_TEXTURE_SIZE, renderTextureWidth);
        computeShader.SetVector(ShaderConstants.PARAM_UV_HIT, uvHit);
        computeShader.SetFloat(ShaderConstants.PARAM_RADIUS, brushRadius);
        computeShader.SetFloat(ShaderConstants.PARAM_BRUSH_STRENGTH, brushStrength);
        computeShader.SetFloat(ShaderConstants.PARAM_BRUSH_FALLOFF, brushFalloff);
        computeShader.SetTexture(kernelHandleNumber, ShaderConstants.PARAM_RESULT, renderTexture);

        computeShader.Dispatch(kernelHandleNumber, renderTexture.width / KERNEL_SIZE, renderTexture.height / KERNEL_SIZE, 1);

        objectMaterial.SetTexture(ShaderConstants.PARAM_MASK, renderTexture);
        objectMaterial.SetFloat(ShaderConstants.PARAM_MASK_STRENGTH, 1f);
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

    public override void InvertMeshModification()
    {
        brushStrength *= -1f;
    }
    #endregion
}