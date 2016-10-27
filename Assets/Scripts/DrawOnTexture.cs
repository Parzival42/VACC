using UnityEngine;
using System.Collections;

public class DrawOnTexture : MonoBehaviour
{
    public Camera cam;
    public Texture2D brush;
    public int brushSize;

    private Texture2D tmpTexture;


    void Start()
    {
        Texture2D origTexture = GetComponent<Renderer>().material.mainTexture as Texture2D;
        tmpTexture = new Texture2D(origTexture.width, origTexture.height);

        for (int y = 0; y < tmpTexture.height; y++)
        {
            for (int x = 0; x < tmpTexture.width; x++)
            {
                tmpTexture.SetPixel(x, y, new Color(1,1,1,0));
            }
        }
        tmpTexture.Apply();

        GetComponent<Renderer>().material.SetTexture(Shader.PropertyToID("_Stencil"), tmpTexture);
    }
    void Update()
    {
        if (!Input.GetMouseButton(0))
            return;

        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            return;

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
            return;

        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tmpTexture.width;
        pixelUV.y *= tmpTexture.height;
        //AddToStencil(tmpTexture, brush, pixelUV, brushSize);
        tmpTexture.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.white);
        tmpTexture.Apply();
    }

    /*void AddToStencil(Texture2D stencil, Texture2D brush, Vector2 brushPosition, int brushSizePixels)
    {
        RenderTexture rt = RenderTexture.GetTemporary(stencil.width, stencil.height, 0, RenderTextureFormat.ARGB32);
        //Copy existing stencil to render texture (blit sets the active RenderTexture)
        Graphics.Blit(stencil, rt);

        //Apply brush
        RenderTexture.active = rt;
        float bs2 = brushSizePixels / 2f;
        Graphics.DrawTexture(new Rect(brushPosition.x - bs2, brushPosition.y - bs2, brushSizePixels, brushSizePixels), brush);
        //Read texture back to stencil
        stencil.ReadPixels(new Rect(0, 0, stencil.width, stencil.height), 0, 0, true);
        stencil.Apply();
        RenderTexture.active = null;
        rt.Release();
    }*/
}
