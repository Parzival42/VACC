using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MainPowerHandler(bool powerOn);

public class MainKillSwitch : MonoBehaviour {


    public static event MainPowerHandler MainPowerChanged;
    private bool powerOn = false;

	// Use this for initialization
	void Start () {
        //OnMainPowerChanged(false);
	}

    void OnMouseDown()
    {
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
