using System.Collections;
using UnityEngine;

public delegate void DustSuckerStatusHandler(bool status);

public class DustSuckerSwitch : MonoBehaviour, Toggle {

    #region variables
    public static event DustSuckerStatusHandler DustSuckerStatus;

    private bool suckerActive = false;
    #endregion

    #region methods
    public void ToggleSwitch()
    {
        OnDustSuckerStatusChanged();
    }

    private void OnDustSuckerStatusChanged()
    {
        suckerActive = !suckerActive;
        if (DustSuckerStatus != null)
        {
            Debug.Log("switched off");
            DustSuckerStatus(suckerActive);
        }
    }
    #endregion
}
