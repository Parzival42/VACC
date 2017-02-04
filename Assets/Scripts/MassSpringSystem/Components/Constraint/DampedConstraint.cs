using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DampedConstraint : Constraint {

    private Vector3 direction;
    private Vector3 offset;
    private float distance;
    private float factor;
    private float dampFactor;
    private float tolerance = 0.0f;

    public DampedConstraint(PointMass pointA, PointMass pointB, float dampFactor) : base(pointA, pointB)
    {
        this.dampFactor = dampFactor;
        //tolerance = distance * 1.5f;
    }

    public override void Solve()
    {
        direction = pointB.Position - pointA.Position;
        distance = Vector3.Magnitude(direction);

        if(distance > restingDistance * (1 - tolerance))
        {
            factor = (distance - restingDistance) / (distance * springFactor);
            offset = direction * factor;

            pointA.CorrectPosition(offset);
            pointB.CorrectPosition(-offset);
        }
    }
}
