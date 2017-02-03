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

    void OnCollisionEnter()
    {
        collEvent = FMODUnity.RuntimeManager.CreateInstance(coll);
        collEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        collEvent.getParameter(collisionSound, out collParam);
        collParam.setValue(1f);
        collEvent.start();
        collEvent.release();
    }

}
