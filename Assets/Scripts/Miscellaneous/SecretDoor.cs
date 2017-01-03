using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDoor : MonoBehaviour {

    [SerializeField]
    private Vector3 offsetVector;

    [SerializeField]
    private float tweenTime = 0.3f;

    private Vector3 originalPosition;

	// Use this for initialization
	void Start () {
        SecretRoomSwitch.SecretRoomOpenend += OnSecretDoorStatusChange;

        originalPosition = transform.position;
	}
	
	private void OnSecretDoorStatusChange(bool opened)
    {
        if (opened)
        {
            OpenDoor();    
        }else
        {
            CloseDoor();
        }
    }


    private void OpenDoor()
    {
        LeanTween.value(gameObject, transform.position, transform.position + offsetVector, tweenTime).setOnUpdate((Vector3 position) => {
            transform.position = position;
        })
        .setEase(LeanTweenType.easeInOutSine);
    }


    private void CloseDoor()
    {
        LeanTween.value(gameObject, transform.position, originalPosition, tweenTime).setOnUpdate((Vector3 position) => {
            transform.position = position;
        })
       .setEase(LeanTweenType.easeInOutSine);
    }
}
