using UnityEngine;

public class MeshPainter : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField]
    private RenderTexture renderTexture;

    [SerializeField]
    private Texture originalTexture;

    [SerializeField]
    protected Shader currentShader;

    [SerializeField]
    private RenderTexture copyRT;

    [Header("Shader parameters")]
    //[SerializeField]
    //private float radius = 0.03f;

    [SerializeField]
    [Range(-1f, 1f)]
    private float paintStrength = 1f;

    [SerializeField]
    private float falloff = 100f;

    private Vector3 uvHit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    private Material objectMaterial;

    private bool swap = false;

    private Material currentMaterial;
    public Material CurrentMaterial
    {
        get
        {
            if (currentMaterial == null)
            {
                currentMaterial = new Material(currentShader);
                currentMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return currentMaterial;
        }
    }

    private void Start ()
    {
        objectMaterial = GetComponent<Renderer>().material;

        InitializeRenderTextures();
    }
	
	private void Update ()
    {
        CurrentMaterial.SetVector("_uvHit", uvHit);
        //CurrentMaterial.SetFloat("_Radius", radius);
        CurrentMaterial.SetFloat("_PaintStrength", paintStrength);
        CurrentMaterial.SetFloat("_Falloff", falloff);

        Graphics.Blit(copyRT, renderTexture, CurrentMaterial);
        Graphics.Blit(renderTexture, copyRT);
    }

    private void OnMouseDrag()
    {
        Vector3 hit;
        RaycastHit rayHit;
        if (Camera.main.CollisionFor(Camera.main.ScreenPointToRayFor(Input.mousePosition), 8, out hit, out rayHit))
        {
            // This only works with a Mesh Collider!!!
            uvHit = rayHit.point;
        }
    }

    private void InitializeRenderTextures()
    {
        Graphics.Blit(originalTexture, renderTexture);
        Graphics.Blit(originalTexture, copyRT);
    }

    private void OnMouseUp()
    {
        uvHit.Set(float.MaxValue, float.MaxValue, float.MaxValue);
    }

    private void OnDisable()
    {
        InitializeRenderTextures();

        if (currentMaterial)
            DestroyImmediate(currentMaterial);
    }
}
