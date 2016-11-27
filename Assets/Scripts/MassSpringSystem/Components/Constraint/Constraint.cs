using UnityEngine;
using System.Collections;

public abstract class Constraint {

    #region variables
    protected PointMass pointA;
    protected PointMass pointB;
    protected float restingDistance;
    protected float springFactor;
    #endregion

    #region methods
    public Constraint(PointMass pointA, PointMass pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        this.restingDistance = Vector3.Distance(pointA.Position, pointB.Position);
    }

    public abstract void Solve();

    public void DrawConnection()
    {
        Gizmos.DrawLine(pointA.Position, pointB.Position);
    }
    #endregion
}
