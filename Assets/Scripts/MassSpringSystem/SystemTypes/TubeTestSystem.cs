using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter), typeof(TubeGenerator))]
public class TubeTestSystem : MonoBehaviour {

    #region variables

    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float slider = 5.0f;


    //tube mesh
    private TubeGenerator tubeGenerator;
    private Tube tube;

    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<PointMass> middleMassPointLine;
    private List<PointMass> upperMassPointLine;
    private List<PointMass> lowerMassPointLine;
    private List<PointMass> leftMassPointLine;
    private List<PointMass> rightMassPointLine;

    private List<Constraint> constraintList;
 
    private int xAmount;
    private int yAmount;
    private Vector3[,] vertex2DRepresentation;

    private Vector3[] vertices;

    private bool isInitialized = false;

    private float distanceToCenter = 0.0f;
    #endregion

    #region methods
    void Start () {

        middleMassPointLine = new List<PointMass>();
        upperMassPointLine = new List<PointMass>();
        lowerMassPointLine = new List<PointMass>();
        leftMassPointLine = new List<PointMass>();
        rightMassPointLine = new List<PointMass>();

        constraintList = new List<Constraint>();


        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;


        tubeGenerator = GetComponent<TubeGenerator>();
        tube = tubeGenerator.GenerateTube();
        

        vertex2DRepresentation = tube.Vertex2DRepresentation;
        xAmount = tube.Vertex2DRepresentation.GetLength(0);
        yAmount = tube.Vertex2DRepresentation.GetLength(1);


        mesh = tube.Mesh;
        vertices = mesh.vertices;

        //GenerateMassPoints();
        GenerateMassPointLine();

        ConnectMassPoints();


        meshFilter.mesh = mesh;
        isInitialized = true;
	}

  

    void FixedUpdate () {

        //update springs
        for (int j = 0; j < 15; j++)
        {
            for (int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].Solve();
            }
        }

        //gravitation
        for (int i = 0; i < middleMassPointLine.Count; i++)
        {
            middleMassPointLine[i].ApplyForce(new Vector3(0, -slider * 50, 0 ));
            upperMassPointLine[i].ApplyForce(new Vector3(0, -slider * 50, 0));
            lowerMassPointLine[i].ApplyForce(new Vector3(0, -slider * 50, 0));
            leftMassPointLine[i].ApplyForce(new Vector3(0, -slider * 50, 0));
            rightMassPointLine[i].ApplyForce(new Vector3(0, -slider * 50, 0));
        }


        //update points
        for (int i = 0; i < middleMassPointLine.Count; i++)
        {
            middleMassPointLine[i].Simulate(Time.deltaTime);
            upperMassPointLine[i].Simulate(Time.deltaTime);
            lowerMassPointLine[i].Simulate(Time.deltaTime);
            leftMassPointLine[i].Simulate(Time.deltaTime);
            rightMassPointLine[i].Simulate(Time.deltaTime);
        }


        UpdateVertices();


        mesh.vertices = vertices;
    }


    private void UpdateVertices()
    {
        for (int i = 0; i < middleMassPointLine.Count; i++)
        {
            for (int j = 0; j < xAmount; j++)
            {
                vertices[i * xAmount + j].x = ( middleMassPointLine[i].Position).y;
            }
        }
    }


    //private void GenerateMassPoints()
    //{
    //    for(int i = 0; i < xAmount; i++)
    //    {
    //        for(int j = 0; j < yAmount; j++)
    //        {
    //            if(j==0 || j == yAmount - 1)
    //            {
    //                middleMassPointLine.Add(new RegularPointMass(gameObject.transform.TransformPoint(vertex2DRepresentation[i,j])));
                 
    //            }else
    //            {
    //                middleMassPointLine.Add(new RegularPointMass(gameObject.transform.TransformPoint(vertex2DRepresentation[i, j])));
    //            }
    //        }
    //    }
    //}

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


    private void GenerateMassPointLine()
    {
        Vector3 center = new Vector3();
        for (int i = 0; i < yAmount; i++)
        {
            center.Set(0, 0, 0);
            for (int j = 0; j < xAmount; j++)
            {
                center += vertex2DRepresentation[j, i];
            }
            center /= (float)xAmount;

            Vector3 direction = center - vertex2DRepresentation[0, i];

            if (i == 0 || i == yAmount + 1)
            {

                middleMassPointLine.Add(new FixedPointMass(gameObject.transform.TransformPoint(center)));
                upperMassPointLine.Add(new FixedPointMass(gameObject.transform.TransformPoint(center + direction)));
                lowerMassPointLine.Add(new FixedPointMass(gameObject.transform.TransformPoint(center - direction)));
                leftMassPointLine.Add(new FixedPointMass(gameObject.transform.TransformPoint(center - Vector3.Cross(direction, Vector3.up))));
                rightMassPointLine.Add(new FixedPointMass(gameObject.transform.TransformPoint(center + Vector3.Cross(direction, Vector3.up))));
            }
            else
            {
                middleMassPointLine.Add(new RegularPointMass(gameObject.transform.TransformPoint(center)));
                upperMassPointLine.Add(new RegularPointMass(gameObject.transform.TransformPoint(center + direction)));
                lowerMassPointLine.Add(new RegularPointMass(gameObject.transform.TransformPoint(center - direction)));
                leftMassPointLine.Add(new RegularPointMass(gameObject.transform.TransformPoint(center - Vector3.Cross(direction, Vector3.up))));
                rightMassPointLine.Add(new RegularPointMass(gameObject.transform.TransformPoint(center + Vector3.Cross(direction, Vector3.up))));
            }
        }

        distanceToCenter = Vector3.Magnitude(vertex2DRepresentation[xAmount - 1, yAmount - 1] - center);
    }

    

    #endregion


    #region debug
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

            for(int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].DrawConnection();
            }
        }
    }


    #endregion
}
