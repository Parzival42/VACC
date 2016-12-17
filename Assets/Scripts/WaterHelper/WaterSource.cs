using UnityEngine;

public class WaterSource : MonoBehaviour, Toggle
{
    #region Inspector variables
    [SerializeField]
    private string collisionLayerName;

    [SerializeField]
    private float collisionDistance = 1f;

    [SerializeField]
    private float waterBrushStrength = 0.3f;

    [SerializeField]
    private float waterBrushRadius = 0.2f;
    #endregion

    #region Internal members
    private ComputeMeshPaintWaterPipe waterPipeSimulation;
    private Vector2 uvHit = new Vector2(float.MaxValue, float.MaxValue);
    private int collisionLayer;
    private bool waterSourceEnabled = true;

    // Holds the information about the water source values:
    // X: uvHit.x, Y: uvHit.y, Z: brushStrength, W: brushRadius
    // This decision is made to pass the data easy to the compute shader
    private Vector4 waterSourceInfo = new Vector4();
    #endregion

    #region Properties
    /// <summary>
    /// X: uvHit.x, Y: uvHit.y, Z: brushStrength, W: brushRadius
    /// </summary>
    public Vector4 WaterSourceInfo
    {
        get
        {
            waterSourceInfo.Set(uvHit.x, uvHit.y, waterBrushStrength, waterBrushRadius);
            return waterSourceInfo;
        }
    }
    #endregion

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (waterSourceEnabled)
            CheckCollision();
        else
            ResetHitPosition();
    }

    private void CheckCollision()
    {
        RaycastHit hit;
        bool isCollision = Physics.Raycast(transform.position, -transform.up, out hit, collisionDistance, 1 << collisionLayer);

        if (isCollision)
        {
            // Set uv hit
            uvHit.Set(hit.textureCoord.x, hit.textureCoord.y);
            #if UNITY_EDITOR
            Debug.DrawLine(transform.position, hit.point, Color.blue);
            #endif
        }
        else
        {
            #if UNITY_EDITOR
            Debug.DrawRay(transform.position, -transform.up * collisionDistance, Color.blue);
            #endif
            // Set to the max value if no hit happens
            ResetHitPosition();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.2f);
    }

    private void Initialize()
    {
        waterPipeSimulation = FindObjectOfType<ComputeMeshPaintWaterPipe>();
        if (waterPipeSimulation == null)
            Debug.LogError("Water sources only work in combination with ComputeMeshPaintWaterPipe script! Check if this script is in the scene.");

        collisionLayer = LayerMask.NameToLayer(collisionLayerName);

        // Register at the water simulation
        waterPipeSimulation.AddWaterSource(this);
    }

    private void ResetHitPosition()
    {
        uvHit.Set(float.MaxValue, float.MaxValue);
    }

    public void ToggleSwitch()
    {
        waterSourceEnabled = !waterSourceEnabled;
    }

    private void OnDestroy()
    {
        // Remove registration from water simulation
        waterPipeSimulation.RemoveWaterSource(this);
    }
}