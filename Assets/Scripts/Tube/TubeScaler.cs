﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeformType
{
    Dust,
    Layer1,
    Layer2,
    Layer3
}



public class TubeScaler : MonoBehaviour {

    #region variables
    [SerializeField]
    LeanTweenType expandEaseType = LeanTweenType.easeOutQuad;

    [SerializeField]
    LeanTweenType shrinkEaseType = LeanTweenType.easeOutBounce;

    [SerializeField]
    Vector3 originalScale = new Vector3(1, 1, 1);

    [SerializeField]
    Vector3 expandedScale = new Vector3(1.7f, 1.7f, 1.7f);

    [SerializeField]
    Vector3 biggerExpandedScale = new Vector3(2.5f, 2.5f, 2.5f);

    [SerializeField]
    Vector3 biggestExpandedScale = new Vector3(3.2f, 3.2f, 3.2f);

    [SerializeField]
    bool doItNow = false;

    
    private bool deformAllowed = true;
    private Transform[] tubeSegments;
    private bool isInitialized;

    #endregion
    #region properties

    public Transform[] TubeSegments
    {
        set {
            tubeSegments = value;
            isInitialized = true;
        }
    }
    #endregion
    #region methods


    //test stuff only
    void Start()
    {
        SuckingScript.TubeDeform += DeformTube;
    }


	
	// Update is called once per frame
	void Update () {
        if (isInitialized)
        {
            if (doItNow)
            {
                doItNow = false;
                StartCoroutine(TweenScheduler());
            }
        }
	}

    private void InititializeTween(int index)
    {
        LeanTween.value(gameObject, originalScale, expandedScale, 0.15f).setOnUpdate((Vector3 currentScale) =>
        {
            tubeSegments[index].localScale = currentScale;
        }).setEase(expandEaseType).setOnComplete(() => {
            LeanTween.value(gameObject, expandedScale, originalScale, 0.3f).setOnUpdate((Vector3 currentScale) =>
            {
                tubeSegments[index].localScale = currentScale;
            }).setEase(shrinkEaseType);
        });
    }

    private void InititializeBiggerTween(int index)
    {
        LeanTween.value(gameObject, originalScale, biggerExpandedScale, 0.35f).setOnUpdate((Vector3 currentScale) =>
        {
            tubeSegments[index].localScale = currentScale;
        }).setEase(expandEaseType).setOnComplete(() => {
            LeanTween.value(gameObject, biggerExpandedScale, originalScale, 0.5f).setOnUpdate((Vector3 currentScale) =>
            {
                tubeSegments[index].localScale = currentScale;
            }).setEase(shrinkEaseType);
        });
    }


    private void InititializeBiggestTween(int index)
    {
        LeanTween.value(gameObject, originalScale, biggestExpandedScale, 0.35f).setOnUpdate((Vector3 currentScale) =>
        {
            tubeSegments[index].localScale = currentScale;
        }).setEase(expandEaseType).setOnComplete(() => {
            LeanTween.value(gameObject, biggestExpandedScale, originalScale, 0.5f).setOnUpdate((Vector3 currentScale) =>
            {
                tubeSegments[index].localScale = currentScale;
            }).setEase(shrinkEaseType);
        });
    }


    private void DeformTube(DeformType deformType)
    {
        if (deformAllowed)
        {
            deformAllowed = false;
            StartCoroutine(Cooldown(0.4f));
            if(DeformType.Dust == deformType)
            {
                StartCoroutine(DeformType.Dust.ToString(), 0.25f);
            }

            if(DeformType.Layer1 == deformType || DeformType.Layer2 == deformType)
            {
                Debug.Log("layer1");
                StartCoroutine(DeformType.Layer1.ToString(), 0.25f);
            }

            if (DeformType.Layer3 == deformType)
            {
                StartCoroutine(DeformType.Layer3.ToString(), 0.25f);
            }
        }
    }


    private IEnumerator TweenScheduler()
    {
        for(int i = 1; i < tubeSegments.Length-1; i++)
        {
            InititializeTween(i);
            yield return new WaitForSeconds(0.05f);
        }

    }


    private IEnumerator Cooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        deformAllowed = true;
    }



    #region deformation variants

    private IEnumerator Dust(float duration)
    {
        for (int i = 1; i < tubeSegments.Length - 1; i++)
        {
            InititializeTween(i);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator Layer1(float duration)
    {
        for (int i = 1; i < tubeSegments.Length - 1; i++)
        {
            InititializeBiggerTween(i);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator Layer3(float duration)
    {
        for (int i = 1; i < tubeSegments.Length - 1; i++)
        {
            InititializeBiggestTween(i);
            yield return new WaitForSeconds(0.05f);
        }
    }



    #endregion

    #endregion
}
