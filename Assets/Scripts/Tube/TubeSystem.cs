using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class TubeSystem : MonoBehaviour {

    SkinnedMeshRenderer skinnedMeshRenderer;
    TubeGenerator tubeGenerator;
    Tube tube;

    [SerializeField]
    private Transform ropeStart;

    [SerializeField]
    private Transform ropeEnd;

    [SerializeField]
    private bool now = false;

    private Vector3 gizmoSize;

	// Use this for initialization
	void Start () {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (now)
        {
            now = !now;
            Init();
        }
        Debug.DrawLine(ropeStart.position, ropeEnd.position, Color.green, 1, false);
    }


    private void Init()
    {
        tubeGenerator = gameObject.GetComponent<TubeGenerator>();
        tube = tubeGenerator.GenerateTube();

        Transform[] bones = tube.Bones;
        for (int i = 1; i < bones.Length; i++)
        {

            CharacterJoint characterJoint = bones[i].gameObject.AddComponent<CharacterJoint>();
            characterJoint.connectedBody = bones[i - 1].GetComponent<Rigidbody>();

        }

        gizmoSize = Vector3.one;


        skinnedMeshRenderer.sharedMesh = tube.Mesh;
        skinnedMeshRenderer.bones = tube.Bones;
    }




    void OnDrawGizmos()
    {
        if(ropeStart != null && ropeEnd != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(ropeStart.position, 1);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ropeEnd.position, 1);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(ropeStart.position, ropeEnd.position);
        }
    }
}
