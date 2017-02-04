using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TubeMeshUpdater), typeof(TubeGenerator))]
public class TubeMSDSystem : CoreMSDSystem {

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
    private int fixedStartSegments = 2;

    [SerializeField]
    private int fixedEndSegements = 2;

    [SerializeField]
    private bool endConstraintUsed = false;

    private List<PointMass> middleMassPointLine;
    private List<PointMass> upperMassPointLine;
    private List<PointMass> lowerMassPointLine;
    private List<PointMass> leftMassPointLine;
    private List<PointMass> rightMassPointLine;


    private List<Transform> collisonObjects;

    private TubeGenerator tubeGenerator;
    private Tube tube;

    private TubeMeshUpdater tubeMeshUpdater;
    private TubeScaler tubeScaler;

    private MonoBehaviour gravitationForce;
    #endregion

    #region methods

    public override void GenerateConstraints()
    {
        constraintList = new List<Constraint>();

        for (int i = 0; i < middleMassPointLine.Count; i++)
        {
            if(i < middleMassPointLine.Count - 1)
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

            if (i > 1)
            {
                //connection from the outer lines to the inner ones (2 points apart)
                constraintList.Add(new RegularConstraint(middleMassPointLine[i], upperMassPointLine[i - 2]));
                constraintList.Add(new RegularConstraint(middleMassPointLine[i], lowerMassPointLine[i - 2]));
                constraintList.Add(new RegularConstraint(middleMassPointLine[i], leftMassPointLine[i - 2]));
                constraintList.Add(new RegularConstraint(middleMassPointLine[i], rightMassPointLine[i - 2]));
            }

            if(i > 0)
            {
                //connection from the outer lines to the inner ones (1 point apart)
                constraintList.Add(new RegularConstraint(middleMassPointLine[i], upperMassPointLine[i - 1]));
                constraintList.Add(new RegularConstraint(middleMassPointLine[i], lowerMassPointLine[i - 1]));
                constraintList.Add(new RegularConstraint(middleMassPointLine[i], leftMassPointLine[i - 1]));
                constraintList.Add(new RegularConstraint(middleMassPointLine[i], rightMassPointLine[i - 1]));
            }
        }
    }

    public override void GeneratePointMasses()
    {
        middleMassPointLine = new List<PointMass>();
        upperMassPointLine = new List<PointMass>();
        lowerMassPointLine = new List<PointMass>();
        leftMassPointLine = new List<PointMass>();
        rightMassPointLine = new List<PointMass>();
        pointList = new List<PointMass>();

        for (int i = 0; i < connections; i++)
        {
            if (i < fixedStartSegments || (i > connections - fixedEndSegements && endConstraintUsed))
            {
                middleMassPointLine.Add(new FixedPointMass(startConnection.position + startConnection.forward * i * lengthScale));
                upperMassPointLine.Add(new FixedPointMass(startConnection.position + startConnection.up * widthScale + startConnection.forward * i * lengthScale));
                lowerMassPointLine.Add(new FixedPointMass(startConnection.position - startConnection.up * widthScale + startConnection.forward * i * lengthScale));
                leftMassPointLine.Add(new FixedPointMass(startConnection.position - startConnection.right * widthScale + startConnection.forward * i * lengthScale));
                rightMassPointLine.Add(new FixedPointMass(startConnection.position + startConnection.right * widthScale + startConnection.forward * i * lengthScale));
            }
            else
            {
                middleMassPointLine.Add(new RegularPointMass(startConnection.position + startConnection.forward * i * lengthScale));
                upperMassPointLine.Add(new RegularPointMass(startConnection.position + startConnection.up * widthScale + startConnection.forward * i * lengthScale));

                lowerMassPointLine.Add(new RegularPointMass(startConnection.position - startConnection.up * widthScale + startConnection.forward * i * lengthScale));
                leftMassPointLine.Add(new RegularPointMass(startConnection.position - startConnection.right * widthScale + startConnection.forward * i * lengthScale));
                rightMassPointLine.Add(new RegularPointMass(startConnection.position + startConnection.right * widthScale + startConnection.forward * i * lengthScale));
            }
            pointList.Add(middleMassPointLine[i]);
            pointList.Add(upperMassPointLine[i]);
            pointList.Add(lowerMassPointLine[i]);
            pointList.Add(leftMassPointLine[i]);
            pointList.Add(rightMassPointLine[i]);
        }
    }

    public override void PostSimulationStep()
    {
        //update the colliders position 
        for (int i = 0; i < collisonObjects.Count; i++)
        {
            collisonObjects[i].position = middleMassPointLine[i].Position;
        }

        //update mesh
        tubeMeshUpdater.UpdateMesh(middleMassPointLine);

        //adjust spring factor
        for (int i = 0; i < constraintList.Count; i++)
        {
            constraintList[i].SetSpringFactor(springStrength);
        }
    }

    public override void PreSimulationStep()
    {
        //update the mass points that are attached to 
        for(int i = 0; i < fixedStartSegments; i++)
        {
            middleMassPointLine[i].Position = startConnection.position + startConnection.forward* i *lengthScale;
            upperMassPointLine[i].Position = startConnection.position + startConnection.up * widthScale + startConnection.forward * i * lengthScale;
            lowerMassPointLine[i].Position = startConnection.position - startConnection.up * widthScale + startConnection.forward * i * lengthScale;
            leftMassPointLine[i].Position = startConnection.position - startConnection.right * widthScale + startConnection.forward * i * lengthScale;
            rightMassPointLine[i].Position = startConnection.position + startConnection.right * widthScale + startConnection.forward * i * lengthScale;
        }

        //do the same for the last mass points if the rope should be anchored between two points
        if (endConstraintUsed)
        {
            middleMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position;
            upperMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position + startConnection.up * widthScale;
            lowerMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position - startConnection.up * widthScale;
            leftMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position - startConnection.right * widthScale;
            rightMassPointLine[middleMassPointLine.Count - 1].Position = endConnection.position + startConnection.right * widthScale;
        }


        for (int i = 1; i < collisonObjects.Count-1; i++)
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
    }

    public override void Initialize()
    {
        GenerateCollisionObjects();

        tubeGenerator = GetComponent<TubeGenerator>();
        Tube tube = tubeGenerator.GenerateTube(widthScale);
        tubeScaler = GetComponent<TubeScaler>();
        tubeScaler.TubeSegments = tube.Bones;
        tubeMeshUpdater = GetComponent<TubeMeshUpdater>();
        tubeMeshUpdater.Initialize(tube, startConnection);

        transform.rotation = startConnection.rotation;

        gravitationForce = gameObject.AddComponent<Gravitation>();
    }

    private void GenerateCollisionObjects()
    {
        collisonObjects = new List<Transform>();
        for (int i = 0; i < connections-2; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = middleMassPointLine[i].Position;
            go.transform.localScale = new Vector3(widthScale * 3, widthScale * 3, widthScale * 3);
            go.GetComponent<Renderer>().enabled = false;
            go.layer = LayerMask.NameToLayer("MassSpring");
            go.tag = "Silent";
            Rigidbody rigid = go.AddComponent<Rigidbody>();
            rigid.useGravity = false;
            collisonObjects.Add(go.transform);
        }
    }

    #endregion
}
