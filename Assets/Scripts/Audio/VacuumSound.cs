using UnityEngine;
using System.Collections;



public class VacuumSound : MonoBehaviour
{
    private FMOD_StudioEventEmitter emitter;
    public bool isRunning = false;
    
    [FMODUnity.EventRef]
    public string startup = "event:/DysonConvolution";  
    FMOD.Studio.EventInstance startupEv;

    [Range(0.0f, 1.0f)]
    public float powerValue = 0f;
    [Range(0.0f, 1f)]
    public float dustOcclusionValue = 0f;
    
    FMOD.Studio.ParameterInstance power;
    FMOD.Studio.ParameterInstance dustOcclusion;
    [FMODUnity.EventRef]
    public string shutdown = "event:/DysonShutdown";       
    FMOD.Studio.EventInstance shutdownEv;

    void Start()
    {
        startupEv = FMODUnity.RuntimeManager.CreateInstance(startup);
        shutdownEv = FMODUnity.RuntimeManager.CreateInstance(shutdown);
        startupEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        shutdownEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(startupEv, gameObject.transform, GetComponent<Rigidbody>());
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(shutdownEv, gameObject.transform, GetComponent<Rigidbody>());
    }

    public void StartEngine()
    {
        if (!isRunning)
        {

            if (shutdownEv != null) shutdownEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            if (startupEv != null) startupEv.start();

            startupEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            shutdownEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(startupEv, gameObject.transform, GetComponent<Rigidbody>());
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(shutdownEv, gameObject.transform, GetComponent<Rigidbody>());
        }

        isRunning = true;
    }

    public void StopEngine()
    {
        if (isRunning)
        {
            if(startupEv != null) startupEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            if (startupEv != null) shutdownEv.start();

            Debug.Log("stop");
        }

        isRunning = false;
    }

    public void SetPower(float v)
    {
        startupEv.getParameter("Power", out power);
        power.setValue(v);
    }

    public void SetDustOcclusion(float v)
    {
        startupEv.getParameter("DustOcclusion", out dustOcclusion);
        dustOcclusion.setValue(v);
    }

    public void Update()
    {
        SetPower(powerValue);
        SetDustOcclusion(dustOcclusionValue);
    }
}