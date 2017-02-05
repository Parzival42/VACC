using System;
using System.Collections;
using UnityEngine;

public delegate void DustSuckerStatusHandler(bool status);

public class DustSuckerSwitch : MonoBehaviour, Toggle {

    #region variables
    public static event DustSuckerStatusHandler DustSuckerStatus;

    private bool hasPower = false;
    private bool suckerActive = false;
    #endregion

    #region methods
    void Start()
    {
        SocketMagnet.DustSuckerConnection += DustSuckerConnectionUpdate;
        ConnectorDragAndDrop.DustSuckerConnectionLost += DustSuckerConnectionUpdate;
    }

    private void DustSuckerConnectionUpdate()
    {
       suckerActive = false;
       DustSuckerConnectionUpdate(false);
    }

    private void DustSuckerConnectionUpdate(bool connected)
    {
        hasPower = connected;
    }

    public void ToggleSwitch()
    {
        FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance("event:/Switch");
        e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(e, gameObject.transform, GetComponent<Rigidbody>());
        e.start();
        e.release();

        if (hasPower)
        {
            OnDustSuckerStatusChanged();
        }
    }

    private void OnDustSuckerStatusChanged()
    {
        suckerActive = !suckerActive;
        if (DustSuckerStatus != null)
        {
            DustSuckerStatus(suckerActive);
        }
    }
    #endregion
}
