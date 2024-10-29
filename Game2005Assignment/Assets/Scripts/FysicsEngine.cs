using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class FysicsEngine : MonoBehaviour
{
    static FysicsEngine instance = null;
    public static FysicsEngine Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<FysicsEngine>();
            }
            return instance;
        }
    }
    public List<FysicsObject> objects = new List<FysicsObject> ();
    public float dt = 0.02f;
    public Vector3 gravityAcceleration = new Vector3(0, -10, 0);

    // Start is called before the first frame update
    private void KinematicUpdate()
    {
        foreach (FysicsObject objectA in objects)
        {
            Vector3 prevPos = objectA.transform.position;
            Vector3 newPos = objectA.transform.position + objectA.velocity * dt;

            // Position
            objectA.transform.position = newPos;

            // Velocity update according to gravity acceleration
            Vector3 accelerationThisFrame = gravityAcceleration * objectA.gravityScale;

            Vector3 vSquared = objectA.velocity.normalized * objectA.velocity.sqrMagnitude;

            Vector3 dragAcceleration = -objectA.drag * vSquared;

            // Add acceleration due to drag
            accelerationThisFrame += dragAcceleration;

            // Apply acceleration
            objectA.velocity += accelerationThisFrame * dt;

            // Debug drawing
            Debug.DrawLine(prevPos, newPos, Color.green, 10);
            Debug.DrawLine(objectA.transform.position, objectA.transform.position + objectA.velocity, Color.red);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (FysicsObject obj in objects)
        {
            obj.GetComponent<Renderer>().material.color = Color.white;
        }
        KinematicUpdate();
        CollisionUpdate();
    }
    
    // Check for collision between objects.
    private void CollisionUpdate()
    {
        for (int i = 0; i < objects.Count; i++) // N
        {
            FysicsObject objectA = objects[i];

            for (int j = i + 1; j < objects.Count; j++) // N
            {
                FysicsObject objectB = objects[j];

                if (objectA == objectB) continue;

                bool isOverlapping = false;

                if (objectA.shape.GetShape() == FysicsShape.Shape.Sphere &&
                    objectB.shape.GetShape() == FysicsShape.Shape.Sphere)
                {
                    isOverlapping = IsOverlappingSpheres(objectA, objectB);
                }
                else if (objectA.shape.GetShape() == FysicsShape.Shape.Sphere &&
                         objectB.shape.GetShape() == FysicsShape.Shape.Plane)
                {
                    isOverlapping = IsOverlappingSpherePlane((FysicsShapeSphere)objectA.shape, (FysicsShapePlane)objectB.shape);
                }
                else if (objectA.shape.GetShape() == FysicsShape.Shape.Plane &&
                         objectB.shape.GetShape() == FysicsShape.Shape.Sphere)
                {
                    isOverlapping = IsOverlappingSpherePlane((FysicsShapeSphere)objectB.shape, (FysicsShapePlane)objectA.shape);
                }

                if (isOverlapping)
                {
                    // Colliding
                    Debug.DrawLine(objectA.transform.position, objectB.transform.position, Color.red);

                    // Changing the color for colliding objects.
                    objectA.GetComponent<Renderer>().material.color = Color.red;
                    objectB.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
    }
    public static bool IsOverlappingSpheres(FysicsObject objectA, FysicsObject objectB)
    {
        Debug.Log("checking collision between: " + objectA.name + " and " + objectB.name);
        Vector3 d = objectA.transform.position - objectB.transform.position;
        float distance = d.magnitude;

        float radiusA = ((FysicsShapeSphere)objectA.shape).radius;
        float radiusB = ((FysicsShapeSphere)objectB.shape).radius;

        return distance < radiusA + radiusB;
    }

    public static bool IsOverlappingSpherePlane(FysicsShapeSphere sphere, FysicsShapePlane plane)
    {
        Vector3 planeTosphere = sphere.transform.position - plane.transform.position;
        float positionAlongNormal = Vector3.Dot(planeTosphere, plane.Normal());
        float distanceToPlane = Mathf.Abs(positionAlongNormal);
        return distanceToPlane < sphere.radius;
    } 
}
