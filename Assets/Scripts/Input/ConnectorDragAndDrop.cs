using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DustSuckerConnectionLostHandler();

[RequireComponent(typeof(Rigidbody))]
public class ConnectorDragAndDrop : MonoBehaviour {

    #region variables
    public static event DustSuckerConnectionLostHandler DustSuckerConnectionLost;

    private Rigidbody attachedRigidbody;

    private bool isDragable = true;
    private bool unplugged = true;

    [SerializeField]
    private float pickUpDistance = 3.0f;
    #endregion

    #region property

    public bool IsDragable
    {
        set {
            if(isDragable == false && value == true)
            {
                unplugged = false;
            }
            isDragable = value;
        }
    }

    #endregion

    #region methods
    void Start()
    {
        attachedRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        if (isDragable)
        {
            if (!unplugged)
            {
                unplugged = true;
                OnDustSuckerConnectionLost();
            }
            attachedRigidbody.isKinematic = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
    void OnMouseDrag()
    {
        if (isDragable)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(3.0f);
            transform.position = rayPoint;
        }
    }

    void OnMouseUp()
    {
        if (isDragable)
        {
            attachedRigidbody.isKinematic = false;
        }
    }

    void OnDustSuckerConnectionLost()
    {
        if (DustSuckerConnectionLost != null)
        {
            DustSuckerConnectionLost();
        }
    }

    #endregion
}
