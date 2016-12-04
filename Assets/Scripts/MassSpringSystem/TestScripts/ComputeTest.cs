using UnityEngine;
using System.Collections;

public class ComputeTest : MonoBehaviour {

    [SerializeField]
    private ComputeShader computeShader;
    private ComputeBuffer computeBuffer;

    private PointMassComponents[] pmcArray = new PointMassComponents[12*20000];
    private PointMassComponents[] pmcArrayNew = new PointMassComponents[12*20000];


    private ComputeBuffer bufferCurrentPosition;
    private ComputeBuffer bufferPreviousPosition;
    private ComputeBuffer bufferacceleration;

    private Vector3[] currentPositions = new Vector3[120000];
    private Vector3[] previousPosition = new Vector3[120000];
    private Vector3[] acceleration = new Vector3[120000];

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

        for(int i = 0; i < currentPositions.Length; i++)
        {
            currentPositions[i] = new Vector3(i, 0, 0);
            previousPosition[i]= new Vector3(i - 1, 0, 0);
            acceleration[i] = new Vector3(0.02f, 0, 0);
        }


        kernelHandle = computeShader.FindKernel("CSSimulate");
       
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        RunShaderNew();
	}

    private void RunShader()
    {
        kernelHandle = computeShader.FindKernel("CSSimulate");
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


    private void RunShaderNew()
    {
        kernelHandle = computeShader.FindKernel("CSSimulateNew");

        bufferCurrentPosition = new ComputeBuffer(currentPositions.Length, 3 * 4);
        bufferPreviousPosition = new ComputeBuffer(previousPosition.Length, 3 * 4);
        bufferacceleration = new ComputeBuffer(acceleration.Length, 3 * 4);

        bufferCurrentPosition.SetData(currentPositions);
        bufferPreviousPosition.SetData(previousPosition);
        bufferacceleration.SetData(acceleration);

        computeShader.SetBuffer(kernelHandle, "currentPosition", bufferCurrentPosition);
        computeShader.SetBuffer(kernelHandle, "previousPosition", bufferPreviousPosition);
        computeShader.SetBuffer(kernelHandle, "acceleration", bufferacceleration);

        computeShader.Dispatch(kernelHandle, currentPositions.Length / 24, 1, 1);

        bufferCurrentPosition.GetData(currentPositions);


        bufferCurrentPosition.Release();
        bufferPreviousPosition.Release();
        bufferacceleration.Release();
        

        if (!executedOnce)
        {
            executedOnce = true;
        }
    }

    //void OnDisable()
    //{
    //    computeBuffer.Release();
    //}


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
