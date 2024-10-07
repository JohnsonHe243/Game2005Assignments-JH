using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float mass = 1.0f;
    public float drag = 0.1f;
    public Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        PhysicsEngine.Instance.objects.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        // PhysicsEngine.Instance.gravityAcceleration;
    }
}
