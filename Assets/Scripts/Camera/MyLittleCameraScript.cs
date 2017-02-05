using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is my script
public class MyLittleCameraScript : MonoBehaviour {

    public GameObject finalSound;
    public GameObject ambientSound;
    public VacuumSound vaccuumSound;

    public void FadeOut()
    {
        AnimationManager.Instance.FadeSceneToWhite();
        StartCoroutine(ReloadLevel());
    }

    public void StartSound()
    {
        finalSound.GetComponent<FMODUnity.StudioEventEmitter>().Play();
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
        Application.Quit();
    }
}
