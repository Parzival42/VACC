using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
public class PauseManager : MonoBehaviour {

	#region variables
	[SerializeField]
	private float tweenTime = 1.0f;

	[SerializeField]
	private GameObject pauseText;

	[SerializeField]
	private GameObject infoText;

	private bool gamePaused = false;

	private bool isCooledDown = true;

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
		if(Input.GetButtonDown("Pause") && isCooledDown){
			isCooledDown = false;
			if(!gamePaused){
				TimeTween(true);
			}else{
				TimeTween(false);
			}
			gamePaused = !gamePaused;
		}

		if(Input.GetButtonDown("Restart")){
			Time.timeScale = 1.0f;
			SceneManager.LoadScene(0);
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

		ShowUI(animateIn);

        FMOD.Studio.Bus masterBus;
        masterBus = RuntimeManager.GetBus("Bus:/");

        LeanTween.value(gameObject, first, second, tweenTime).setOnUpdate(
            (float val) => { Time.timeScale = val; masterBus.setFaderLevel(val); }
        ).setEase(LeanTweenType.easeInOutCubic).setUseEstimatedTime(true).setOnComplete(()=> {
			isCooledDown = true;
        });
    }

	private void ShowUI(bool show){
		infoText.SetActive(show);
		pauseText.SetActive(show);
	}
}

