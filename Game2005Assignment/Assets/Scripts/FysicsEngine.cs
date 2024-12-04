using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class FysicsEngine : MonoBehaviour
{
    public struct CollisionInfo
    {
        public bool didcollide;
        public Vector3 normal;

        public CollisionInfo(bool didcollide, Vector3 normal)
        {
            this.didcollide = didcollide;
            this.normal = normal;
        }
    }
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
    public List<FysicsObject> objekts = new List<FysicsObject> ();
    public float dt = 0.02f;
    public Vector3 gravityAcceleration = new Vector3(0, -10, 0);

    public Vector3 GetGravityForce(FysicsObject objekt)
    {
        return gravityAcceleration * objekt.gravityScale * objekt.mass;
    }

    // Start is called before the first frame update
    private void KinematicUpdate()
    {
        foreach (FysicsObject objekt in objekts)
        {
            Vector3 Fg = GetGravityForce(objekt);
            objekt.netForce += Fg;

            // Position
            if (!objekt.isStatic) // boolean used to allow objects to move by kinematic or not
            {
                //Vector3 vSquared = objekt.velocity.normalized * objekt.velocity.sqrMagnitude;
                //Vector3 dragForce = -objekt.drag * vSquared;

                //// Add drag
                //objekt.netForce += dragForce;

                // Velocity update according to gravity acceleration
                Vector3 accelerationThisFrame = objekt.netForce / objekt.mass;
                //a = F/m

                // Apply acceleration
                objekt.velocity += accelerationThisFrame * dt;
                Vector3 newPos = objekt.transform.position + objekt.velocity * dt;
                objekt.transform.position = newPos;

                // Debug drawing
                // Velocity
                Debug.DrawRay(objekt.transform.position, objekt.velocity, Color.red);
                // Force of gravity
                Debug.DrawRay(objekt.transform.position, Fg, new Color(0.5f, 0.0f, 0.4f));
            }
            objekt.netForce = Vector3.zero;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (FysicsObject objekt in objekts)
        {
            objekt.GetComponent<Renderer>().material.color = Color.white;
        }
        CollisionUpdate();
        KinematicUpdate();
    }
    
    // Check for collision between objects.
    private void CollisionUpdate()
    {
        for (int i = 0; i < objekts.Count; i++) // N
        {
            FysicsObject objektA = objekts[i];

            for (int j = i + 1; j < objekts.Count; j++) // N
            {
                FysicsObject objektB = objekts[j];

                CollisionInfo collisionInfo = new CollisionInfo(false, Vector3.zero);

                if (objektA.shape.GetShape() == FysicsShape.Shape.Sphere &&
                    objektB.shape.GetShape() == FysicsShape.Shape.Sphere)
                {
                    collisionInfo = CollisionResponseSphereSphere((FysicsShapeSphere)objektA.shape, (FysicsShapeSphere)objektB.shape);
                }
                else if (objektA.shape.GetShape() == FysicsShape.Shape.Sphere &&
                         objektB.shape.GetShape() == FysicsShape.Shape.Plane)
                {
                    collisionInfo = CollisionResponseSpherePlane((FysicsShapeSphere)objektA.shape, (FysicsShapePlane)objektB.shape);
                }
                else if (objektA.shape.GetShape() == FysicsShape.Shape.Plane &&
                         objektB.shape.GetShape() == FysicsShape.Shape.Sphere)
                {
                    collisionInfo = CollisionResponseSpherePlane((FysicsShapeSphere)objektB.shape, (FysicsShapePlane)objektA.shape);
                }

                if (collisionInfo.didcollide)
                {
                    // Colliding
                    // Changing the color for colliding objects.
                    objektA.GetComponent<Renderer>().material.color = Color.red;
                    objektB.GetComponent<Renderer>().material.color = Color.red;

                    //  Calculate gravity force
                    Vector3 Fg = GetGravityForce(objektA);

                    // Calculate perpendicular component of gravity by project gravity vector onto normal
                    float gravityDotNormal = Vector3.Dot(Fg, collisionInfo.normal);
                    Vector3 gravityProjectedNormal = collisionInfo.normal * gravityDotNormal;

                    // Add normal force due to gravity
                    Vector3 Fn = -gravityProjectedNormal;
                    Debug.DrawRay(objektA.transform.position, Fn, Color.green);

                    objektA.netForce += Fn;
                    objektB.netForce -= Fn;

                    // Calculate relative velocity determine if we should apply kinetic friction or not
                    Vector3 veloARelativeToB = objektA.velocity - objektB.velocity;
                    // Project relative velocity onto the surface (e.g. subtract perpendicular component to a plane)
                    float veloDotNormal = Vector3.Dot(veloARelativeToB, collisionInfo.normal);
                    Vector3 veloProjectedNormal = collisionInfo.normal * veloDotNormal; // part of velocity aligned along the normal axis
                    
                    // Subtract velocity aligned with normal axis to the velocity of the plane projected perpendicular to collision normal.
                    Vector3 veloARelativeToBProjectedOntoPlane = veloARelativeToB - veloProjectedNormal; // reltive motion between A and B in-plane

                    // Friction
                    if(veloARelativeToBProjectedOntoPlane.sqrMagnitude > 0.00001)
                    {
                        // Magnitude of friction is coefficient of friction times normal force magnitude
                        float frictionMagnitude = Fn.magnitude * objektA.coefficientOfFriction;
                        Vector3 Ff = -veloARelativeToBProjectedOntoPlane.normalized * frictionMagnitude;

                        Debug.DrawRay(objektA.transform.position, Ff, new Color(0.8f, 0.6f, 0.0f));

                        objektA.netForce += Ff;
                        objektB.netForce -= Ff;
                    }

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
    public static bool IsCollidingSpherePlane(FysicsShapeSphere sphere, FysicsShapePlane plane)
    {
        Vector3 planeToSphere = sphere.transform.position - plane.transform.position;
        float positionAlongNormal = Vector3.Dot(planeToSphere, plane.Normal());
        float distanceToPlane = Mathf.Abs(positionAlongNormal);
        float overlap = sphere.radius - distanceToPlane;
        if (overlap > 0.0f)
        {
            //Vector3 minTranslationV = plane.Normal() * overlap;
            //sphere.transform.position += minTranslationV;
            sphere.transform.position += plane.Normal() * overlap;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static CollisionInfo CollisionResponseSphereSphere(FysicsShapeSphere sphereA, FysicsShapeSphere sphereB)
    {
        Vector3 Displacement = sphereA.transform.position - sphereB.transform.position;
        float distance = Displacement.magnitude;
        float overlap = (sphereA.radius + sphereB.radius) - distance;
        if (overlap < 0)
        {
            return new CollisionInfo(false, Vector3.zero);
        }

        Vector3 collisionNormalBtoA;

        if (distance <= 0.00001f)
        {
            collisionNormalBtoA = Vector3.up;
        }
        else
        {
            collisionNormalBtoA = (Displacement / distance);
        }

        Vector3 minTranslationV = collisionNormalBtoA * overlap;
        sphereA.transform.position += minTranslationV / 2;
        sphereB.transform.position -= minTranslationV / 2;

        return new CollisionInfo(true, collisionNormalBtoA);
    }
    public static CollisionInfo CollisionResponseSpherePlane(FysicsShapeSphere sphere, FysicsShapePlane plane)
    {
        Vector3 planeToSphere = sphere.transform.position - plane.transform.position;
        float positionAlongNormal = Vector3.Dot(planeToSphere, plane.Normal());

        float distanceToPlane = Mathf.Abs(positionAlongNormal);

        float overlap = sphere.radius - distanceToPlane;

        if (overlap < 0)
        {
            return new CollisionInfo(false, Vector3.zero);

        }

        sphere.transform.position += plane.Normal() * (sphere.radius - positionAlongNormal);

        return new CollisionInfo(true, plane.Normal());
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
