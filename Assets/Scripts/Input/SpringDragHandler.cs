using System;
using UnityEngine;

public abstract class SpringDragHandler : InputDragHandler
{
    protected readonly DragScript dragObject;
    protected readonly Collider dragCollider;
    protected readonly Camera cam;
    protected readonly SpringJoint spring;
    protected readonly Rigidbody rigidbody;

    protected float originalSpringStrength;

    public SpringDragHandler(DragScript dragableObject, Collider triggerCollider, SpringJoint springJoint)
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

    public void ActivateSpring()
    {
        spring.spring = originalSpringStrength;
    }

    public void DeactivateSpring()
    {
        spring.spring = 0f;
    }

    #region Abstract methods
    public abstract void OnDeselected();
    public abstract void OnDrag();
    public abstract void OnSelected();
    #endregion

}
