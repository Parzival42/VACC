using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void DustSuckerConnectionHandler(bool connected);

public class SocketMagnet : MonoBehaviour {

    #region variables
    public static event DustSuckerConnectionHandler DustSuckerConnection;

    [Header("Rotation properties")]
    [SerializeField]
    private float rotationTime = 0.2f;

    [SerializeField]
    private LeanTweenType rotationEaseType = LeanTweenType.linear;

    [Header("Position properties")]
    [SerializeField]
    private float positionTime = 0.3f;

    [SerializeField]
    private LeanTweenType positionEaseType = LeanTweenType.linear;

    private WaitForSeconds unplugCooldownTime;

    private bool isConnected = false;
    private bool isReady = true;
    #endregion

    void Start()
    {
        ConnectorDragAndDrop.DustSuckerConnectionLost += ConnectionLost;
        unplugCooldownTime = new WaitForSeconds(1.4f);
    }

    private void ConnectionLost()
    {
        PlaySound();
        isConnected = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isReady)
        {
            isReady = false;
            ConnectorDragAndDrop connector = other.gameObject.GetComponent<ConnectorDragAndDrop>();
            if (connector != null)
            {
                InitConnectionSequence(connector);
            }
        }
    }

    private void InitConnectionSequence(ConnectorDragAndDrop connector)
    {
        connector.IsDragable = false;
        ConnectConnector(connector);
    }


    private void ConnectConnector(ConnectorDragAndDrop connector)
    {
        PlaySound();
        Transform connectorTransform = connector.transform;
        Vector3 stuff = transform.eulerAngles - connectorTransform.eulerAngles;
        Vector3 rot = connectorTransform.transform.eulerAngles;
        Vector3 tmp = new Vector3();
        LeanTween.value(gameObject, Vector3.zero, stuff, rotationTime).setOnUpdate((Vector3 angleOffset) =>
        {
            tmp.Set(rot.x + angleOffset.x, rot.y + angleOffset.y, rot.z + angleOffset.z);
            connectorTransform.eulerAngles = tmp;
        })
        .setEase(rotationEaseType);

        Vector3 pos = transform.position - connectorTransform.position - transform.forward * 0.15f;
        Vector3 oldPos = connectorTransform.position;

        LeanTween.value(gameObject, Vector3.zero, pos, positionTime).setOnUpdate((Vector3 newPos) =>
        {
            connectorTransform.position = oldPos + newPos;
        })
        .setEase(positionEaseType)
        .setOnComplete(()=> { OnDustSuckerConnectionChanged(connector); });
    }

     private void OnDustSuckerConnectionChanged(ConnectorDragAndDrop connector)
    {
        isConnected = true;
        StartCoroutine(UnplugCooldown(connector));
        if (DustSuckerConnection != null)
        {
            DustSuckerConnection(isConnected);
        }
    }

    private IEnumerator UnplugCooldown(ConnectorDragAndDrop connector)
    {
        yield return unplugCooldownTime;
        connector.IsDragable = true;
        isReady = true;
    }


    void PlaySound()
    {
        FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance("event:/Jack");
        e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(e, gameObject.transform, GetComponent<Rigidbody>());
        e.start();
        e.release();
    }

}
