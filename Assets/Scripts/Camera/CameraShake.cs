using UnityEngine;

[ExecuteInEditMode]
public class CameraShake : MonoBehaviour
{
    [SerializeField]
    protected Shader currentShader;

    [Header("Shader Parameters")]
    [SerializeField]
    public float offsetX;

    [SerializeField]
    public float offsetY;

    [SerializeField]
    public float zoom;

    private Material currentMaterial;
    public Material material
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

    private void Start()
    {
        if (!SystemInfo.supportsImageEffects)
            enabled = false;
        else if (!currentShader && !currentShader.isSupported)
            enabled = false;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (currentShader != null)
        {
            material.SetFloat("_OffsetX", offsetX);
            material.SetFloat("_OffsetY", offsetY);
            material.SetFloat("_Zoom", zoom);
            Graphics.Blit(source, destination, material);
        }
        else
            Graphics.Blit(source, destination);
    }

    private void OnDisable()
    {
        if (currentMaterial)
            DestroyImmediate(currentMaterial);
    }
}