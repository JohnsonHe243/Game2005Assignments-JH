using System;
using System.Collections.Generic;
using UnityEngine;

public class AngryBird : MonoBehaviour
{
    public float angleDegrees = 60f;
    public float speed = 15f;
    // public float startHeight = 1;

    public GameObject plane;
    public GameObject sphere; // This can be a reference in the scene, or to a Prefab.

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
            GameObject Ball = Instantiate(sphere);

            FysicsObject SteelBall = Ball.GetComponent<FysicsObject>();
            SteelBall.transform.position = new Vector3(-15, 10, 0);
            SteelBall.velocity = new Vector3(5, 0, 0);
            SteelBall.material = FysicsObject.Material.Steel;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject FlatPlane = Instantiate(plane);
            GameObject Ball = Instantiate(sphere);

            FysicsObject WoodBall = Ball.GetComponent<FysicsObject>();
            WoodBall.transform.position = new Vector3(-15, 10, 0);
            WoodBall.velocity = new Vector3(5, 0, 0);
            WoodBall.material = FysicsObject.Material.Wood;

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameObject FlatPlane = Instantiate(plane);
            GameObject Ball = Instantiate(sphere);

            FysicsObject ClothBall = Ball.GetComponent<FysicsObject>();
            ClothBall.transform.position = new Vector3(-15, 10, 0);
            ClothBall.velocity = new Vector3(5, 0, 0);
            ClothBall.material = FysicsObject.Material.Cloth;

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
