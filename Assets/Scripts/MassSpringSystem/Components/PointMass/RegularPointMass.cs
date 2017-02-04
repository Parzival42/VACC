using UnityEngine;

public class RegularPointMass : PointMass {

    #region methods
    public RegularPointMass(Vector3 currentPosition) : base(currentPosition)
    {
    }

    public override void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }

    public override void CorrectPosition(Vector3 offset)
    {
        currentPosition += offset;
    }

    public override void Simulate(float deltaTime)
    {
        swapVector = currentPosition;

        currentPosition = currentPosition + (currentPosition - previousPosition) * 0.96f + acceleration * 0.5f * deltaTime * deltaTime;

        previousPosition = swapVector;

        acceleration = Vector3.zero;
    }
    #endregion

}
