using UnityEngine;

public abstract class PhysicsShape : MonoBehaviour 
{
    public enum Shape
    {
        Sphere,
        Plane,
        Halfspace
    }

    public abstract Shape GetShape();
}
