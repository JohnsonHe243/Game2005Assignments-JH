using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    static PhysicsEngine instance = null;
    public static PhysicsEngine Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PhysicsEngine>();
            }
            return instance;
        }
    }
    public List<PhysicsObject> objects = new List<PhysicsObject> ();
    public float dt = 0.02f;
    public Vector3 gravityAcceleration = new Vector3(0, -10, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(PhysicsObject objectA in objects)
        {
            Vector3 prevPos = objectA.transform.position;
            Vector3 newPos = objectA.transform.position + objectA.velocity * dt;
            //position
            objectA.transform.position= newPos;
            //velocity update according to gravity acceleration
            Vector3 accelerationThisframe = gravityAcceleration;
            Vector3 vSquared = objectA.velocity.normalized * objectA.velocity.sqrMagnitude;
            Vector3 dragAcceleration = -objectA.drag * vSquared; // -c * v^2
            //Add acceleration due to drag
            accelerationThisframe += dragAcceleration;
            //Apply acceleration
            objectA.velocity += accelerationThisframe * dt;
            //Debug drawing
            Debug.DrawLine(prevPos, newPos, Color.green, 10);
            Debug.DrawLine(objectA.transform.position, objectA.transform.position + objectA.velocity, Color.red);
        }
        foreach(PhysicsObject obj in objects)
        {
            obj.GetComponent<Renderer>().material.color = Color.white;
        }
        //Collision check loop
        for (int i = 0; i < objects.Count; i++) // N
        {
            PhysicsObject objectA = objects[i];
            for (int j = i  + 1; j < objects.Count; j++) // N
            {
                PhysicsObject objectB = objects[j];
                if (objectA == objectB) continue;
                //Check for collisions between objects
                if (IsOverlappingSpheres(objectA, objectB))
                {
                    //colliding
                    Debug.DrawLine(objectA.transform.position, objectB.transform.position, Color.red);
                    //objectA.velocity = -objectA.velocity;
                    //objectB.velocity = -objectB.velocity;
                    //color change
                    objectA.GetComponent<Renderer>().material.color= Color.red;
                    objectB.GetComponent<Renderer>().material.color = Color.red;
                }
                else
                {
                    //no collisions
                }
            }
        }
    }
    public bool IsOverlappingSpheres(PhysicsObject objectA, PhysicsObject objectB)
    {
        Debug.Log("checking collision between: " + objectA.name + " and " + objectB.name);
        Vector3 d = objectA.transform.position - objectB.transform.position;
        float distance = d.magnitude;
        return distance < objectA.radius + objectB.radius;
    }
}
