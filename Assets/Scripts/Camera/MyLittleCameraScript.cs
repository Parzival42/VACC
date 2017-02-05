using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is my script
public class MyLittleCameraScript : MonoBehaviour {

    public GameObject finalSound;

	public void FadeOut()
    {
        AnimationManager.Instance.FadeSceneToWhite();
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
        Debug.Log("MUTE");
    }
}
