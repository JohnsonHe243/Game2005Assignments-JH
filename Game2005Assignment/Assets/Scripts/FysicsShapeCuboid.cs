
using UnityEngine;

public class FysicsShapeCuboid : FysicsShape
{
    public float width = 1.0f;
    public float height = 1.0f;
    // Start is called before the first frame update
    public override Shape GetShape()
    {
        return Shape.Cuboid;
    }

    public void UpdateScale()
    {
        transform.localScale = 
            new Vector3(width, height, 1);
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
