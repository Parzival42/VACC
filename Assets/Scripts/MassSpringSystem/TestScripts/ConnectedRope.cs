using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(TubeMeshUpdater), typeof(TubeGenerator))]
public class ConnectedRope : MonoBehaviour {

    #region variables
    [SerializeField]
    private Transform startConnection;

    [SerializeField]
    private Transform endConnection;

    [SerializeField]
    private int connections = 25;

    [SerializeField]
    [Range(0.05f, 2.0f)]
    private float widthScale = 1.0f;

    [SerializeField]
    [Range(0.05f, 2.0f)]
    private float lengthScale = 1.0f;

    [SerializeField]
    [Range(1.15f, 35.0f)]
    private float springStrength = 1.5f;


    [SerializeField]
    private bool endConstraintUsed = false;

    private List<PointMass> middleMassPointLine;
    private List<PointMass> upperMassPointLine;
    private List<PointMass> lowerMassPointLine;
    private List<PointMass> leftMassPointLine;
    private List<PointMass> rightMassPointLine;
    private List<Constraint> constraintList;

    //test stuff
    private List<Transform> collisonObjects;

    private TubeGenerator tubeGenerator;
    private Tube tube;

    private TubeMeshUpdater tubeMeshUpdater;
    private TubeScaler tubeScaler;

    private bool isInitialized = false;
    private bool usesMeshFilter = true;
    private MonoBehaviour gravitationForce;
    #endregion

    #region methods
    // Use this for initialization
    void Start () {
        

        middleMassPointLine = new List<PointMass>();
        upperMassPointLine = new List<PointMass>();
        lowerMassPointLine = new List<PointMass>();
        leftMassPointLine = new List<PointMass>();
        rightMassPointLine = new List<PointMass>();
        constraintList = new List<Constraint>();
        collisonObjects = new List<Transform>();

        tubeGenerator = GetComponent<TubeGenerator>();
        Tube tube = tubeGenerator.GenerateTube(widthScale);
        tubeScaler = GetComponent<TubeScaler>();
        tubeScaler.TubeSegments = tube.Bones;
        tubeMeshUpdater = GetComponent<TubeMeshUpdater>();
        tubeMeshUpdater.Initialize(tube, startConnection);

        transform.rotation = startConnection.rotation;
        
        gravitationForce = gameObject.AddComponent<Gravitation>();
        Setup();
        isInitialized = true;
    }

    private void Setup()
    {       
        GenerateMassPointLines();
        ConnectMassPoints();
        GenerateCollisionObjects();
    }
    private void GenerateCollisionObjects()
    {
        collisonObjects = new List<Transform>();
        for (int i = 0; i < connections; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = middleMassPointLine[i].Position;
            go.transform.localScale = new Vector3(widthScale * 3, widthScale * 3, widthScale * 3);
            go.GetComponent<Renderer>().enabled = false;
            go.layer = LayerMask.NameToLayer("MassSpring");
            Rigidbody rigid = go.AddComponent<Rigidbody>();
            rigid.useGravity = false;
            collisonObjects.Add(go.transform);
        }
    }


