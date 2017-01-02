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
    private float tweenTime = 0.3f;

    [SerializeField]
    private Color offColor = Color.red;

    [SerializeField]
    private Color onColor = Color.green;

    private bool activationPossible = true;
    private WaitForSeconds coolDown;

    private Toggle toggle;
    private Camera currentCamera;
    private Collider currentCollider;
    private Material material;

    private bool isPulsing = false;
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
        currentCollider = GetComponent<Collider>();
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


    void OnMouseOver()
    {
        if (!isPulsing)
        {
            isPulsing = true;
            StartPulseTween();
        }
    }

    void OnMouseExit()
    {
        isPulsing = false;
    }

    private void StartPulseTween()
    {
        PulseIn();
    }


    private void PulseIn()
    {
        LeanTween.value(gameObject, 0.0f, 1.0f, tweenTime).setOnUpdate((float count) => {
             material.SetFloat("_EffectAmount", count);
        })
        .setEase(LeanTweenType.linear)
        .setOnComplete(() =>
        {
            PulseOut();
        });
    }

    private void PulseOut()
    {
        LeanTween.value(gameObject, 1.0f, 0.0f, tweenTime).setOnUpdate((float count) => {
            material.SetFloat("_EffectAmount", count);
        })
       .setEase(LeanTweenType.linear)
       .setOnComplete(() =>
       {
           if (isPulsing)
           {
               PulseIn();
           }
       });
    }


    private IEnumerator Cooldown()
    {
        activationPossible = false;
        yield return coolDown;
        activationPossible = true;
    }
    #endregion
}
