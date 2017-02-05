using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(LineRenderer))]
public class CableMSDSystem : CoreMSDSystem {

    

    [SerializeField]
    private Transform startConnection;

    [SerializeField]
    private Transform endConnection;

    [SerializeField]
    private int connections = 25;

    [SerializeField]
    [Range(0.01f, 2.0f)]
    private float widthScale = 1.0f;

    [SerializeField]
    [Range(0.01f, 2.0f)]
    private float lengthScale = 1.0f;

    [SerializeField]
    [Range(1.15f, 35.0f)]
    private float springStrength = 1.5f;

    [SerializeField]
    private int fixedStartSegments = 2;

    [SerializeField]
    private int fixedEndSegements = 2;

    [SerializeField]
    private bool endConstraintUsed = false;


    private LineRenderer lineRenderer;
    private List<Transform> collisonObjects;
    private MonoBehaviour gravitationForce;

   
    public override void GenerateConstraints()
    {
        constraintList = new List<Constraint>();

        for (int i = 0; i < pointList.Count-1; i++)
        {
            constraintList.Add(new DampedConstraint(pointList[i], pointList[i + 1], 0.075f));
        }
    }

    public override void GeneratePointMasses()
    {      
        pointList = new List<PointMass>();

        for (int i = 0; i < connections; i++)
        {
            if (i < fixedStartSegments || (i > connections - fixedEndSegements && endConstraintUsed))
            {
                pointList.Add(new FixedPointMass(startConnection.position + startConnection.forward * i * lengthScale));
            }
            else
            {
                pointList.Add(new RegularPointMass(startConnection.position + startConnection.forward * i * lengthScale));
            }
        }
    }

    public override void Initialize()
    {

        GenerateCollisionObjects();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.numPositions = pointList.Count;

        for (int i = 0; i < pointList.Count; i++)
        {
            lineRenderer.SetPosition(i, pointList[i].Position);
        }

        gravitationForce = gameObject.AddComponent<Gravitation>();

    }

    public override void PostSimulationStep()
    {
        //update the colliders position 
        for (int i = 0; i < collisonObjects.Count; i++)
        {
            collisonObjects[i].position = pointList[i].Position;
            collisonObjects[i].rotation = Quaternion.identity;
        }

        //update mesh
        for(int i = 0; i < pointList.Count; i++)
        {
            lineRenderer.SetPosition(i, pointList[i].Position);
        }

        //adjust spring factor
        for (int i = 0; i < constraintList.Count; i++)
        {
            constraintList[i].SetSpringFactor(springStrength);
        }
    }

    public override void PreSimulationStep()
    {
        //update the mass points that are attached to 
        for (int i = 0; i < fixedStartSegments; i++)
        {
           pointList[i].Position = startConnection.position + startConnection.forward * i * lengthScale;
        }

        //do the same for the last mass points if the rope should be anchored between two points
        if (endConstraintUsed)
        {
            pointList[pointList.Count - 1].Position = endConnection.position;
        }


        for (int i = 1; i < collisonObjects.Count - 1; i++)
        {
            pointList[i].Position = collisonObjects[i].position;
        }

        //apply external forces
        for (int j = 0; j < pointList.Count; j++)
        {
            pointList[j].ApplyForce(((Force)gravitationForce).getForce());
        }
    }

    private void GenerateCollisionObjects()
    {
        collisonObjects = new List<Transform>();
        for (int i = 0; i < connections - 2; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = pointList[i].Position;
            go.transform.localScale = new Vector3(widthScale * 3, widthScale * 3, widthScale * 3);
            go.GetComponent<Renderer>().enabled = false;
            go.layer = LayerMask.NameToLayer("MassSpring");
            go.tag = "Silent";
            Rigidbody rigid = go.AddComponent<Rigidbody>();
            rigid.useGravity = false;
            rigid.constraints = RigidbodyConstraints.FreezeRotation;
            collisonObjects.Add(go.transform);
        }
    }
}
