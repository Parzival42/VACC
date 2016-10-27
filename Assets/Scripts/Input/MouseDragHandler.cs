using System;
using UnityEngine;

/// <summary>
/// Drags the object with the mouse based on a spring component.
/// </summary>
public class MouseDragHandler : InputDragHandler
{
    private readonly DragScript dragObject;
    private readonly Collider dragCollider;
    private readonly Camera cam;
    private readonly SpringJoint spring;
    private readonly Rigidbody rigidbody;

    private Vector3 worldScreenPoint = Vector3.zero;
    private Vector3 offset = Vector3.zero;

    private Vector3 lastHit = Vector3.zero;

    private float originalSpringStrength;

    public MouseDragHandler(DragScript dragableObject, Collider triggerCollider, SpringJoint springJoint)
    {
        rigidbody = dragableObject.GetComponent<Rigidbody>();

        // Init spring
        spring = springJoint;
        InitializeSpring();

        // Drag object
        dragObject = dragableObject;
        dragCollider = triggerCollider;
        cam = Camera.main;
    }

    private void InitializeSpring()
    {
        spring.autoConfigureConnectedAnchor = false;
        originalSpringStrength = spring.spring;
        spring.spring = 0f;
    }

    public void OnDrag ()
    {
        spring.connectedAnchor = CalculateCollisionWorldPoint() + Vector3.up * dragObject.GroundOffset;

        if (rigidbody.IsSleeping())
            rigidbody.WakeUp();

        Debug.DrawLine(spring.connectedAnchor, new Vector3(spring.connectedAnchor.x, 0, spring.connectedAnchor.z), Color.red);
    }

    public void OnSelected ()
    {
        spring.anchor = dragObject.transform.InverseTransformPoint(CalculateCollisionForObject());
        spring.spring = originalSpringStrength;
    }

    private Vector3 CalculateMouseWorldPosition ()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private Ray CalculateCameraToObjectRay()
    {
        return cam.ScreenPointToRay(Input.mousePosition);
    }

    public void OnDeselected()
    {
        spring.spring = 0;
    }

    private Vector3 CalculateCollisionWorldPoint()
    {
        Vector3 hit;

        //if (CalculateCollisionFor(9, out hit))
        //{
        //    Vector3 groundPoint;
        //    CastRayToGround(hit, out groundPoint);
        //    return groundPoint;
        //}

        CalculateCollisionFor(8, out hit);
        return hit;
    }

    private Vector3 CalculateCollisionForObject()
    {
        Vector3 hit;
        CalculateCollisionFor(9, out hit);
        return hit;
    }

    private bool CastRayToGround(Vector3 origin, out Vector3 hitPoint)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 10f, 1 << 8))
        {
            hitPoint = hit.point;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }

    private bool CalculateCollisionFor(int layerMask, out Vector3 hitPoint)
    {
        Ray r = CalculateCameraToObjectRay();
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, cam.farClipPlane, 1 << layerMask))
        {
            lastHit = hit.point;
            hitPoint = hit.point;
            return true;
        }
        else
            hitPoint = lastHit;

        return false;
    }
}