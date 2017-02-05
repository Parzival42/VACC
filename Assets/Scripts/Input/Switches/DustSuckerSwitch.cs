using System;
using System.Collections;
using UnityEngine;

public delegate void DustSuckerStatusHandler(bool status);

public class DustSuckerSwitch : MonoBehaviour, Toggle {

    public Renderer changeEmissionRend;
    private Material changeEmissionMat;

    public Texture emissionOff;
    public Texture emissionStandby;
    public Texture emissionOn;

    #region variables
    public static event DustSuckerStatusHandler DustSuckerStatus;

    private bool hasPower = false;
    private bool suckerActive = false;
    private bool globalPower = false;
    #endregion

    #region methods
    void Start()
    {
        SocketMagnet.DustSuckerConnection += DustSuckerConnectionUpdate;
        ConnectorDragAndDrop.DustSuckerConnectionLost += DustSuckerConnectionUpdate;
        MainKillSwitch.MainPowerChanged += GlobalPowerChanged;

        changeEmissionMat = changeEmissionRend.materials[0];
    }

    private void GlobalPowerChanged(bool status)
    {
        globalPower = status;
        if (!globalPower && suckerActive)
        {
            ToggleSwitch();
        }

        if(!globalPower)
            changeEmissionMat.SetTexture("_EmissionMap", emissionOff);
        else
            changeEmissionMat.SetTexture("_EmissionMap", emissionStandby);
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

        if (suckerActive)
            changeEmissionMat.SetTexture("_EmissionMap", emissionOn);
        else
            changeEmissionMat.SetTexture("_EmissionMap", emissionStandby);
    }
    #endregion
}
