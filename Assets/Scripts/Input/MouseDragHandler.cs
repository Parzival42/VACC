using UnityEngine;

/// <summary>
/// Drags the object with the mouse based on a spring component.
/// </summary>
public class MouseDragHandler : SpringDragHandler
{
    #region Members
    private readonly static float ANCHOR_OFFSET = 0.2f;
    private Vector3 worldScreenPoint = Vector3.zero;
    private Vector3 offset = Vector3.zero;

    private Vector3 lastHit = Vector3.zero;
    private float originalSpringDamping;
    private bool isDragged = false;
    #endregion

    public MouseDragHandler(DragScript dragableObject, Collider triggerCollider, SpringJoint springJoint) 
        : base(dragableObject, triggerCollider, springJoint)
    {
        this.originalSpringDamping = spring.damper;
    }

    public override void OnDrag ()
    {
        isDragged = true;
        spring.connectedAnchor = CalculateCollisionWorldPoint();

        // Is the connected anchor left or right of the dragObj?
        Vector3 springDir = Vector3.Normalize(dragObject.transform.position - spring.connectedAnchor);
        springDir.y = 0;
        float direction = AngleDir(dragObject.transform.forward, springDir, dragObject.transform.up);

        RotateTowardsTarget();

        // Is the connected anchor in front of behind the dragObj?
        float springFwdDot = Vector3.Dot(dragObject.transform.forward, springDir);

        // Calculate Base Anchor position
        PlaceAnchor(direction, springFwdDot);

        Vector3 anchorWorldPosition = dragObject.transform.TransformPoint(spring.anchor);

        // For the connector anchor (mouse) use the y position of the base anchor to use the distance on the x/z plane
        CalculateAndSetSpringStrength(Vector3.Distance(
            new Vector3(spring.connectedAnchor.x, anchorWorldPosition.y, spring.connectedAnchor.z),
            anchorWorldPosition));

        if (rigidbody.IsSleeping())
            rigidbody.WakeUp();

#if UNITY_EDITOR
        // Direction
        Debug.DrawLine(dragObject.transform.position, dragObject.transform.position + (dragObject.transform.forward * springFwdDot).normalized * -0.5f, Color.yellow);
        // Mouse Position
        Debug.DrawLine(spring.connectedAnchor, new Vector3(spring.connectedAnchor.x, 0, spring.connectedAnchor.z), Color.red);
        // Connection between mouse and base anchor
        Debug.DrawLine(spring.connectedAnchor, anchorWorldPosition, Color.blue);
#endif
    }

    /// <summary>
    /// Calculates the base anchor of the spring based on the side of the mouse (relative
    /// to the transform position and if the mouse is in front or in the back of the transform.
    /// </summary>
    /// <param name="leftRightDirection">Specifies if the mouse is on the left or the right (relative to transform)</param>
    /// <param name="frontBackDirection">Specifies if the mouse is in the front or in the back of the transform</param>
    private void PlaceAnchor(float leftRightDirection, float frontBackDirection)
    {
        // Change anchor position based on angle and direction
        if (IsLeft(leftRightDirection) && IsFront(frontBackDirection) || IsLeft(leftRightDirection) && !IsFront(frontBackDirection))
        {
            // Anchor on the left side -> Mouse is on the right in the front OR
            // Anchor on the left side -> Mouse is on the left in the back
            spring.anchor = new Vector3(-ANCHOR_OFFSET, 0, 0);
            //RotateBasedOnMousePosition(!IsFront(frontBackDirection));
        }
        else if (!IsLeft(leftRightDirection) && IsFront(frontBackDirection) || !IsLeft(leftRightDirection) && !IsFront(frontBackDirection))
        {
            // Anchor on the right side -> Mouse is on the left in the front OR
            // Anchor on the left side -> Mouse is on the left in the back
            spring.anchor = new Vector3(ANCHOR_OFFSET, 0, 0);
            //RotateBasedOnMousePosition(IsFront(frontBackDirection));
        }
        else
            spring.anchor = Vector3.zero;
    }

    private void RotateBasedOnMousePosition(bool invert)
    {
        float inverter = invert ? -1f : 1f;
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(0, inverter * dragObject.rotationHelper * Time.deltaTime, 0));
    }

    private void RotateTowardsTarget()
    {
        Vector3 localTargetPosition = dragObject.transform.InverseTransformPoint(new Vector3(spring.connectedAnchor.x, 0f, spring.connectedAnchor.z));

        float angle = Mathf.Atan2(localTargetPosition.x, localTargetPosition.z) * Mathf.Rad2Deg;

        Vector3 eulerVelocity = new Vector3(0f, angle, 0f);
        Quaternion deltaRotation = Quaternion.Euler(eulerVelocity * Time.deltaTime * dragObject.rotationHelper);
        rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
    }

    public override void OnSelected ()
    {
        spring.anchor = CalculateCollisionForObject();
        ActivateSpring();
    }

    public override void OnDeselected()
    {
        isDragged = false;
        DeactivateSpring();
    }

    /// <summary>
    /// Direction == -1 -> Left
    /// Direction == 1 -> Right
    /// </summary>
    private bool IsLeft(float direction)
    {
        return direction <= 0;
    }

    /// <summary>
    /// Direction < 0 -> Front
    /// Directoin > 0 -> Back
    /// </summary>
    private bool IsFront(float direction)
    {
        return direction < 0;
    }

    /// <summary>
    /// Calculates and sets the damping value of the spring based on the distance between the 
    /// base anchor point and the other anchor point which is placed by the mouse.
    /// </summary>
    private void CalculateAndSetSpringStrength(float anchorToMouseDistance)
    {
        float normalizedDistance = Mathf.InverseLerp(0f, dragObject.distanceForMinDamping, anchorToMouseDistance);
        spring.damper = Mathf.Lerp(originalSpringDamping, dragObject.minDamping, normalizedDistance);
    }

    private Vector3 CalculateCollisionWorldPoint()
    {
        Vector3 hit;

        // Case 1: Object was hit -> Don't add additional height to collision point.
        // Skip this if object is currently dragged
        if (!isDragged && cam.CollisionFor(cam.ScreenPointToRayFor(Input.mousePosition), 9, out hit))
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
    
    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir).normalized;
        float dir = Vector3.Dot(perp, up);

        return dir;
    }
}