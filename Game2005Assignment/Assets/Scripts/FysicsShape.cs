using UnityEngine;

public abstract class FysicsShape : MonoBehaviour 
{
    public enum Shape
    {
        Sphere,
        Cuboid,
        Plane,
        Halfspace
    }

    public abstract Shape GetShape();
}
