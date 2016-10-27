using UnityEngine;

/// <summary>
/// Drags the object with the mouse based on a spring component.
/// </summary>
public class MouseDragHandler : InputDragHandler
{
    #region Members
    private readonly DragScript dragObject;
    private readonly Collider dragCollider;
    private readonly Camera cam;
    private readonly SpringJoint spring;
    private readonly Rigidbody rigidbody;

    private Vector3 worldScreenPoint = Vector3.zero;
    private Vector3 offset = Vector3.zero;

    private Vector3 lastHit = Vector3.zero;

    private float originalSpringStrength;
    #endregion

    public bool IsSpringActive
    {
        get
        {
            if (spring.spring > 0f)
                return true;
            else
                return false;
        }
    }

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
        spring.connectedAnchor = CalculateCollisionWorldPoint();

        if (rigidbody.IsSleeping())
            rigidbody.WakeUp();

        Debug.DrawLine(spring.connectedAnchor, new Vector3(spring.connectedAnchor.x, 0, spring.connectedAnchor.z), Color.red);
    }

    public void OnSelected ()
    {
        spring.anchor = dragObject.transform.InverseTransformPoint(CalculateCollisionForObject());
        spring.spring = originalSpringStrength;
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

        // Case 1: Object was hit -> Don't add additional height to collision point.
        if (CalculateCollisionFor(9, out hit))
            return hit;

        // Case 2: Ground was hit -> Add additional height to contact point.
        CalculateCollisionFor(8, out hit);
        return hit + Vector3.up * dragObject.GroundOffset;
    }

    private Vector3 CalculateCollisionForObject()
    {
        Vector3 hit;
        CalculateCollisionFor(9, out hit);
        return hit;
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