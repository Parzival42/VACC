using System;
using UnityEngine;

public delegate void TubeDeformHandler(DeformType deformType);

public class SuckingScript : MonoBehaviour
{

    public static event TubeDeformHandler TubeDeform;

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

    [Header("Nitrous Oxide")]
    [Tooltip("Determines which objects can be sucked in")]
    [SerializeField]
    private NOS currentStage = NOS.Stage1;

    [Header("Collision settings")]
    [SerializeField]
    private int collisionRays = 5;

    [SerializeField]
    protected float checkAngle = 30.0f;

    [SerializeField]
    private float collisionMaxLength = 0.6f;

    private DragScript drag;
    private bool dustSuckerActive = false;

	private void Start ()
    {
        CheckFields();
        drag = GetComponent<DragScript>();
        meltShader = Shader.Find(meltShaderID);
        DustSuckerSwitch.DustSuckerStatus += ChangeDustSuckerStatus;
        ConnectorDragAndDrop.DustSuckerConnectionLost += ChangeDustSuckerStatus;
        NitrousOxideSystems.NitrousStageChanged += ChangeNitrousStage;
    }

    private void ChangeNitrousStage(int change)
    {
        currentStage += change;
    }

    private void ChangeDustSuckerStatus()
    {
        ChangeDustSuckerStatus(false);
    }

    private void ChangeDustSuckerStatus(bool status)
    {
        dustSuckerActive = status;
    }

    private void CheckFields()
    {
        if (suckingPoint == null)
            Debug.LogError("No sucking point specified, you failed in every aspect!");
    }
	
	private void Update ()
    {
        if (drag.IsDragged && dustSuckerActive)
        {
            CheckCollisionTerrain();
            CheckCollisionMeshes();
        }
	}

    protected void CheckCollisionTerrain()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(suckingPoint.position, -suckingPoint.up, out hitInfo, suckingDistanceTerrain, 1 << collisionLayerTerrain);

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

            OnTubeDeform(DeformType.Dust);
        }
    }

    protected void CheckCollisionMeshes()
    {
        RaycastHit hitInfo;
        //bool hit = Physics.Raycast(suckingPoint.position, suckingPoint.forward, out hitInfo, suckingDistanceMeshes, 1 << collisionLayerMeshes);
        bool hit = CheckForCollision(out hitInfo, collisionLayerMeshes);

        if (hit)
        {
            NOS stage = NOS.Stage1;
            for(int i = 0; i <= (int)currentStage; i++)
            {
                if (hitInfo.transform.gameObject.tag == stage.ToString())
                {
                    DeformType deformType = (DeformType)i+1;
                    OnTubeDeform(deformType);
                    PlaySound();
                    ChangeToMeltMaterial(hitInfo.transform.gameObject);
                    hitInfo.transform.gameObject.SendMessage("IsGettingSuckedIn", currentStage, SendMessageOptions.DontRequireReceiver);
                }
                stage++;
            }
        }
    }

    private void PlaySound()
    {
        FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance("event:/Slurp");
        e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(e, gameObject.transform, GetComponent<Rigidbody>());
        e.start();
        e.release();
    }

    private bool CheckForCollision(out RaycastHit hit, int collisionLayer)
    {
        hit = new RaycastHit();

        // The angle between 2 raycasts.
        float angleBetween = checkAngle / collisionRays;

        // The current angle.
        float currentAngle = -(checkAngle * 0.5f);

        // Specifies if there was no hit at all.
        bool neverHit = true;

        for (int i = 0; i < collisionRays; i++)
        {
            Quaternion rotation = Quaternion.Euler(new Vector3(0f, currentAngle, 0f));
            Vector3 direction = rotation * transform.forward;

            bool objHit = Physics.Raycast(suckingPoint.position, direction, out hit, collisionMaxLength, 1 << collisionLayer);

            #if UNITY_EDITOR
            Debug.DrawLine(suckingPoint.position, suckingPoint.position + direction * collisionMaxLength, Color.yellow);
            #endif

            if (objHit)
                return true;

            currentAngle += angleBetween;
        }
        return !neverHit;
    }

    private void ChangeToMeltMaterial(GameObject obj)
    {
        Material currMaterial = obj.GetComponent<Renderer>().material;
        if (currMaterial)
        {
            Color color = currMaterial.GetColor("_Color");
            Texture albedo = currMaterial.GetTexture("_MainTex");
            Texture normals = currMaterial.GetTexture("_BumpMap");
            currMaterial.shader = meltShader;
            currMaterial.SetColor("_Color", color);
            currMaterial.SetFloat("_MeltY", suckingPoint.position.y);
            currMaterial.SetVector("_MeltPosition", suckingPoint.position);
            currMaterial.SetTexture("_NormalTex", normals);
            currMaterial.SetTexture("_MainTex", albedo);

            Collider[] pinaColada = obj.GetComponents<Collider>();
            foreach (Collider cocktail in pinaColada)
            {
                cocktail.enabled = false;
            }

            Rigidbody warmBeer = obj.GetComponent<Rigidbody>();
            warmBeer.drag = 0f;
            warmBeer.angularDrag = 0f;
            warmBeer.useGravity = false;
            warmBeer.isKinematic = false;
            warmBeer.constraints = RigidbodyConstraints.None;

            Sucked suckedIn = obj.AddComponent<Sucked>();
            suckedIn.finalPosition = obj.transform.position - Vector3.up * 2.5f;
            suckedIn.suckDirection = transform;
        }
    }


    private void OnTubeDeform(DeformType deformType)
    {
        if (TubeDeform != null)
        {
            TubeDeform(deformType);
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