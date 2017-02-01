using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SkinnedMeshRenderer))]
public class TubeMeshUpdater : MonoBehaviour {

    #region variables
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float slider = 5.0f;


    //tube mesh
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh mesh;
    private Tube tube;
    private Transform[] bones;


    private bool isInitialized = false;

    #endregion

    #region properties
    public Tube Tube
    {
        set {
            tube = value;

            if (tube != null)
            {
                skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
                mesh = tube.Mesh;
                bones = tube.Bones;
                skinnedMeshRenderer.sharedMesh = mesh;
                skinnedMeshRenderer.bones = bones;
            }
            isInitialized = true;
        }
    }
    #endregion

    #region methods
	
	// Update is called once per frame
	public void UpdateMesh (List<PointMass> pointmassPositions) {
        if (isInitialized)
        {
            for(int i = 0; i < bones.Length; i++)
            {
               bones[bones.Length-i-1].position = pointmassPositions[i].Position;
            }
        }
	}

    #endregion
}
