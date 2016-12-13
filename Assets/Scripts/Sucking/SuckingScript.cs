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

    [SerializeField]
    protected float brushFalloff = 0.6f;

    [Header("Mesh Melting")]
    [SerializeField]
    protected string meltShaderID = "DustSucker/MeshMelt";
    private Shader meltShader;

    [Header("Physics")]
    [SerializeField]
    protected int collisionLayerTerrain = 8;

    [SerializeField]
    protected int collisionLayerMeshes = 11;

    [SerializeField]
    [Tooltip("The working distance of the dust sucking.")]
    protected float suckingDistanceTerrain = 0.5f;
    [SerializeField]
    protected float suckingDistanceMeshes = 0.5f;

    private DragScript drag;

	private void Start ()
    {
        CheckFields();
        drag = GetComponent<DragScript>();
        meltShader = Shader.Find(meltShaderID);
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
            CheckCollisionTerrain();
            CheckCollisionMeshes();
        }
	}

    protected void CheckCollisionTerrain()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(suckingPoint.position, -suckingPoint.up, out hitInfo, suckingDistanceTerrain, 1 << collisionLayerMeshes);

        #if UNITY_EDITOR
        Debug.DrawRay(suckingPoint.position, -suckingPoint.up * suckingDistanceTerrain, Color.green);
        #endif

        if (hit)
        {
            PaintDataReceiver paintReceiver = hitInfo.collider.transform.GetComponent<PaintDataReceiver>();
            paintReceiver.Radius = suckingRadius;
            paintReceiver.BrushStrength = brushStrength;
            paintReceiver.BrushFalloff = brushFalloff;
            paintReceiver.SetUVHitPosition(hitInfo.textureCoord);
        }
    }

    protected void CheckCollisionMeshes()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(suckingPoint.position, -suckingPoint.forward, out hitInfo, suckingDistanceMeshes, 1 << collisionLayerMeshes);

        #if UNITY_EDITOR
        Debug.DrawRay(suckingPoint.position, -suckingPoint.forward * suckingDistanceMeshes, Color.yellow);
#endif

        if (hit)
        {
            ChangeToMeltMaterial(hitInfo.transform.gameObject);
        }
    }

    private void ChangeToMeltMaterial(GameObject obj)
    {
        Material currMaterial = obj.GetComponent<Renderer>().material;
        Color color = currMaterial.GetColor("_Color");
        currMaterial.shader = meltShader;
        currMaterial.SetColor("_Color", color);
        currMaterial.SetFloat("_MeltY", suckingPoint.position.y);
        currMaterial.SetVector("_MeltPosition", suckingPoint.position);

        obj.GetComponent<Collider>().enabled = false;
        obj.GetComponent<Rigidbody>().useGravity = false;

        obj.AddComponent<Sucked>().finalPosition = obj.transform.position - obj.transform.up * 2.5f;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(suckingPoint.position, Vector3.one * 0.05f);
    }
    #endif
}