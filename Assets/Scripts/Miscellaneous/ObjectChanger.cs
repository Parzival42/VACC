using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ObjectChanger : MonoBehaviour {

    [SerializeField]
    private Material originalMaterial;

    [SerializeField]
    private Material powerOffMaterial;

    private Renderer meshRenderer;

	void Awake () {
        meshRenderer = GetComponent<Renderer>();
        MainKillSwitch.MainPowerChanged += SwitchObjects;
	}
	
    private void SwitchObjects(bool powerOn)
    {
        if (powerOn)
        {
            meshRenderer.material = originalMaterial;
        }
        else
        {
            meshRenderer.material = powerOffMaterial;
        }
    }
}
