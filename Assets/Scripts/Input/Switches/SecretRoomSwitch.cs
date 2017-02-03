using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void SecretRoomOpenedHandler(bool openend);

public class SecretRoomSwitch : MonoBehaviour, Toggle {

    public static event SecretRoomOpenedHandler SecretRoomOpenend;

    private bool isOpen = false;

    public void ToggleSwitch()
    {
        FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance("event:/Switch");
        e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(e, gameObject.transform, GetComponent<Rigidbody>());
        e.start();
        e.release();
        OnSecretRoomOpenend();
    }

    private void OnSecretRoomOpenend()
    {
        isOpen = !isOpen;
        if (SecretRoomOpenend != null)
        {
            SecretRoomOpenend(isOpen);
        }
    }
}
