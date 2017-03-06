using UnityEngine;
using System.Collections;
using System;



public class TubeGenerator : MonoBehaviour, TubeGeneratorInterface {

    #region variables

    [Header("tube cap config")]
    [SerializeField]
    [Range(0.001f, 25.0f)]
    private float outerRadius = 0.5f;

    [SerializeField]
    [Range(3, 64)]
    private int radialSegments = 12;

    [Header("tube body config")]
    [SerializeField]
    [Range(0.01f, 250.0f)]
    private float tubeLength = 2.5f;

    [SerializeField]
    [Range(1, 256)]
    private int tubeSegments = 1;

    [Header("temp bool for generating")]
    [SerializeField]
    private bool generateNow;

    private bool doneGenerating = true;

    private Vector3[,] vertex2DArray;
    private Vector3[,] normal2DArray;
    private Vector2[,] uv2DArray;

    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uvs;
    private int[] indices;
    private BoneWeight[] boneWeights;
    private Transform[] bones;
    private Matrix4x4[] bindPoses;
    private BoxCollider[] boxColliders;
    private Rigidbody[] rigidBodies;
    #endregion

    #region methods
	// Update is called once per frame
	void Update () {
        if (generateNow)
        {
            generateNow = !generateNow;
            if (doneGenerating)
            {
                GenerateTube(outerRadius);
            }
        }
	}

    public Tube GenerateTube(float widthScale)
    {
        doneGenerating = false;
        Mesh mesh = new Mesh();

        //vertices
        vertex2DArray = GenerateCircleVertices(radialSegments, widthScale, tubeSegments, tubeLength);
        vertices = Copy2DArrayTo1D(vertex2DArray);

        //normals
        normal2DArray = CalculateNormals(vertex2DArray);
        normals = Copy2DArrayTo1D(normal2DArray);

        //uvs
        uv2DArray = CalculateUVs(vertex2DArray);
        uvs = Copy2DArrayTo1D(uv2DArray);

        //indices
        indices = CalculateIndices(vertex2DArray);

        //vertex boneweights
        boneWeights = CalculateBoneWeights(vertex2DArray);

        //bones
        bones = GenerateBones(vertex2DArray);

        //bindposes
        bindPoses = CalculateBindPoses(bones);

        //colliders
        //boxColliders = SetBoxColliders(bones);

        //rigidbodies
        //rigidBodies = SetRigidBodies(bones);

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = indices;

        mesh.RecalculateBounds();
        mesh.boneWeights = boneWeights;
        mesh.bindposes = bindPoses;

        doneGenerating = true;

        return new Tube(mesh, vertex2DArray, normal2DArray, uv2DArray, bones, boxColliders);
    }

    private Rigidbody[] SetRigidBodies(Transform[] bones)
    {
        Rigidbody[] rigidBodies = new Rigidbody[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            rigidBodies[i] = bones[i].gameObject.AddComponent<Rigidbody>();
            rigidBodies[i].useGravity = false;
            rigidBodies[i].isKinematic = false;
        }


        return rigidBodies;
    }

    private BoxCollider[] SetBoxColliders(Transform[] bones)
    {
        BoxCollider[] boxColliders = new BoxCollider[bones.Length];
        Vector3 colliderDimensions = new Vector3(2 * outerRadius, tubeLength / tubeSegments, 2 * outerRadius);

        for(int i = 0; i < bones.Length; i++)
        {
            boxColliders[i] = bones[i].gameObject.AddComponent<BoxCollider>();
            boxColliders[i].size = colliderDimensions;
        }

        return boxColliders;
    }

    private Matrix4x4[] CalculateBindPoses(Transform[] bones)
    {
        Matrix4x4[] bindPoses = new Matrix4x4[bones.Length];

        for(int i = 0; i < bones.Length; i++)
        {
            bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
        }

        return bindPoses;
    }

    private Transform[] GenerateBones(Vector3[,] vertices)
    {
        Transform[] bones = new Transform[vertices.GetLength(1)];
        Vector3 bonePosition = new Vector3();

        for (int j = 0; j < vertices.GetLength(1); j++)
        {
            bonePosition.Set(0, 0, 0);
            for (int i = 0; i < vertices.GetLength(0); i++)
            {
                bonePosition += vertices[i, j];
            }

            bonePosition /= vertices.GetLength(0);


            bones[j] = new GameObject("Bone"+j).transform;
            bones[j].parent = transform;

            bones[j].localRotation = Quaternion.identity;
            bones[j].localPosition = bonePosition;

            bones[j].gameObject.layer = LayerMask.NameToLayer("MassSpring");
        }

        return bones;
    }

