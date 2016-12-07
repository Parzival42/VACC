using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class DragScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float groundOffset = 1f;

    public bool IsDragged
    {
        get { return isDragged; }
    }
    private bool isDragged = false;

    private Material mat;
    private SpringJoint spring;
    private InputDragHandler inputHandler;
    private VacuumSound sound;

    public float GroundOffset { get { return groundOffset; } }

	private void Start ()
    {
        spring = GetComponent<SpringJoint>();
        inputHandler = new MouseDragHandler(this, GetTriggerCollider(), spring);
        mat = GetComponent<Renderer>().material;
        sound = GetComponent<VacuumSound>();
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
        sound.StartEngine();
        inputHandler.OnSelected();
        isDragged = true;
    }

    private void OnMouseUp()
    {
        sound.StopEngine();
        inputHandler.OnDeselected();
        isDragged = false;
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