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
    private Transform startConnection;

    private bool isInitialized = false;

    #endregion


    #region methods
	
    public void Initialize(Tube tube, Transform startConnection)
    {
        this.tube = tube;

        if (tube != null)
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            mesh = tube.Mesh;
            bones = tube.Bones;
            for(int i = 0; i < bones.Length; i++)
            {
                bones[i].rotation = startConnection.rotation;
            }

            skinnedMeshRenderer.sharedMesh = mesh;
            skinnedMeshRenderer.bones = bones;
        }

        this.startConnection = startConnection;
        isInitialized = true;
    }

	// Update is called once per frame
	public void UpdateMesh (List<PointMass> pointmassPositions) {
        if (isInitialized)
        {
            for(int i = 0; i < bones.Length; i++)
            {
                bones[i].position = pointmassPositions[i].Position;
                bones[i].rotation = startConnection.rotation;
                //bones[i].rotation = AdjustRotation(i, pointmassPositions);
                //
                AdjustRotation(i, pointmassPositions, bones);
                bones[i].Rotate(startConnection.up, 180);
            }
        }
	}

    private void AdjustRotation(int index, List<PointMass> pointMassPositions, Transform[] bones)
    {
        if (index > 0 && index < pointMassPositions.Count - 1)
        {
            Vector3 prev = pointMassPositions[index - 1].Position;
            Vector3 next = pointMassPositions[index + 1].Position;
            Vector3 direction = next - prev;
            direction.Normalize();

            bones[index].rotation = Quaternion.LookRotation(direction);
            //bones[index].Rotate(Vector3.up, 180);
        }
    }

    private Vector3 CalculateDirection(int index, List<PointMass> pointMassPositions)
    {
        Vector3 result = pointMassPositions[index].Position + Vector3.forward;
        if (index > 0 && index < pointMassPositions.Count - 1)
        {
            Vector3 prev = pointMassPositions[index - 1].Position;
            Vector3 next = pointMassPositions[index + 1].Position;
            Vector3 direction = next - prev;
            direction.Normalize();

           // Debug.DrawLine(startConnection.position , startConnection.position + startConnection.forward);


            Vector3 angleCorrector = new Vector3();
            angleCorrector.x = AngleInDeg(direction.y, direction.z, startConnection.up.y, startConnection.up.z);
            angleCorrector.y = AngleInDeg(direction.x, direction.z, startConnection.up.x, startConnection.up.z);
            angleCorrector.z = AngleInDeg(direction.x, direction.y, startConnection.up.x, startConnection.up.y);




            //startConnection.position += startConnection.up * Time.deltaTime;

            //result = pointMassPositions[index].Position + direction.normalized;


            result = angleCorrector;
            if(index < 5)
            {
             //   Debug.Log("Rotation: " + angleCorrector);
            }


        }
        return result;

    }

   private float AngleInDeg(float v1, float v2, float w1, float w2)
    {
        return Mathf.Atan2(w2 - v2, w1 - v1) * 180 / Mathf.PI;
    }



    #endregion
}
