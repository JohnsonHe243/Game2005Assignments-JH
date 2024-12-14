using UnityEngine;

public class AngryBird : MonoBehaviour
{
    private GameObject ball; // Cached reference to the ball
    private GameObject square;
    private FysicsObject fysicsObject;

    void Start()
    {
        // Find and cache the ball object
        ball = GameObject.FindWithTag("Sphere");
        square = GameObject.FindWithTag("Rect");


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            ball.transform.position = new Vector3(-16, -4, 0);
        }

        if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            // Convert mouse position to world coordinates
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 25));
            mouseWorldPosition.z = 0; // Ensure it's in 2D space

            // Calculate angle in radians
            Vector3 direction = mouseWorldPosition - ball.transform.position;
            float angleDegrees = Mathf.Atan2(direction.y, direction.x);

            // Calculate speed based on displacement
            float speed = direction.magnitude * 5;

            // Calculate launch velocity
            Vector3 launchVelocity = new Vector3(
                speed * Mathf.Cos(angleDegrees),
                speed * Mathf.Sin(angleDegrees)
            );

            FysicsObject sphereBird = ball.GetComponent<FysicsObject>();
            // Apply velocity to the FysicsObject
            sphereBird.gravityScale = 1;
            sphereBird.velocity = -launchVelocity;
        }

        if (Input.GetMouseButtonDown(1)) // Left mouse button pressed
        {
            square.transform.position = new Vector3(-16, -4, 0);
        }

        if (Input.GetMouseButtonUp(1)) // Left mouse button released
        {
            // Convert mouse position to world coordinates
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 25));
            mouseWorldPosition.z = 0; // Ensure it's in 2D space

            // Calculate angle in radians
            Vector3 direction = mouseWorldPosition - square.transform.position;
            float angleDegrees = Mathf.Atan2(direction.y, direction.x);

            // Calculate speed based on displacement
            float speed = direction.magnitude * 5;

            // Calculate launch velocity
            Vector3 launchVelocity = new Vector3(
                speed * Mathf.Cos(angleDegrees),
                speed * Mathf.Sin(angleDegrees)
            );

            FysicsObject sphereBird = square.GetComponent<FysicsObject>();
            // Apply velocity to the FysicsObject
            sphereBird.gravityScale = 1;
            sphereBird.velocity = -launchVelocity;
        }
    }
}
