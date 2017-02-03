using UnityEngine;
using FMODUnity;

/// <summary>
/// Fades the global sound in.
/// </summary>
public class StartSoundLerp : MonoBehaviour
{
    [SerializeField]
    private float fadeInTime = 3f;

	private void Start ()
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
	}
}