    private BoneWeight[] CalculateBoneWeights(Vector3[,] vertices)
    {
        BoneWeight[] weights = new BoneWeight[vertices.GetLength(0)*vertices.GetLength(1)];

        for(int j = 0; j < vertices.GetLength(1); j++)
        {
            for(int i = 0; i < vertices.GetLength(0); i++)
            {
                weights[j * vertices.GetLength(0) + i].boneIndex0 = j;
                weights[j * vertices.GetLength(0) + i].weight0 = 1;
            }
        }
       
        return weights;
    }

    private Vector3[,] GenerateCircleVertices(int radialSegments, float outerRadius, int tubeSegments, float tubeLength)
    {
        Vector3 currentVertex = new Vector3();
        Vector3[,] vertices = new Vector3[radialSegments + 1, 1 + tubeSegments];
        float PI2 = 2 * Mathf.PI;
        float fracturedLength = tubeLength / (float)tubeSegments;

        for (int i = 0; i <= radialSegments; i++)
        {
            float r1 = (float)(i % radialSegments) / radialSegments * PI2;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            currentVertex = new Vector3(cos * outerRadius, sin * outerRadius, 0);
            for (int j = 0; j < vertices.GetLength(1); j++)
            {
                currentVertex.z = j * fracturedLength;
                vertices[i, j] = currentVertex;
            }
        }
        return vertices;
    }

    private Vector3[,] CalculateNormals(Vector3[,] vertices)
    {
        Vector3 currentNormal = new Vector3();
        Vector3[,] normals = new Vector3[vertices.GetLength(0), vertices.GetLength(1)];

        float PI2 = 2 * Mathf.PI;
        int radialSegments = vertices.GetLength(0)-1;
        for (int i = 0; i < vertices.GetLength(0); i++)
        {
            float r1 = (float)(i) / radialSegments * PI2;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            currentNormal.Set(cos, 0, sin);

            for (int j = 0; j < vertices.GetLength(1); j++)
            {
                normals[i, j] = currentNormal;
            }
        }
        return normals;
    }


    private Vector2[,] CalculateUVs(Vector3[,] vertices)
    {
        Vector2 currentUV = new Vector2();
        Vector2[,] uvs = new Vector2[vertices.GetLength(0), vertices.GetLength(1)];

        float xStep = 1.0f / vertices.GetLength(0)-1;
        float yStep = 1.0f / vertices.GetLength(1)-1;

        for (int i = 0; i < vertices.GetLength(0); i++)
        {
            for(int j = 0; j < vertices.GetLength(1); j++)
            {
                currentUV.Set(xStep*i, yStep*j);
                uvs[i, j] = currentUV;
            }
        }
        return uvs;
    }

    private int[] CalculateIndices(Vector3[,] vertices)
    {
        //x vertices -1 * y vertices -1 * 2 (1 quad == two triangles) * 3 int values for each triangle
        int[] indices = new int[(vertices.GetLength(0)) * (vertices.GetLength(1)-1) *2 *3];

        int index = 0;
        int iLength = vertices.GetLength(0);
        int jLength = vertices.GetLength(1);
        for (int i = 0; i < iLength; i++)
        {
            for (int j = 0; j < jLength; j++)
            {

                if (j < jLength - 1)
                {
                    if (i < iLength - 1)
                    {
                        indices[index++] = i + iLength * j;
                        indices[index++] = i + iLength * (j + 1);
                        indices[index++] = i + 1 + iLength * (j + 1);

                        indices[index++] = i + iLength * j;
                        indices[index++] = i + 1 + iLength * (j + 1);
                        indices[index++] = i + 1 + iLength * j;
                    }
                    else
                    {
                        indices[index++] = i + iLength * j;
                        indices[index++] = i + iLength * (j + 1);
                        indices[index++] = 0 + iLength * (j + 1);

                        indices[index++] = i + iLength * j;
                        indices[index++] = 0 + iLength * (j + 1);
                        indices[index++] = 0 + iLength * j;
                    }
                }
            }
        }
        return indices;
    }

    private T[] Copy2DArrayTo1D<T>(T[,] array2D)
    {
        T[] array1D = new T[array2D.GetLength(0) * array2D.GetLength(1)];

        for (int i = 0; i < array2D.GetLength(0); i++)
        {
            for (int j = 0; j < array2D.GetLength(1); j++)
            {
                array1D[i + array2D.GetLength(0) * j] = array2D[i, j];
            }
        }
        return array1D;
    }

    public Tube GenerateTube()
    {
        return GenerateTube(outerRadius);
    }

    #endregion
}
