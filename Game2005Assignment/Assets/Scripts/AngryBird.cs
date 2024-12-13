using System;
using System.Collections.Generic;
using UnityEngine;

public class AngryBird : MonoBehaviour
{
    public float angleDegrees = 60f;
    public float speed = 15f;
    // public float startHeight = 1;

    public GameObject plane;
    public GameObject projectile_1; // This can be a reference in the scene, or to a Prefab.
    public GameObject projectile_2;

    void Update()
    {
        Vector3 launchVelocity = new Vector3(speed * Mathf.Cos(angleDegrees * Mathf.Deg2Rad),
                                             speed * Mathf.Sin(angleDegrees * Mathf.Deg2Rad));

        Vector3 startPosition = transform.position; //new Vector3(0, startHeight, 0);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("Launch!");
        //    GameObject newObject = Instantiate(projectile_1);
        //    FysicsObject fysicsObject = newObject.GetComponent<FysicsObject>();


        //    fysicsObject.velocity = launchVelocity;

        //    fysicsObject.transform.position = startPosition;
        //}
        Debug.DrawLine(startPosition, startPosition + launchVelocity, Color.red);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject FlatPlane = Instantiate(plane);
            GameObject Ball = Instantiate(projectile_1);
            FysicsObject BoucingBall = Ball.GetComponent<FysicsObject>();
            BoucingBall.velocity = Vector3.zero;
            BoucingBall.transform.position = new Vector3(0, 10, 0);

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject FlatPlane = Instantiate(plane);
            GameObject Ball_1 = Instantiate(projectile_1);
            GameObject Ball_2 = Instantiate(projectile_1);

            FysicsObject MovingBall = Ball_1.GetComponent<FysicsObject>();
            FysicsObject StationaryBall = Ball_2.GetComponent<FysicsObject>();
            MovingBall.velocity = new Vector3(30, 0, 0);
            MovingBall.transform.position = new Vector3(-15, -4, 0);
            StationaryBall.transform.position = new Vector3(15, -4, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (FysicsObject objekt in FysicsEngine.Instance.objekts)
            {
                Destroy(objekt.gameObject);
                FysicsEngine.Instance.objekts = new List<FysicsObject>();
            }
        }
    }
}
