using UnityEngine;
using System.Collections;
using FMODUnity;

/// <summary>
/// Fades the global sound in.
/// </summary>
public class StartSoundLerp : MonoBehaviour
{
    [SerializeField]
    private float fadeInTime = 3f;

	private void Awake ()
    {
        string masterBusString = "Bus:/";
        FMOD.Studio.Bus masterBus;
        masterBus = RuntimeManager.GetBus(masterBusString);

        //masterBus.getFaderLevel(out float);
        LeanTween.value(0f, 1f, fadeInTime).setEase(LeanTweenType.easeInCubic).setOnUpdate(
            (float value) => {
                masterBus.setFaderLevel(value);
            }
        );
        StartCoroutine(FadeImageAfterAudio());
	}

    public IEnumerator FadeImageAfterAudio()
    {
        yield return new WaitForSeconds(5.7f);
        AnimationManager.Instance.FadeBlackToScene();
    }
}
