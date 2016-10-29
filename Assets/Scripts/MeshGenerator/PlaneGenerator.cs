using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneGenerator : MonoBehaviour
{
    [SerializeField]
    private int xSize = 4;

    [SerializeField]
    private int ySize = 4;

    [SerializeField]
    private float vertexOffset = 0.2f;

    [SerializeField]
    private Material meshMaterial;

    [SerializeField]
    private bool debugDrawVertex = false;

    #region Internal members
    private Vector3[] vertices;
    private Vector2[] uv;
    private Vector4[] tangents;
    private Mesh mesh;
    #endregion

    public void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Procedural Plane";

        GetComponent<Renderer>().material = meshMaterial;
        
        GenerateVerticesUvTangents();

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;
        mesh.triangles = CalculateTriangles();
        mesh.RecalculateNormals();
    }

    private void GenerateVerticesUvTangents()
    {
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        uv = new Vector2[vertices.Length];
        tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        float yOffset = 0f;
        float xOffset = 0f;
        for (int i = 0, y = 0; y <= ySize; y++, yOffset += vertexOffset)
        {
            xOffset = 0f;
            for (int x = 0; x <= xSize; x++, i++, xOffset += vertexOffset)
            {
                vertices[i] = new Vector3(xOffset, yOffset);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }
    }

    private int[] CalculateTriangles()
    {
        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        return triangles;
    }

    private void OnDrawGizmos()
    {
        if (debugDrawVertex && vertices != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < vertices.Length; i++)
                Gizmos.DrawCube(vertices[i], Vector3.one * 0.05f);
        }
    }
}
