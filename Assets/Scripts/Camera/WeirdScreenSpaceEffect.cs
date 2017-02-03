using UnityEngine;

[ExecuteInEditMode]
public class WeirdScreenSpaceEffect : MonoBehaviour
{
    [SerializeField]
    protected Shader currentShader;

    [Range(0.0f, 1.0f)]
    public float intensity = 0f;

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

    void Start()
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
            material.SetFloat("_Intensity", intensity);
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