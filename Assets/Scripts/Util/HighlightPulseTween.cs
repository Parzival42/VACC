using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HighlightPulseTween : MonoBehaviour {
    #region variables
    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.linear;

    [SerializeField]
    private float pulseTweenTime = 0.3f;

    private Material material;
    private bool isPulsing = false;
    #endregion

    #region methods
    void Start () {
        material = GetComponent<Renderer>().material;
    }
	
    void OnMouseOver()
    {
        if (!isPulsing)
        {
            isPulsing = true;
            PulseIn();
        }
    }

    void OnMouseExit()
    {
        isPulsing = false;
    }

    private void PulseIn()
    {
        LeanTween.value(gameObject, 0.0f, 1.0f, pulseTweenTime).setOnUpdate((float count) => {
            material.SetFloat("_EffectAmount", count);
        })
        .setEase(easeType)
        .setOnComplete(() =>
        {
            PulseOut();
        });
    }

    private void PulseOut()
    {
        LeanTween.value(gameObject, 1.0f, 0.0f, pulseTweenTime).setOnUpdate((float count) => {
            material.SetFloat("_EffectAmount", count);
        })
       .setEase(easeType)
       .setOnComplete(() =>
       {
           if (isPulsing)
           {
               PulseIn();
           }
       });
    }
    #endregion
}
