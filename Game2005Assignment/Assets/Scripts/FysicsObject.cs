using UnityEngine;

public class FysicsObject : MonoBehaviour
{
    public FysicsShape shape = null;
    public float mass = 1.0f;
    public float drag = 0.1f;
    public float gravityScale = 1;
    public Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        shape = GetComponent<FysicsShape>();
        FysicsEngine.Instance.objects.Add(this);
    }
}
