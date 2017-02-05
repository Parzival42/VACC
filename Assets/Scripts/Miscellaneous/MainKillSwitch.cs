using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MainPowerHandler(bool powerOn);

public class MainKillSwitch : MonoBehaviour {


    public static event MainPowerHandler MainPowerChanged;
    private bool powerOn = false;
    private FMODUnity.StudioEventEmitter sound;

	// Use this for initialization
	void Start () {
        OnMainPowerChanged(false);
        sound = GetComponent<FMODUnity.StudioEventEmitter>();
	}

    void OnMouseDown()
    {
        sound.Play();
        powerOn = !powerOn;
        OnMainPowerChanged(powerOn);
        Debug.Log("done");
    }

    private void OnMainPowerChanged(bool powerOn)
    {
        if(MainPowerChanged != null)
        {
            MainPowerChanged(powerOn);
        }
    }
}
