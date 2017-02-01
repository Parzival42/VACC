using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeScaler : MonoBehaviour {

    #region variables
    [SerializeField]
    LeanTweenType expandEaseType = LeanTweenType.easeOutQuad;

    [SerializeField]
    LeanTweenType shrinkEaseType = LeanTweenType.easeOutBounce;

    [SerializeField]
    Vector3 originalScale = new Vector3(1, 1, 1);

    [SerializeField]
    Vector3 expandedScale = new Vector3(2, 2, 1);

    [SerializeField]
    bool doItNow = false;

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
        //tubeSegments = new Transform[10];
        //for(int i = 0; i < 10; i++)
        //{
        //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    go.transform.position = new Vector3(0,0,i*1);
        //    tubeSegments[i] = go.transform;
        //}
        //isInitialized = true;
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
        LeanTween.value(gameObject, originalScale, expandedScale, 0.2f).setOnUpdate((Vector3 currentScale) =>
        {
            tubeSegments[index].localScale = currentScale;
        }).setEase(expandEaseType).setOnComplete(() => {
            LeanTween.value(gameObject, expandedScale, originalScale, 0.4f).setOnUpdate((Vector3 currentScale) =>
            {
                tubeSegments[index].localScale = currentScale;
            }).setEase(shrinkEaseType);
        });
    }



    private IEnumerator TweenScheduler()
    {
        for(int i = 0; i < tubeSegments.Length; i++)
        {
            InititializeTween(i);
            yield return new WaitForSeconds(0.05f);
        }

    }


    #endregion
}
