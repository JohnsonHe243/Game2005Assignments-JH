
using UnityEngine;

public class FysicsShapeRect : FysicsShape
{ 
    public float width = 1.0f;
    public float height = 1.0f;
    public override Shape GetShape()
    {
        return Shape.Rect;
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
