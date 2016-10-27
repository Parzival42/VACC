using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class DragScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float groundOffset = 1f;


    private Material mat;
    private SpringJoint spring;
    private InputDragHandler inputHandler;

    public float GroundOffset { get { return groundOffset; } }

	private void Start ()
    {
        spring = GetComponent<SpringJoint>();
        inputHandler = new MouseDragHandler(this, GetTriggerCollider(), spring);
        mat = GetComponent<Renderer>().material;
	}

    private void Update()
    {
        mat.SetVector("_MousePosition", transform.TransformPoint(spring.anchor));
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