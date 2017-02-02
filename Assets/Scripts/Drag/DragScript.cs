using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class DragScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float groundOffset = 1f;

    [SerializeField]
    [Tooltip("Minimum damping value which is used when the mouse position is far away from the nozzle. "
        + "The max damping value is taken from the Spring Joint component.")]
    public float minDamping = 5f;

    [SerializeField]
    [Tooltip("If the distance between mouse and nozzle is higher than this value, the min damping value is taken.")]
    public float distanceForMinDamping = 2f;

    [SerializeField]
    [Tooltip("This value will artificially rotate the object additionally to the mouse movement.")]
    public float rotationHelper = 80f;

    public bool IsDragged
    {
        get { return isDragged; }
    }
    private bool isDragged = false;

    private Material mat;
    private SpringJoint spring;
    private InputDragHandler inputHandler;
    private VacuumSound sound;
    private bool dustSuckerActive = false;

    public float GroundOffset { get { return groundOffset; } }

	private void Start ()
    {
        spring = GetComponent<SpringJoint>();
        inputHandler = new MouseDragHandler(this, GetTriggerCollider(), spring);
        mat = GetComponent<Renderer>().material;
        sound = GetComponent<VacuumSound>();
        DustSuckerSwitch.DustSuckerStatus += UpdateDustSuckerStatus;
        ConnectorDragAndDrop.DustSuckerConnectionLost += UpdateDustSuckerStatus;
    }

    private void UpdateDustSuckerStatus()
    {
        UpdateDustSuckerStatus(false);
    }

    private void UpdateDustSuckerStatus(bool status)
    {
        dustSuckerActive = status;
        if (dustSuckerActive)
        {
            sound.StartEngine();
        }else
        {
            sound.StopEngine();
        }
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
        isDragged = true;
    }

    private void OnMouseUp()
    {
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