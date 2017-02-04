using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour {

    [Range(1, 600)]
    public float speed = 220f;

    private float currentSpeed = 220.0f;
    

    void Awake()
    {
        MainKillSwitch.MainPowerChanged += SwitchStatus;
    }

	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime * currentSpeed);
    }

    private void SwitchStatus(bool powerOn)
    {
        if (powerOn)
        {
            StartRotation();
        }else
        {
            EndRotation();
        }
    }


    private void StartRotation()
    {
        LeanTween.value(gameObject, 0.0f, speed, 1.5f).setOnUpdate((float newSpeed) => {
            currentSpeed = newSpeed;
        })
        .setEase(LeanTweenType.easeOutSine);
    }


    private void EndRotation()
    {
        LeanTween.value(gameObject, speed, 0.0f, 1.5f).setOnUpdate((float newSpeed) => {
            currentSpeed = newSpeed;
        })
        .setEase(LeanTweenType.easeInSine);
    }
}
