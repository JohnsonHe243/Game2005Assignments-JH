using UnityEngine;

public class Motion : MonoBehaviour
{
    public float X,Y = 0;
    public float a = 1;
    public float b = 3;
    public float t = 0;

    void FixedUpdate()
    {
        float dt = 0.98f / 60.0f;

        X = X + (-Mathf.Sin(t * a) * a * b * dt);
        Y = Y + (-Mathf.Cos(t * a) * a * b * dt);

        transform.position = new Vector3(X, Y, transform.position.z);

        t += dt;

    }
}
