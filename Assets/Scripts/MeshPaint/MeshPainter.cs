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

    private Vector2 uvHit = Vector2.zero;
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

        Graphics.Blit(originalTexture, renderTexture);
        Graphics.Blit(originalTexture, copyRT);
    }
	
	private void Update ()
    {
        CurrentMaterial.SetVector("_uvHit", uvHit);

        Graphics.Blit(copyRT, renderTexture, CurrentMaterial);
        Graphics.Blit(renderTexture, copyRT);
    }

    private void OnMouseDrag()
    {
        Vector3 hit;
        RaycastHit rayHit;
        if (Camera.main.CollisionFor(Camera.main.ScreenPointToRayFor(Input.mousePosition), 8, out hit, out rayHit))
        {
            uvHit = rayHit.textureCoord;
            //Debug.Log(uvHit);
        }
    }

    private void OnDisable()
    {
        if (currentMaterial)
            DestroyImmediate(currentMaterial);
    }
}
