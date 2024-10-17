using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsShapePlane : PhysicsShape
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

    