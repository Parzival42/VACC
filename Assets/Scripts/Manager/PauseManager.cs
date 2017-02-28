using UnityEngine;
using FMODUnity;
public class PauseManager : MonoBehaviour {

	#region variables
	[SerializeField]
	private float tweenTime = 1.0f;

	private bool gamePaused = false;

	private static PauseManager pauseManagerInstance = null;
	#endregion

	#region properties
	public static PauseManager Instance{
		get{
			pauseManagerInstance = FindObjectOfType<PauseManager>();
            if (pauseManagerInstance == null)
                pauseManagerInstance = new GameObject("_PauseManager").AddComponent<PauseManager>();
            return pauseManagerInstance;
		}
	}

	#endregion

	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Pause")){
			if(!gamePaused){
				//Time.timeScale = 0.0f;
				TimeTween(true);
			}else{
				//Time.timeScale = 1.0f;
				TimeTween(false);
			}
			gamePaused = !gamePaused;
		}
	}

 private void TimeTween(bool animateIn)
    {
        float first = 1.0f;
        float second = 0.0f;

        if (!animateIn)
        {
            first = 0.0f;
            second = 1.0f;
        }

        FMOD.Studio.Bus masterBus;
        masterBus = RuntimeManager.GetBus("Bus:/");

        LeanTween.value(gameObject, first, second, tweenTime).setOnUpdate(
            (float val) => { Time.timeScale = val; masterBus.setFaderLevel(val); }
        ).setEase(LeanTweenType.easeInOutCubic).setUseEstimatedTime(true).setOnComplete(()=> {
        });
    }
}