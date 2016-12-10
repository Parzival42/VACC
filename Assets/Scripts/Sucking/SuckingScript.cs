using UnityEngine;

public class SuckingScript : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("At this point the dust sucking happens")]
    [SerializeField]
    protected Transform suckingPoint;

    [SerializeField]
    protected float suckingRadius = 0.05f;

    [SerializeField]
    protected float brushStrength = -0.02f;

    [Header("Physics")]
    [SerializeField]
    protected int collisionLayer = 8;

    [SerializeField]
    [Tooltip("The working distance of the dust sucking.")]
    protected float suckingDistance = 0.5f;

    private DragScript drag;

	private void Start ()
    {
        CheckFields();
        drag = GetComponent<DragScript>();
    }

    private void CheckFields()
    {
        if (suckingPoint == null)
            Debug.LogError("No sucking point specified, you failed in every aspect!");
    }
	
	private void Update ()
    {
        if (drag.IsDragged)
        {
            CheckCollision();
        }
	}

    protected void CheckCollision()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(suckingPoint.position, -suckingPoint.up, out hitInfo, suckingDistance, 1 << collisionLayer);

        #if UNITY_EDITOR
        Debug.DrawRay(suckingPoint.position, -suckingPoint.up * suckingDistance, Color.green);
        #endif

        if (hit)
        {
            PaintDataReceiver paintReceiver = hitInfo.collider.transform.GetComponent<PaintDataReceiver>();
            paintReceiver.Radius = suckingRadius;
            paintReceiver.BrushStrength = brushStrength;
            paintReceiver.SetUVHitPosition(hitInfo.textureCoord);
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(suckingPoint.position, Vector3.one * 0.05f);
    }
    #endif
}