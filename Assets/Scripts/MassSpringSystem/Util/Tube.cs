using UnityEngine;
using System.Collections;

public class Tube {

    #region variables
    //mesh itself
    private Mesh mesh;

    //vertex, normal and uv in 2d array for better understanding the relative positions
    private Vector3[,] vertex2DRepresentation;
    private Vector3[,] normal2DRepresentation;
    private Vector2[,] uv2DRepresentation;
    #endregion

    #region properties
    public Mesh Mesh
    {
       get { return mesh; }
    }

    public Vector3[,] Vertex2DRepresentation
    {
        get { return vertex2DRepresentation; }
    }

    public Vector3[,] Normal2DRepresentation
    {
        get { return normal2DRepresentation; }
    }

    public Vector2[,] UV2DRepresentation
    {
        get { return uv2DRepresentation; }
    }
    #endregion

    #region constructor
    public Tube(Mesh mesh, Vector3[,] vertex2DRepresentation, Vector3[,] normal2DRepresentation, Vector2[,] uv2DRepresentation)
    {
        this.mesh = mesh;
        this.vertex2DRepresentation = vertex2DRepresentation;
        this.normal2DRepresentation = normal2DRepresentation;
        this.uv2DRepresentation = uv2DRepresentation;
    }
    #endregion


}
