using UnityEngine;
using System.Collections;

public class ComputeTest : MonoBehaviour {

    [SerializeField]
    private ComputeShader computeShader;
    private ComputeBuffer computeBuffer;

    private PointMassComponents[] pmcArray = new PointMassComponents[12*10000];
    private PointMassComponents[] pmcArrayNew = new PointMassComponents[12*10000];

    struct PointMassComponents
    {
        public Vector3 currentPosition;
        public Vector3 previousPosition;
        public Vector3 acceleration;
    }

    private int kernelHandle;

    //debug
    private Vector3 boxSize = new Vector3(0.5f, 0.5f, 0.5f);
    private bool executedOnce = false;

	// Use this for initialization
	void Start () {
	
        for(int i = 0; i < pmcArray.Length; i++)
        {
            pmcArray[i] = new PointMassComponents();
            pmcArray[i].currentPosition = new Vector3(i,0,0);
            pmcArray[i].previousPosition = new Vector3(i - 1, 0, 0);
            pmcArray[i].acceleration = new Vector3(0.05f,0,0);
        }

        kernelHandle = computeShader.FindKernel("CSSimulate");
       
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        RunShader();
	}

    private void RunShader()
    {
        computeBuffer = new ComputeBuffer(pmcArray.Length, 3 * 3 * 4  /*3 vectors with the size of 12 byte*/);
        computeBuffer.SetData(pmcArray);
        computeShader.SetBuffer(kernelHandle, "Result", computeBuffer);
        computeShader.Dispatch(kernelHandle, pmcArray.Length/24,1,1);

        computeBuffer.GetData(pmcArray);

        computeBuffer.Release();
        

        if (!executedOnce)
        {
            executedOnce = true;
        }

    }


    void OnDrawGizmos()
    {
        if (executedOnce)
        {
            //for (int i = 0; i < pmcArray.Length; i++)
            //{
            //    Gizmos.DrawCube(pmcArray[i].currentPosition, boxSize);
            //}

            Gizmos.DrawCube(pmcArray[0].currentPosition, boxSize);
        }

    }
}
