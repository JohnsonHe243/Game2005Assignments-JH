using UnityEngine;

public class AngryBird : MonoBehaviour
{
    public float angleDegrees = 30f;
    public float speed = 1f;
    public float startHeight = 1;

    public float angleRadians;
    public float x;
    public float y;

    public float deltaTime = 0.02f;

    public Vector3 velocity;
    public Vector3 gravityAcceleration = new Vector3(0, -10, 0);
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("Launch!");

        //    //velocity = new Vector3(10, 16);


        //    x = (Mathf.Cos(angleDegrees) * speed);
        //    //y = (float)(speed * Mathf.Sin(angleDegrees));
        //    y = Mathf.Sqrt(Mathf.Pow(speed, 2) - Mathf.Pow(x, 2));
        //    velocity = new Vector3(x, y);

        //    transform.position = new Vector3(0, startHeight, 0);
        //    Debug.DrawLine(transform.position, transform.position + velocity, Color.red, 2);
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Launch!");

            angleRadians = angleDegrees * Mathf.Deg2Rad;
            x = speed * Mathf.Cos(angleRadiant);
            y = speed * Mathf.Sin(angleRadiant);

            velocity = new Vector3(x, y);

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

        Debug.DrawLine(transform.position, transform.position + velocity, Color.red);
    }
}
