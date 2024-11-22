using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using Unity.VisualScripting;
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
                    isOverlapping = CollideSpheres((FysicsShapeSphere)objectA.shape, (FysicsShapeSphere)objectB.shape);
                }
                else if (objectA.shape.GetShape() == FysicsShape.Shape.Sphere &&
                         objectB.shape.GetShape() == FysicsShape.Shape.Plane)
                {
                    isOverlapping = IsCollidingSpherePlane((FysicsShapeSphere)objectA.shape, (FysicsShapePlane)objectB.shape);
                }
                else if (objectA.shape.GetShape() == FysicsShape.Shape.Sphere &&
                         objectB.shape.GetShape() == FysicsShape.Shape.Plane)
                {
                    isOverlapping = IsCollidingSpherePlane((FysicsShapeSphere)objectA.shape, (FysicsShapePlane)objectB.shape);
                }
                else if (objectA.shape.GetShape() == FysicsShape.Shape.Cuboid &&
                         objectB.shape.GetShape() == FysicsShape.Shape.Cuboid)
                {
                    isOverlapping = CollideCuboids((FysicsShapeCuboid)objectA.shape, (FysicsShapeCuboid)objectB.shape);
                }
                else if (objectA.shape.GetShape() == FysicsShape.Shape.Sphere &&
                         objectB.shape.GetShape() == FysicsShape.Shape.Cuboid)
                {
                    isOverlapping = CollideSphereCuboid((FysicsShapeSphere)objectA.shape, (FysicsShapeCuboid)objectB.shape);
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

    public static bool CollideSpheres(FysicsShapeSphere sphereA, FysicsShapeSphere sphereB)
    {
        Vector3 Displacement= sphereA.transform.position - sphereB.transform.position;
        float distance = Displacement.magnitude;
        float overlap = (sphereA.radius + sphereB.radius) - distance;
        if (overlap > 0.0f)
        {
            Vector3 collisionNormalBtoA = (Displacement / distance);
            Vector3 minTranslationV = collisionNormalBtoA * overlap;
            sphereA.transform.position += minTranslationV/2;
            sphereB.transform.position -= minTranslationV/2;
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool CollideCuboids(FysicsShapeCuboid cuboidA, FysicsShapeCuboid cuboidB)
    {
        Vector3 Displacement = cuboidA.transform.position - cuboidB.transform.position;
        float distance = Displacement.magnitude;
        float overlapX = (cuboidA.width / 2 + cuboidB.width / 2) - distance;
        float overlapY = (cuboidA.width / 2 + cuboidB.height / 2) - distance;
        float overlap;

        if (overlapX > 0.0f && overlapY > 0.0f)
        {
            // Ensure minimum overlap is used (for 2D; expand for 3D if needed).
            if (overlapX > overlapY)
            {
                overlap = overlapX;
            }
            else
            {
                overlap = overlapY;
            }

            Vector3 collisionNormalBtoA = (Displacement / distance);
            Vector3 minTranslationV = collisionNormalBtoA * overlap;
            cuboidA.transform.position += minTranslationV / 2;
            cuboidB.transform.position -= minTranslationV / 2;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CollideSphereCuboid(FysicsShapeSphere sphere, FysicsShapeCuboid cuboid)
    {
        Vector3 Displacement = cuboid.transform.position - sphere.transform.position;
        float distance = Displacement.magnitude;
        float overlapX = (sphere.radius + cuboid.width / 2) - distance;
        float overlapY = (sphere.radius + cuboid.height / 2) - distance;

        if (overlapX > 0.0f && overlapY > 0.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsCollidingSpherePlane(FysicsShapeSphere sphere, FysicsShapePlane plane)
    {
        Vector3 planeToSphere = sphere.transform.position - plane.transform.position;
        float positionAlongNormal = Vector3.Dot(planeToSphere, plane.Normal());
        float distanceToPlane = Mathf.Abs(positionAlongNormal);
        float overlap = sphere.radius - distanceToPlane;
        if (overlap > 0.0f)
        {
            Vector3 minTranslationV = plane.Normal() * overlap;
            sphere.transform.position += minTranslationV;
            return true;
        }
        else
        {
            return false;
        }
    }

    //public static bool IsOverlappingSpheres(FysicsShapeSphere sphereA, FysicsShapeSphere sphereB)
    //{
    //    Vector3 Displacement = sphereA.transform.position - sphereB.transform.position;
    //    float distance = Displacement.magnitude;
    //    return distance < sphereA.radius + sphereB.radius;
    //}
    //public static bool IsOverlappingSpherePlane(FysicsShapeSphere sphere, FysicsShapePlane plane)
    //{
    //    Vector3 planeToSphere = sphere.transform.position - plane.transform.position;
    //    float positionAlongNormal = Vector3.Dot(planeToSphere, plane.Normal());
    //    float distanceToPlane = Mathf.Abs(positionAlongNormal);
    //    return distanceToPlane < sphere.radius;
    //} 
}
