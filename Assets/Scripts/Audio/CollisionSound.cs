using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string coll = "event:/Collision";
    FMOD.Studio.EventInstance collEvent;

    public string collisionSound = "fjuture";
    FMOD.Studio.ParameterInstance collParam;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Silent"))
        {
            if (collEvent != null)
            {
                FMOD.Studio.PLAYBACK_STATE state;
                collEvent.getPlaybackState(out state);

                if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                {
                    PlayOnce();
                }
            }
            else
            {
                PlayOnce();
            }
        }
    }

    void PlayOnce()
    {
        collEvent = FMODUnity.RuntimeManager.CreateInstance(coll);
        collEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(collEvent, gameObject.transform, GetComponent<Rigidbody>());
        collEvent.getParameter(collisionSound, out collParam);
        collParam.setValue(1f);
        collEvent.start();
        collEvent.release();
    }

}
