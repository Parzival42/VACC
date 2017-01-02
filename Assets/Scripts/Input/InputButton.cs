using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Toggle), typeof(Collider))]
public class InputButton : MonoBehaviour {

    #region variables
    [SerializeField]
    private bool useCooldown = false;

    [SerializeField]
    private float cooldownTime = 0.2f;

    private bool activationPossible = true;
    private WaitForSeconds coolDown;

    private Toggle toggle;
    private Camera currentCamera;
    private Collider currentCollider;
    #endregion

    #region property
    public Camera CurrentCamera
    {
        set { currentCamera = value; }
    }
    #endregion

    #region methods
    void Start () {
        toggle = GetComponent<Toggle>();
        currentCollider = GetComponent<Collider>();
        currentCamera = Camera.main;
        coolDown = new WaitForSeconds(cooldownTime);
    }

    void OnMouseDown()
    {
        Debug.Log("done");
        if (activationPossible)
        {
            toggle.ToggleSwitch();
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        activationPossible = false;
        yield return coolDown;
        activationPossible = true;
    }
    #endregion
}
