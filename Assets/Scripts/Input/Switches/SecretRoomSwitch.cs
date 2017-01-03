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
