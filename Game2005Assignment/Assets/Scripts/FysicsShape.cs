using UnityEngine;

public abstract class FysicsShape : MonoBehaviour 
{
    public enum Shape
    {
        Sphere,
        Rect,
        Plane,
        Halfspace
    }

    public abstract Shape GetShape();
}
