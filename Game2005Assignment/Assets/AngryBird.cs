using UnityEngine;

public class AngryBird : MonoBehaviour
{
    public float angleDegrees = 30;
    public float speed = 100;
    public float startHeight = 1;

    public float deltaTime = 0.02f;

    public Vector3 velocity;
    public Vector3 gravityAcceleration = new Vector3(0, -10, 0);
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Launch!");
            velocity = new Vector3(10, 16);
            transform.position = new Vector3(0, startHeight, 0);
            Debug.DrawLine(transform.position, transform.position + velocity, Color.red, 2);
        }
    }

    private void FixedUpdate()
    {
        Vector3 prevPos = transform.position;

        transform.position = transform.position + velocity * deltaTime;

        Debug.DrawLine(prevPos, transform.position, Color.green, 10);

        velocity = velocity + gravityAcceleration * deltaTime;
    }
}
