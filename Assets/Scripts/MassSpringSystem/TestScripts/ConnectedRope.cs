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
    private float forceValue = 10.0f;

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
        tubeMeshUpdater = GetComponent<TubeMeshUpdater>();
        tubeMeshUpdater.Tube = tubeGenerator.GenerateTube();
        
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
        for(int i = 0; i < connections; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = middleMassPointLine[i].Position;
            go.transform.localScale = new Vector3(2, 2, 2);
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
            //connect outer lines to the inner one
            //middleMassPointLine[i].ConnectTo(upperMassPointLine[i-2]);
            //middleMassPointLine[i].ConnectTo(lowerMassPointLine[i-2]);
            //middleMassPointLine[i].ConnectTo(leftMassPointLine[i-2]);
            //middleMassPointLine[i].ConnectTo(rightMassPointLine[i-2]);
            if (i > 2)
            {
                //constraintList.Add(new RegularConstraint(middleMassPointLine[i], upperMassPointLine[i - 3]));
                //constraintList.Add(new RegularConstraint(middleMassPointLine[i], lowerMassPointLine[i - 3]));
                //constraintList.Add(new RegularConstraint(middleMassPointLine[i], leftMassPointLine[i - 3]));
                //constraintList.Add(new RegularConstraint(middleMassPointLine[i], rightMassPointLine[i - 3]));
            }
           


            constraintList.Add(new RegularConstraint(middleMassPointLine[i], upperMassPointLine[i - 2]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], lowerMassPointLine[i - 2]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], leftMassPointLine[i - 2]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], rightMassPointLine[i - 2]));



            constraintList.Add(new RegularConstraint(middleMassPointLine[i], upperMassPointLine[i - 1]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], lowerMassPointLine[i - 1]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], leftMassPointLine[i - 1]));
            constraintList.Add(new RegularConstraint(middleMassPointLine[i], rightMassPointLine[i - 1]));

            //upperMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);
            //lowerMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);
            //leftMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);
            //rightMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);


            //middleMassPointLine[i].ConnectTo(upperMassPointLine[i - 2]);
            //middleMassPointLine[i].ConnectTo(lowerMassPointLine[i - 2]);
            //middleMassPointLine[i].ConnectTo(leftMassPointLine[i - 2]);
            //middleMassPointLine[i].ConnectTo(rightMassPointLine[i - 2]);

            //middleMassPointLine[i].ConnectTo(upperMassPointLine[i - 1]);
            //middleMassPointLine[i].ConnectTo(lowerMassPointLine[i - 1]);
            //middleMassPointLine[i].ConnectTo(leftMassPointLine[i - 1]);
            //middleMassPointLine[i].ConnectTo(rightMassPointLine[i - 1]);


            //upperMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);
            //lowerMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);
            //leftMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);
            //rightMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);

            //middleMassPointLine[i].ConnectTo(upperMassPointLine[i - 2]);
            //middleMassPointLine[i].ConnectTo(lowerMassPointLine[i - 2]);
            //middleMassPointLine[i].ConnectTo(leftMassPointLine[i - 2]);
            //middleMassPointLine[i].ConnectTo(rightMassPointLine[i - 2]);

            //middleMassPointLine[i].ConnectTo(upperMassPointLine[i - 1]);
            //middleMassPointLine[i].ConnectTo(lowerMassPointLine[i - 1]);
            //middleMassPointLine[i].ConnectTo(leftMassPointLine[i - 1]);
            //middleMassPointLine[i].ConnectTo(rightMassPointLine[i - 1]);


            //upperMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);
            //lowerMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);
            //leftMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);
            //rightMassPointLine[i - 2].ConnectTo(middleMassPointLine[i]);

            ////connect neighbour lines
            //upperMassPointLine[i].ConnectTo(leftMassPointLine[i]);
            //upperMassPointLine[i].ConnectTo(rightMassPointLine[i]);
            //lowerMassPointLine[i].ConnectTo(leftMassPointLine[i]);
            //lowerMassPointLine[i].ConnectTo(rightMassPointLine[i]);
        }
    }


    private void GenerateMassPointLines()
    {
        Vector3 center = new Vector3(0,0,0);
        for (int i = 0; i < connections; i++)
        {
            if (i == 0 || i == connections + 1)
            {
                middleMassPointLine.Add(new FixedPointMass(Vector3.zero));
                upperMassPointLine.Add(new FixedPointMass(Vector3.up));
                lowerMassPointLine.Add(new FixedPointMass(Vector3.down));
                leftMassPointLine.Add(new FixedPointMass(Vector3.left));
                rightMassPointLine.Add(new FixedPointMass(Vector3.right));
            }
            else
            {
                middleMassPointLine.Add(new RegularPointMass(Vector3.forward * (i+1)));
                upperMassPointLine.Add(new RegularPointMass(Vector3.up + Vector3.forward * (i+1)));

                lowerMassPointLine.Add(new RegularPointMass(Vector3.down + Vector3.forward * (i+1)));
                leftMassPointLine.Add(new RegularPointMass(Vector3.left + Vector3.forward * (i+1)));
                rightMassPointLine.Add(new RegularPointMass(Vector3.right + Vector3.forward * (i+1)));
            }
        }

    }

    void FixedUpdate()
    {
        middleMassPointLine[0].Position = startConnection.position;
        upperMassPointLine[0].Position = startConnection.position +Vector3.up;
        lowerMassPointLine[0].Position = startConnection.position + Vector3.down;
        leftMassPointLine[0].Position = startConnection.position + Vector3.left;
        rightMassPointLine[0].Position = startConnection.position + Vector3.right;


        middleMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position;

        for (int i = 0; i < collisonObjects.Count; i++)
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
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].Solve();
            }
        }

        //update pointmasses
        for (int i = 0; i < middleMassPointLine.Count; i++)
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

        Vector3 distance = middleMassPointLine[middleMassPointLine.Count - 1].Position - endConnection.position;
        Rigidbody rigid = endConnection.GetComponent<Rigidbody>();
        rigid.AddForce(distance);
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
