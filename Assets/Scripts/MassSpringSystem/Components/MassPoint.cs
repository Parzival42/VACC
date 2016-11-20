using UnityEngine;
using System.Collections.Generic;

public class MassPoint {
    private List<Spring> attachedSprings;

    private Vector3 lastPosition;
    private Vector3 currentPosition;
    private Vector3 acceleration;
    private Vector3 gravitationalForce;
    private Vector3 swapVector;
    private Vector3 fixPoint;

    private float inverseMass;
    private bool fixedPoint;

    public Vector3 Position
    {
        get { return currentPosition; }
        set { currentPosition = value; }
    }

    public float InverseMass
    {
        get { return inverseMass; }
    }

    public MassPoint(Vector3 currentPosition, float mass, bool fixedPoint) {
        this.currentPosition = currentPosition;
        this.fixedPoint = fixedPoint;
        fixPoint = currentPosition;
        lastPosition = currentPosition;
        inverseMass = 1.0f/mass;
        gravitationalForce = new Vector3(0, -9.81f/1.0f, 0);
        swapVector = new Vector3(0, 0, 0);
        attachedSprings = new List<Spring>();
    }

    public void Update(float dt)
    {
       
        //gravitation
       // ApplyForce(gravitationalForce);

        //position update using (no velocity) verlet integration 
        swapVector = currentPosition;

        currentPosition = currentPosition + (currentPosition - lastPosition) * 0.99f + acceleration * 0.5f * dt * dt;

        lastPosition = swapVector;

        acceleration = Vector3.zero;
        
        if(fixedPoint)
        {
            currentPosition = fixPoint;
        }

    }


    public void ApplyForce(Vector3 force)
    {
        acceleration += force * inverseMass;
    }

    public void PinToPoint(Vector3 fixPoint)
    {
        fixedPoint = true;
        this.fixPoint = fixPoint;
        //currentPosition = fixPoint;
    }

    public void ConnectTo(MassPoint massPoint)
    {
        attachedSprings.Add(new Spring(this, massPoint, 0.75f));
    }

    public void RemoveSpring(Spring spring)
    {
        attachedSprings.Remove(spring);
    }

    public void UpdateConnections()
    {
        for(int i = 0; i < attachedSprings.Count; i++)
        {
            attachedSprings[i].draw();
            attachedSprings[i].Update();
        }
    }
     
    public void KillAcceleration()
    {
        acceleration *= -1;
    }
}
