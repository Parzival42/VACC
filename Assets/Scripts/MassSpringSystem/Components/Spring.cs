using UnityEngine;

public class Spring {

    private MassPoint massPoint1;
    private MassPoint massPoint2;

    private float squaredRestingLength;
    private float restingLength;
    private float springConstant;
    private float dampingValue;
    private float tearTolerance;

    private Vector3 direction;

    public Spring(MassPoint massPoint1, MassPoint massPoint2, float springConstant)
    {
        this.massPoint1 = massPoint1;
        this.massPoint2 = massPoint2;
        this.springConstant = springConstant;
    
        
        restingLength = Vector3.Magnitude(massPoint1.Position - massPoint2.Position);
        tearTolerance = restingLength * 2.59f;
        squaredRestingLength = Vector3.SqrMagnitude(massPoint2.Position - massPoint1.Position);
        direction = new Vector3();
    }

    public void Update()
    {
        direction = (massPoint1.Position - massPoint2.Position);
        float distance =( restingLength - Vector3.Magnitude(direction));
        direction.Normalize();
         float difference = (restingLength - distance) / distance;
        //float difference = Mathf.Abs(restingLength)


        //if(Vector3.Magnitude(direction) > tearTolerance)
        //{
        //    massPoint1.RemoveSpring(this);
        //}

        Vector3 force = new Vector3();
        force = direction * distance * springConstant;

        massPoint1.ApplyForce(force*300);
        massPoint2.ApplyForce(-force*300);

        //float influence1 = massPoint1.InverseMass / (massPoint1.InverseMass + massPoint2.InverseMass) * springConstant;
        //float influence2 = springConstant - influence1;


        //massPoint1.ApplyForce(direction * distance * springConstant);
        //massPoint2.ApplyForce(-direction * distance * springConstant);

        //massPoint1.Position += direction * difference * influence1;
        //massPoint2.Position -= direction * difference * influence2;
    }




    public void draw()
    {
        Debug.DrawLine(massPoint1.Position, massPoint2.Position);
    }


}
