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
    private IntroBlackFade screenFader;
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
        screenFader = FindObjectOfType<IntroBlackFade>();

        if (mainCamera == null)
            Debug.LogError("No Main camera found!");

        if (screenFader == null)
            Debug.LogError("No IntroBlackFade script found.");
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
        DoChromaticShiftEffect(lensAberration, animationTime, 4.5f, LeanTweenType.easeOutCirc);
    }

    public void FancyChromaticDispersion(float animationTime)
    {
        UnityStandardAssets.CinematicEffects.LensAberrations lensAberration = mainCamera.GetComponent<UnityStandardAssets.CinematicEffects.LensAberrations>();
        if (lensAberration == null)
            Debug.LogError("No Lens Aberration effect found on camera");

        CancelTween(lensAberrationTween);
        DoChromaticShiftEffect(lensAberration, animationTime, 20.5f, LeanTweenType.easeInOutBounce);
    }

    public void CameraShake(float time, float strength)
    {
        CameraShake shake = mainCamera.GetComponent<CameraShake>();
        if (shake == null)
            Debug.LogError("No Camera Shake script found.");

        LeanTween.value(0, strength, time).setEase(LeanTweenType.easeShake)
            .setOnUpdate((float value) => {
                shake.offsetX = value;
            })
            .setOnComplete(() => { shake.offsetX = 0f; });

        LeanTween.value(0, strength * 0.9f, time).setEase(LeanTweenType.easeShake)
            .setOnUpdate((float value) => {
                shake.offsetY = value;
            })
            .setOnComplete(() => { shake.offsetY = 0f; });
    }

    public void FadeBlackToScene()
    {
        screenFader.FadeIn();
    }

    public void FadeSceneToWhite()
    {
        screenFader.FadeOut();
    }

    public void DoEndstageCameraEffect()
    {
        Animator camAnim = mainCamera.GetComponent<Animator>();
        camAnim.Play("FinalCameraAnimation");
    }

    private void DoChromaticShiftEffect(UnityStandardAssets.CinematicEffects.LensAberrations lensAberration, float animationTime, float shift, LeanTweenType easeType)
    {
        lensAberration.chromaticAberration.enabled = true;
        lensAberrationTween = LeanTween.value(0f, shift, animationTime * 0.5f).setEase(easeType)
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