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
}
