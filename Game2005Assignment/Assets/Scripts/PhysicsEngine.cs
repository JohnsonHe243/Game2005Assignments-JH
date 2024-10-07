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


            // position
            objectA.transform.position += objectA.velocity * dt;
            // velocity update according to gravity acceleration
            objectA.velocity += gravityAcceleration * dt;
            // Drag: TODO

            //Debug drawing
            Debug.DrawLine(prevPos, newPos, Color.green, 10);
            Debug.DrawLine(objectA.transform.position, objectA.transform.position + objectA.velocity, Color.red);
        }
    }
}
