using UnityEngine;

public class FysicsShapeSphere : FysicsShape
{
    public enum ColorType
    {
        White,
        Red,
        Green,
        Blue,
        Yellow
    }
    public ColorType sphereColor = ColorType.White;

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

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = GetColorType(sphereColor);
        }
    }
    public void OnValidate()
    {
        UpdateScale();
    }

    private void Update()
    {
        UpdateScale();
    }

    private Color GetColorType(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.Red:
                return Color.red;
            case ColorType.Green:
                return Color.green;
            case ColorType.Blue:
                return Color.blue;
            case ColorType.Yellow:
                return Color.yellow;
            case ColorType.White:
            default:
                return Color.white;
        }
    }
}
