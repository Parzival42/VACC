using UnityEngine;

/// <summary>
/// Lerps the given camera from point A to B based on the position ranges of a 
/// given follow Transform.
/// </summary>
public class CameraLerper : MonoBehaviour
{
    [Header("Lerp reference Points")]
    [SerializeField]
    private Transform firstLerpPoint;
    [SerializeField]
    private Transform secondLerpPoint;

    [Header("Lerp params")]
    [SerializeField]
    private float tweenTime = 0.4f;

    [Header("External")]
    [SerializeField]
    private ColliderTrigger colliderTrigger;

    private void Start()
    {
        colliderTrigger.OnTriggerEntered += TweenToOtherState;
        colliderTrigger.OnTriggerExited += TweenToOriginalState;
    }

    private void TweenToOriginalState()
    {
        LeanTween.cancel(gameObject);
        LeanTween.move(gameObject, firstLerpPoint, tweenTime).setEase(LeanTweenType.easeInOutSine);
        LeanTween.rotate(gameObject, firstLerpPoint.rotation.eulerAngles, tweenTime).setEase(LeanTweenType.easeInOutSine);
    }

    private void TweenToOtherState()
    {
        LeanTween.cancel(gameObject);
        LeanTween.move(gameObject, secondLerpPoint, tweenTime).setEase(LeanTweenType.easeInOutSine);
        LeanTween.rotate(gameObject, secondLerpPoint.rotation.eulerAngles, tweenTime).setEase(LeanTweenType.easeInOutSine);
    }
}