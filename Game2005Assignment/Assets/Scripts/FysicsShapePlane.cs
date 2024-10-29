using UnityEngine;

public class FysicsShapePlane : FysicsShape
{
    
    public enum Type
    {
        Plane, 
        Halfspace
    }

    public Type planeType = Type.Plane;

    public override Shape GetShape()
    {
        return Shape.Plane;
    }

    public Vector3 GetPosition()
    {
    return transform.position;
    }

    public Vector3 Normal()
    {
        return transform.up;
    }
    
}

    