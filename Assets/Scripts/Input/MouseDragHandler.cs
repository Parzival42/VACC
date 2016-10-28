using UnityEngine;

/// <summary>
/// Drags the object with the mouse based on a spring component.
/// </summary>
public class MouseDragHandler : SpringDragHandler
{
    #region Members
    private Vector3 worldScreenPoint = Vector3.zero;
    private Vector3 offset = Vector3.zero;

    private Vector3 lastHit = Vector3.zero;

    private float originalSpringStrength;
    #endregion

    public MouseDragHandler(DragScript dragableObject, Collider triggerCollider, SpringJoint springJoint) 
        : base(dragableObject, triggerCollider, springJoint)
    { }

    public override void OnDrag ()
    {
        spring.connectedAnchor = CalculateCollisionWorldPoint();

        if (rigidbody.IsSleeping())
            rigidbody.WakeUp();

        Debug.DrawLine(spring.connectedAnchor, new Vector3(spring.connectedAnchor.x, 0, spring.connectedAnchor.z), Color.red);
    }

    public override void OnSelected ()
    {
        spring.anchor = dragObject.transform.InverseTransformPoint(CalculateCollisionForObject());
        ActivateSpring();
    }

    public override void OnDeselected()
    {
        DeactivateSpring();
    }

    private Vector3 CalculateCollisionWorldPoint()
    {
        Vector3 hit;

        // Case 1: Object was hit -> Don't add additional height to collision point.
        if (cam.CollisionFor(cam.ScreenPointToRayFor(Input.mousePosition), 9, out hit))
            return hit;

        // Case 2: Ground was hit -> Add additional height to contact point.
        if (cam.CollisionFor(cam.ScreenPointToRayFor(Input.mousePosition), 8, out hit))
            lastHit.Set(hit.x, hit.y, hit.z);
        else
            hit = new Vector3(lastHit.x, lastHit.y, lastHit.z);

        return hit + Vector3.up * dragObject.GroundOffset;
    }

    private Vector3 CalculateCollisionForObject()
    {
        Vector3 hit;
        cam.CollisionFor(cam.ScreenPointToRayFor(Input.mousePosition), 9, out hit);
        return hit;
    }
}