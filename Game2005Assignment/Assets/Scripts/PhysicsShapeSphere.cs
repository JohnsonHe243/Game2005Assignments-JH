using UnityEngine;

public class PhysicsShapeSphere : PhysicsShape
{
    public float radius = 1.0f;

    public override Shape GetShape()
    {
        return Shape.Sphere;
    }

    public void UpdateScale()
    {
        transform.localScale = 
            new Vector3(radius, radius, radius) * 2f;
    }

    public void OnValidate()
    {
        UpdateScale();
    }

    private void Update()
    {
        UpdateScale();
    }
}
