﻿using UnityEngine;
using System.Collections;
using System;

public class VacuumSound : MonoBehaviour
{
    private FMOD_StudioEventEmitter emitter;
    public bool isRunning = false;
    
    [FMODUnity.EventRef]
    public string startup = "event:/DysonConvolution";  
    FMOD.Studio.EventInstance startupEv;

    [FMODUnity.EventRef]
    public string shutdown = "event:/DysonShutdown";
    FMOD.Studio.EventInstance shutdownEv;

    [FMODUnity.EventRef]
    public string energyBoost = "event:/Nitro";
    FMOD.Studio.EventInstance energyBoostEv;

    [FMODUnity.EventRef]
    public string gravity = "event:/Gravity";
    FMOD.Studio.EventInstance gravityEv;

    [Range(0.0f, 1.0f)]
    public float powerValue = 0f;
    private float powerValueDamped = 0f;
    [Range(0.0f, 1f)]
    public float dustOcclusionValue = 0f;

    FMOD.Studio.ParameterInstance power;
    FMOD.Studio.ParameterInstance dustOcclusion;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        startupEv = FMODUnity.RuntimeManager.CreateInstance(startup);
        shutdownEv = FMODUnity.RuntimeManager.CreateInstance(shutdown);
        energyBoostEv = FMODUnity.RuntimeManager.CreateInstance(energyBoost);
        gravityEv = FMODUnity.RuntimeManager.CreateInstance(gravity);

        startupEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        shutdownEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        energyBoostEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        gravityEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(startupEv, gameObject.transform, rigidbody);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(shutdownEv, gameObject.transform, rigidbody);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(energyBoostEv, gameObject.transform, rigidbody);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(gravityEv, gameObject.transform, rigidbody);

        startupEv.getParameter("Power", out power);
        startupEv.getParameter("DustOcclusion", out dustOcclusion);
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
        powerValue = v;
    }

    public void SetDustOcclusion(float v)
    {
        dustOcclusionValue = v;
        if (dustOcclusion != null)
        {
            dustOcclusion.setValue(v);
        }
    }

    public void Update()
    {
        if(powerValue != powerValueDamped)
        {
            powerValueDamped += ( powerValue - powerValueDamped ) * 0.1f; // damping

            if (power != null)
            {
                power.setValue(powerValueDamped);
            }
        }

        SetDustOcclusion(Mathf.Clamp(rigidbody.velocity.magnitude,0f,1f));
    }

    public void PlayEnergyBoost()
    {
        if (energyBoostEv != null) energyBoostEv.start();
        Debug.Log("boost");
    }

    public void PlayGravityLoss()
    {
        if (gravityEv != null) gravityEv.start();
    }

}