    private void ConnectMassPoints()
    {
        for (int i = 0; i < middleMassPointLine.Count - 1; i++)
        {
            //line connections
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], middleMassPointLine[i + 1]));
            constraintList.Add(new RegularConstraint(upperMassPointLine[i], upperMassPointLine[i + 1]));
            constraintList.Add(new RegularConstraint(lowerMassPointLine[i], lowerMassPointLine[i + 1]));
            constraintList.Add(new RegularConstraint(leftMassPointLine[i], leftMassPointLine[i + 1]));
            constraintList.Add(new RegularConstraint(rightMassPointLine[i], rightMassPointLine[i + 1]));

            //cross connection for inner stability
            constraintList.Add(new RegularConstraint(upperMassPointLine[i], lowerMassPointLine[i + 1]));
            constraintList.Add(new RegularConstraint(lowerMassPointLine[i], upperMassPointLine[i + 1]));
            constraintList.Add(new RegularConstraint(leftMassPointLine[i], rightMassPointLine[i + 1]));
            constraintList.Add(new RegularConstraint(rightMassPointLine[i], leftMassPointLine[i + 1]));
        }
        for (int i = 0; i < middleMassPointLine.Count; i++)
        {
            //connect outer lines to the inner one
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], upperMassPointLine[i]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], lowerMassPointLine[i]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], leftMassPointLine[i]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], rightMassPointLine[i]));

            //connect neighbour lines
            constraintList.Add(new RegularConstraint(upperMassPointLine[i], leftMassPointLine[i]));
            constraintList.Add(new RegularConstraint(upperMassPointLine[i], rightMassPointLine[i]));
            constraintList.Add(new RegularConstraint(lowerMassPointLine[i], leftMassPointLine[i]));
            constraintList.Add(new RegularConstraint(lowerMassPointLine[i], rightMassPointLine[i]));
        }

        for (int i = 2; i < middleMassPointLine.Count; i++)
        {
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], upperMassPointLine[i - 2]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], lowerMassPointLine[i - 2]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], leftMassPointLine[i - 2]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], rightMassPointLine[i - 2]));

            constraintList.Add(new RegularConstraint(middleMassPointLine[i], upperMassPointLine[i - 1]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], lowerMassPointLine[i - 1]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], leftMassPointLine[i - 1]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], rightMassPointLine[i - 1]));
        }
    }


    private void GenerateMassPointLines()
    {
        for (int i = 0; i < connections; i++)
        {
            if (i == 0 || (i == connections - 1 && endConstraintUsed))
            {
                //middleMassPointLine.Add(new FixedPointMass(startConnection.position + startConnection.up * i * lengthScale));
                //upperMassPointLine.Add(new FixedPointMass(startConnection.position + startConnection.forward * widthScale + startConnection.up * i * lengthScale));
                //lowerMassPointLine.Add(new FixedPointMass(startConnection.position - startConnection.forward * widthScale + startConnection.up * i * lengthScale));
                //leftMassPointLine.Add(new FixedPointMass(startConnection.position - startConnection.right * widthScale + startConnection.up * i * lengthScale));
                //rightMassPointLine.Add(new FixedPointMass(startConnection.position + startConnection.right * widthScale + startConnection.up * i * lengthScale));

                middleMassPointLine.Add(new FixedPointMass(startConnection.position + startConnection.forward * i * lengthScale));
                upperMassPointLine.Add(new FixedPointMass(startConnection.position + startConnection.up * widthScale + startConnection.forward * i * lengthScale));
                lowerMassPointLine.Add(new FixedPointMass(startConnection.position - startConnection.up * widthScale + startConnection.forward * i * lengthScale));
                leftMassPointLine.Add(new FixedPointMass(startConnection.position - startConnection.right * widthScale + startConnection.forward * i * lengthScale));
                rightMassPointLine.Add(new FixedPointMass(startConnection.position + startConnection.right * widthScale + startConnection.forward * i * lengthScale));
            }
            else
            {
                //middleMassPointLine.Add(new RegularPointMass(startConnection.position + startConnection.up * i * lengthScale));
                //upperMassPointLine.Add(new RegularPointMass(startConnection.position + startConnection.forward * widthScale + startConnection.up * i * lengthScale));

                //lowerMassPointLine.Add(new RegularPointMass(startConnection.position - startConnection.forward * widthScale + startConnection.up * i * lengthScale));
                //leftMassPointLine.Add(new RegularPointMass(startConnection.position - startConnection.right * widthScale + startConnection.up * i * lengthScale));
                //rightMassPointLine.Add(new RegularPointMass(startConnection.position + startConnection.right * widthScale + startConnection.up * i * lengthScale));

                middleMassPointLine.Add(new RegularPointMass(startConnection.position + startConnection.forward * i * lengthScale));
                upperMassPointLine.Add(new RegularPointMass(startConnection.position + startConnection.up * widthScale + startConnection.forward * i * lengthScale));

                lowerMassPointLine.Add(new RegularPointMass(startConnection.position - startConnection.up * widthScale + startConnection.forward * i * lengthScale));
                leftMassPointLine.Add(new RegularPointMass(startConnection.position - startConnection.right * widthScale + startConnection.forward * i * lengthScale));
                rightMassPointLine.Add(new RegularPointMass(startConnection.position + startConnection.right * widthScale + startConnection.forward * i * lengthScale));
            }
        }

    }

    void FixedUpdate()
    {
        middleMassPointLine[0].Position = startConnection.position;

        //upperMassPointLine[0].Position = startConnection.position + startConnection.forward * widthScale;
        //lowerMassPointLine[0].Position = startConnection.position - startConnection.forward * widthScale;
        //leftMassPointLine[0].Position = startConnection.position - startConnection.right * widthScale;
        //rightMassPointLine[0].Position = startConnection.position + startConnection.right * widthScale;

        upperMassPointLine[0].Position = startConnection.position + startConnection.up * widthScale;
        lowerMassPointLine[0].Position = startConnection.position - startConnection.up * widthScale;
        leftMassPointLine[0].Position = startConnection.position - startConnection.right * widthScale;
        rightMassPointLine[0].Position = startConnection.position + startConnection.right * widthScale;


        //middleMassPointLine[middleMassPointLine.Count-1].Position = endConnection.position;

        //upperMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position + startConnection.forward * widthScale;
        //lowerMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position - startConnection.forward * widthScale;
        //leftMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position - startConnection.right * widthScale;
        //rightMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position + startConnection.right * widthScale;

        if (endConstraintUsed)
        {
            upperMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position + startConnection.up * widthScale;
            lowerMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position - startConnection.up * widthScale;
            leftMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position - startConnection.right * widthScale;
            rightMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position + startConnection.right * widthScale;
            middleMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position;
        }



        for (int i = 1; i < collisonObjects.Count; i++)
        {
            middleMassPointLine[i].Position = collisonObjects[i].position;
        }

        //apply external forces
        for (int j = 0; j < middleMassPointLine.Count; j++)
        {
            middleMassPointLine[j].ApplyForce(((Force)gravitationForce).getForce());
            upperMassPointLine[j].ApplyForce(((Force)gravitationForce).getForce());
            lowerMassPointLine[j].ApplyForce(((Force)gravitationForce).getForce());
            leftMassPointLine[j].ApplyForce(((Force)gravitationForce).getForce());
            rightMassPointLine[j].ApplyForce(((Force)gravitationForce).getForce());
        }

       

       
        //update constraints
        for (int j = 0; j < 25; j++)
        {
            for (int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].Solve();
            }
        }

        //update pointmasses
        //for (int i = 0; i < middleMassPointLine.Count; i++)
        //{
        //    middleMassPointLine[i].Simulate(Time.deltaTime);
        //    upperMassPointLine[i].Simulate(Time.deltaTime);
        //    lowerMassPointLine[i].Simulate(Time.deltaTime);
        //    leftMassPointLine[i].Simulate(Time.deltaTime);
        //    rightMassPointLine[i].Simulate(Time.deltaTime);
        //}

        //middleMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position;

        for (int i = middleMassPointLine.Count-1; i >= 0; i--)
        {
            middleMassPointLine[i].Simulate(Time.deltaTime);
            upperMassPointLine[i].Simulate(Time.deltaTime);
            lowerMassPointLine[i].Simulate(Time.deltaTime);
            leftMassPointLine[i].Simulate(Time.deltaTime);
            rightMassPointLine[i].Simulate(Time.deltaTime);
        }

        //update the colliders position 
        for (int i = 0; i < collisonObjects.Count; i++)
        {
            collisonObjects[i].position = middleMassPointLine[i].Position;
        }


        //update mesh
        tubeMeshUpdater.UpdateMesh(middleMassPointLine);


        //force to fjuture sucker
        //endConnection.position = new Vector3( middleMassPointLine[middleMassPointLine.Count - 1].Position.x , endConnection.position.y, middleMassPointLine[middleMassPointLine.Count - 1].Position.z) ;
        //endConnection.LookAt(startConnection);
        //Vector3 distance = middleMassPointLine[middleMassPointLine.Count - 1].Position - endConnection.position;
        //Rigidbody rigid = endConnection.GetComponent<Rigidbody>();
        //rigid.AddForce(distance);


        ChangeConstraintSpringFactor(springStrength);
    }

    void ChangeConstraintSpringFactor(float springFactor)
    {
        for(int i = 0; i < constraintList.Count; i++)
        {
            constraintList[i].SetSpringFactor(springFactor);
        }
    }


    void OnDrawGizmos()
    {
        if (isInitialized)
        {
            Vector3 size = new Vector3(0.05f, 0.05f, 0.05f);
            for (int i = 0; i < middleMassPointLine.Count; i++)
            {
                middleMassPointLine[i].DrawPoint();
                upperMassPointLine[i].DrawPoint();
                lowerMassPointLine[i].DrawPoint();
                leftMassPointLine[i].DrawPoint();
                rightMassPointLine[i].DrawPoint();
            }

            for (int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].DrawConnection();
            }
        }
    }


    #endregion
}
