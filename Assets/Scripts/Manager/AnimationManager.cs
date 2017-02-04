using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private static AnimationManager animationManager = null;

    #region Inspector variables
    [SerializeField]
    [Tooltip("Should contain animation controller with corresponding animation.")]
    private GameObject carLightPrefab;
    #endregion

    #region Internal variables
    private Camera mainCamera;
    LTDescr lensAberrationTween;
    #endregion

    public static AnimationManager Instance
    {
        get
        {
            animationManager = FindObjectOfType<AnimationManager>();

            if (animationManager == null)
                animationManager = new GameObject("_AnimationManager").AddComponent<AnimationManager>();
            return animationManager;
        }
    }

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (mainCamera == null)
            Debug.LogError("No Main camera found!");
    }

    public void PlayCarLight()
    {
        GameObject carLight = Instantiate(carLightPrefab);
        Animator anim = GetAnimatorOf(carLight);
        anim.Play("huber");
    }

    public void LightChromaticDispersion(float animationTime)
    {
        UnityStandardAssets.CinematicEffects.LensAberrations lensAberration = mainCamera.GetComponent<UnityStandardAssets.CinematicEffects.LensAberrations>();
        if (lensAberration == null)
            Debug.LogError("No Lens Aberration effect found on camera");

        CancelTween(lensAberrationTween);

        lensAberration.chromaticAberration.enabled = true;
        lensAberrationTween = LeanTween.value(0f, 4.5f, animationTime * 0.5f).setEase(LeanTweenType.easeOutCirc)
            .setOnUpdate((float value) => {
                lensAberration.chromaticAberration.amount = value;
            }).setLoopPingPong(1)
            .setOnComplete(() => { lensAberration.chromaticAberration.enabled = false; });
    }

    private Animator GetAnimatorOf(GameObject g)
    {
        Animator anim = g.GetComponent<Animator>();
        if (anim == null)
            Debug.LogError("No animator in prefab <b>" + g.name + "</b>!");
        return anim;
    }

    private void CancelTween(LTDescr tween)
    {
        if (tween != null)
            LeanTween.cancel(tween.id);
    }
}