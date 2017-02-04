using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDoor : MonoBehaviour {

    #region variables
    [SerializeField]
    private Vector3 offsetVector;

    [SerializeField]
    private float tweenTime = 0.3f;

    private Vector3 originalPosition;

    private ThrowManager throwManager;
    #endregion

    #region methods

    // Use this for initialization
    void Start () {
        SecretRoomSwitch.SecretRoomOpenend += OnSecretDoorStatusChange;
        throwManager = GetComponent<ThrowManager>();
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
        throwManager.EjectBooks();
        LeanTween.value(gameObject, transform.position, transform.position + offsetVector, tweenTime).setOnUpdate((Vector3 position) => {
            transform.position = position;
        })
        .setEase(LeanTweenType.easeInOutSine).setOnComplete(()=> { throwManager.EjectNOS(); });
    }


    private void CloseDoor()
    {
        LeanTween.value(gameObject, transform.position, originalPosition, tweenTime).setOnUpdate((Vector3 position) => {
            transform.position = position;
        })
       .setEase(LeanTweenType.easeInOutSine);
    }
    #endregion
}
