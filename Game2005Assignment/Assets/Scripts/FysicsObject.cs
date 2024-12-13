using UnityEngine;

public class FysicsObject : MonoBehaviour
{
    public FysicsShape shape;

    [Range(0.001f, 100000f)]
    public float mass = 1.0f;
    //[Range(0f, 1f)]
    //public float drag = 0.1f;
    [Range(0f, 1f)]
    public float grippiness = 0.5f; // Combining grippiness determines coefficient of friction between two objects

    public float bounciness = 0.5f; // Combining bounciness determines coefficient of restitution between two objects
    public float gravityScale = 1f;
    public Vector3 velocity = Vector3.zero;
    public Vector3 netForce = Vector3.zero;

    public bool isStatic = true;

    // Start is called before the first frame update
    void Start()
    {
        shape = GetComponent<FysicsShape>();
        FysicsEngine.Instance.objekts.Add(this);
    }
}
