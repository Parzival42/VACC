using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FogCreator : MonoBehaviour
{
	protected MeshRenderer fogRenderer;

	public Material fogMaterial;
	public float scale = 1;

	void OnEnable ()
	{
		fogRenderer = gameObject.GetComponent<MeshRenderer>();		
		fogRenderer.material = fogMaterial;
	}

	void Update ()
	{
		float radius = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 6;
		Material mat = Application.isPlaying ? fogRenderer.material : fogRenderer.sharedMaterial;
		mat.SetVector ("FogParam", new Vector4(transform.position.x, transform.position.y, transform.position.z, radius * scale));
	}
}
