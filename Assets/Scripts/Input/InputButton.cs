using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Toggle), typeof(Collider), typeof(Renderer))]
public class InputButton : MonoBehaviour {

    #region variables
    [SerializeField]
    private bool useCooldown = false;

    [SerializeField]
    private float cooldownTime = 0.2f;

    [SerializeField]
    private Color offColor = Color.red;

    [SerializeField]
    private Color onColor = Color.red;

    private bool activationPossible = true;
    private WaitForSeconds coolDown;

    private Toggle toggle;
    private Camera currentCamera;
    private Material material;

    private bool shutOff = false;
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
        currentCamera = Camera.main;
        coolDown = new WaitForSeconds(cooldownTime);
        material = GetComponent<Renderer>().material;
        material.color = offColor;
    }

    void OnMouseDown()
    {
        if (activationPossible)
        {
            if (shutOff)
            {
                material.color = offColor;
            }
            else
            {
                material.color = onColor;
            }
            shutOff = !shutOff;
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
