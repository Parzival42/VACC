using UnityEngine;

/// <summary>
/// the pointmass class
/// </summary>
public abstract class PointMass{

    #region variables
    protected Vector3 currentPosition;
    protected Vector3 previousPosition = new Vector3();
    protected Vector3 acceleration = new Vector3();
    protected Vector3 swapVector = new Vector3();
    #endregion

    #region properties
    public Vector3 Position
    {
        get { return currentPosition; }
        set { currentPosition = value; }
    }

    public Vector3 PreviousPosition
    {
        get { return previousPosition; }
    }
    #endregion

    #region methods

    protected PointMass(Vector3 currentPosition)
    {
        this.currentPosition = currentPosition;
        previousPosition = currentPosition;
    }

    public abstract void ApplyForce(Vector3 force);

    public abstract void CorrectPosition(Vector3 offset);

    public abstract void Simulate(float deltaTime);

    #region draw debug

    public void DrawPoint()
    {
        Gizmos.DrawCube(currentPosition, new Vector3(0.05f, 0.05f, 0.05f));
    }

    #endregion
    #endregion
}
