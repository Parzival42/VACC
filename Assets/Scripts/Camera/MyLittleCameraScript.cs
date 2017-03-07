using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// this is my script
public class MyLittleCameraScript : MonoBehaviour {

    public GameObject finalSound;
    public GameObject ambientSound;
    public VacuumSound vaccuumSound;

    void Awake(){
        UnMuteCommonSounds();
        Reset();
    }

    public void FadeOut()
    {
        AnimationManager.Instance.FadeSceneToWhite();
        StartCoroutine(ReloadLevel());
    }

    public void StartSound()
    {
        finalSound.GetComponent<FMODUnity.StudioEventEmitter>().Play();
    }

    private void Reset(){
        FMOD.Studio.Bus masterBus;
        masterBus = RuntimeManager.GetBus("Bus:/");
        masterBus.setFaderLevel(1.0f);
        Time.timeScale = 1.0f;
    }

    public void UnMuteCommonSounds(){
        string busString = "Bus:/UsualSounds";
        FMOD.Studio.Bus bus;
        bus = RuntimeManager.GetBus(busString);
        bus.setFaderLevel(1.0f);
    }

    public void MuteCommonSounds()
    {
        string busString = "Bus:/UsualSounds";
        FMOD.Studio.Bus bus;
        bus = RuntimeManager.GetBus(busString);
        bus.setFaderLevel(0f);
        ambientSound.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
        vaccuumSound.StopEngine();
        Debug.Log("MUTE");
    }

    IEnumerator ReloadLevel()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }
}
