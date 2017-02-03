using UnityEngine;
using System.Collections;
using System;

public class RegularConstraint : Constraint
{
    private Vector3 direction;
    private Vector3 offset;
    private float distance;
    private float factor;

    public RegularConstraint(PointMass pointA, PointMass pointB) : base(pointA, pointB)
    {
    }

    public override void Solve()
    {
        direction = pointB.Position - pointA.Position;
        distance = Vector3.Magnitude(direction);
        
        factor = (distance - restingDistance) / (distance*springFactor);
        offset = direction * factor;

        pointA.CorrectPosition(offset);
        pointB.CorrectPosition(-offset);
    }
}
