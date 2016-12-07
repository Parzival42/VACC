using UnityEngine;
using System.Collections;



public class VacuumSound : MonoBehaviour
{
    private FMOD_StudioEventEmitter emitter;
    public bool isRunning = false;

    [Range(0.0f, 1.0f)]
    private float coverage;
    [FMODUnity.EventRef]
    public string startup = "event:/DysonStartup";  
    FMOD.Studio.EventInstance startupEv;
    [FMODUnity.EventRef]
    public string shutdown = "event:/DysonShutdown";       
    FMOD.Studio.EventInstance shutdownEv;
    [FMODUnity.EventRef]
    public string idle = "event:/DysonIdle";       
    FMOD.Studio.EventInstance idleEv;
    FMOD.Studio.ParameterInstance rollingSpeedParam;    //speed param object

    void Start()
    {
        // emitter = FindObjectOfType<FMOD_StudioEventEmitter>();
        startupEv = FMODUnity.RuntimeManager.CreateInstance(startup);
        shutdownEv = FMODUnity.RuntimeManager.CreateInstance(shutdown);
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(startupEv, gameObject.transform, GetComponent<Rigidbody>());
       // FMODUnity.RuntimeManager.AttachInstanceToGameObject(shutdownEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    public void Update()
    {
        startupEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        shutdownEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void StartEngine()
    {
        if (!isRunning)
        {

            if (shutdownEv != null) shutdownEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            if (startupEv != null) startupEv.start();

            Debug.Log("start");
        }

        isRunning = true;
    }

    public void StopEngine()
    {
        if (isRunning)
        {
            
           // shutdownEv.set3DAttributes
            if(startupEv != null) startupEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            if (startupEv != null) shutdownEv.start();

            Debug.Log("stop");
        }

        isRunning = false;
    }

    /// <summary>
    /// Set coverage of the nozzle
    /// </summary>
    /// <param name="coverage">Value between 0.0 and 1.0</param>
    public void SetCoverage(float coverage)
    {
        if(this.coverage != coverage)
        {
            this.coverage = coverage;
        }
    }
}