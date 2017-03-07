using UnityEngine;
using System.Collections;
using FMODUnity;

public class LittleBuddhaScript : MonoBehaviour
{
    private GravityManager gravity;

    private void Start()
    {
        gravity = FindObjectOfType<GravityManager>();
        if (gravity == null)
            Debug.LogError("No Gravity Manager :(");
    }

    /// <summary>
    /// Do here all end stage things.
    /// This is called when the buddha starts to be sucked.
    /// </summary>
    public void BuddhaAction()
    {
        StartCoroutine(DoNiceBuddhaThings());
    }

    private IEnumerator DoNiceBuddhaThings()
    {
        yield return new WaitForSeconds(1.5f);
        gravity.StabGravityInTheBack(500000f);

      //  yield return new WaitForSeconds(2f);
        AnimationManager.Instance.DoEndstageCameraEffect();


        yield return new WaitForSeconds(4.5f);
          FMOD.Studio.Bus masterBus;
        masterBus = RuntimeManager.GetBus("Bus:/");

        LeanTween.value(gameObject, 1.0f, 0.5f, 1.0f).setOnUpdate(
            (float val) => {masterBus.setFaderLevel(val); }
        ).setEase(LeanTweenType.easeInOutCubic).setUseEstimatedTime(true);
    }
}