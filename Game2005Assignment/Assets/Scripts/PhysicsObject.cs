using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float radius = 1;
    public float mass = 1.0f;
    public float drag = 0.1f;
    public Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        PhysicsEngine.Instance.objects.Add(this);
    }

    private void OnValidate() // only workds in editor, not in build.
    {
        transform.localScale = new Vector3(radius, radius, radius) * 2f;
    }

    private void Update() // only workds in editor, not in build.
    {
        transform.localScale = new Vector3(radius, radius, radius) * 2f;
    }
}
