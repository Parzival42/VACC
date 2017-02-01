using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Requires that the used Materials is 'LineUI'.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MouseHoverMaterialChanger : MonoBehaviour
{
    private static float RENDER_LINE_ON = 1f;
    private static float RENDER_LINE_OFF = 0f;

    private Material[] materials;

    private bool isMouseDown = false;

	private void Start ()
    {
        materials = GetComponent<Renderer>().materials;
	}

    private void OnMouseOver()
    {
        if (!isMouseDown)
        {
            SetRenderLineWidth(RENDER_LINE_ON);
            SetRenderLineForChildren(RENDER_LINE_ON);
        }
    }

    private void OnMouseExit()
    {
        SetRenderLineWidth(RENDER_LINE_OFF);
        SetRenderLineForChildren(RENDER_LINE_OFF);
    }

    private void OnMouseDown()
    {
        isMouseDown = true;
        SetRenderLineWidth(RENDER_LINE_OFF);
        SetRenderLineForChildren(RENDER_LINE_OFF);
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
    }

    private void SetRenderLineForChildren(float toggleValue)
    {
        foreach (Transform t in transform)
            SetRenderLineWidthFor(t, toggleValue);
    }

    private void SetRenderLineWidthFor(Transform target, float toggleValue)
    {
        Renderer r = target.GetComponent<Renderer>();
        if (r != null)
        {
            Material[] transformMaterials = r.materials;

            foreach (Material m in transformMaterials)
                m.SetFloat("_IsHighlighted", toggleValue);
        }
    }

    private void SetRenderLineWidth(float toggleValue)
    {
        foreach (Material m in materials)
            m.SetFloat("_IsHighlighted", toggleValue);
    }
}