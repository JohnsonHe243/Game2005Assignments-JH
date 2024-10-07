using UnityEngine;

public class AngryBird : MonoBehaviour
{
    public float angleDegrees = 60f;
    public float speed = 15f;
    public float startHeight = 1;

    public GameObject projectileToCopy; // This can be a reference in the scene, or to a Prefab.

    void Update()
    {


        Vector3 launchVelocity = new Vector3(speed * Mathf.Cos(angleDegrees * Mathf.Deg2Rad),
                                            speed * Mathf.Sin(angleDegrees * Mathf.Deg2Rad));
        Vector3 startPosition = new Vector3(0, startHeight, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Launch!");
            GameObject newObject = Instantiate(projectileToCopy);
            PhysicsObject physicsObject = newObject.GetComponent<PhysicsObject>();


            physicsObject.velocity = launchVelocity;

            physicsObject.transform.position = startPosition;
        }
        Debug.DrawLine(startPosition, startPosition + launchVelocity, Color.red);
    }
}
