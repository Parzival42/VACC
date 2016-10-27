using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class DragScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float groundOffset = 1f;

    private InputDragHandler inputHandler;

    public float GroundOffset { get { return groundOffset; } }

	private void Start ()
    {
        inputHandler = new MouseDragHandler(this, GetTriggerCollider(), GetComponent<SpringJoint>());
	}

    private void OnMouseDrag()
    {
        inputHandler.OnDrag();
    }

    private void OnMouseDown()
    {
        inputHandler.OnSelected();
    }

    private void OnMouseUp()
    {
        inputHandler.OnDeselected();
    }

    private Collider GetTriggerCollider()
    {
        foreach (Collider c in GetComponents<Collider>())
        {
            if (c.isTrigger)
                return c;
        }
        return null;
    }
}