using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
    public float minimumMomentumSqr = 0.01f;
    public Vector3 gravityAcceleration = new Vector3(0, -10, 0);

    public Vector3 GetGravityForce(FysicsObject objekt)
    {
        return gravityAcceleration * objekt.gravityScale * objekt.mass;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //foreach (FysicsObject objekt in objekts)
        //{
        //    objekt.GetComponent<Renderer>().material.color = Color.white;
        //}
        CollisionUpdate();
        KinematicUpdate();
    }
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

                Vector3 momentum = objekt.velocity * objekt.mass;
                if (momentum.sqrMagnitude < minimumMomentumSqr)
                {
                    objekt.velocity = Vector3.zero;
                }
                Vector3 newPos = objekt.transform.position + objekt.velocity * dt;
                objekt.transform.position = newPos;

                // Debug drawing
                // Velocity
                Debug.DrawRay(objekt.transform.position, objekt.velocity, Color.magenta);
                // Force of gravity
                Debug.DrawRay(objekt.transform.position, Fg, new Color(0.5f, 0.0f, 0.4f));
            } else
            {
                objekt.velocity = Vector3.zero;
            }
            objekt.netForce = Vector3.zero;
        }
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

                FysicsObject objektCorrectedA = objektA;
                FysicsObject objektCorrectedB = objektB;

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
                    objektCorrectedA = objektB;
                    objektCorrectedB = objektA;
                    collisionInfo = CollisionResponseSpherePlane((FysicsShapeSphere)objektCorrectedA.shape, (FysicsShapePlane)objektCorrectedB.shape);
                }
                else if (objektA.shape.GetShape() == FysicsShape.Shape.Rect &&
                         objektB.shape.GetShape() == FysicsShape.Shape.Rect)
                {
                    collisionInfo = CollisionResponseRects((FysicsShapeRect)objektA.shape, (FysicsShapeRect)objektB.shape);
                }
                else if (objektA.shape.GetShape() == FysicsShape.Shape.Sphere &&
                         objektB.shape.GetShape() == FysicsShape.Shape.Rect)
                {
                    collisionInfo = CollisionResponseSphereRect((FysicsShapeSphere)objektA.shape, (FysicsShapeRect)objektB.shape);
                }
                else if(objektA.shape.GetShape() == FysicsShape.Shape.Rect &&
                        objektB.shape.GetShape() == FysicsShape.Shape.Sphere)
                {
                    objektCorrectedA = objektB;
                    objektCorrectedB = objektA;
                    collisionInfo = CollisionResponseSphereRect((FysicsShapeSphere)objektCorrectedA.shape, (FysicsShapeRect)objektCorrectedB.shape);
                }

                if (collisionInfo.didcollide)
                {
                    // Colliding
                    // Changing the color for colliding objects.

                   objektCorrectedA.GetComponent<Renderer>().material.color = Color.red;
                   objektCorrectedB.GetComponent<Renderer>().material.color = Color.red;

                    //  Calculate gravity force
                    Vector3 Fg = GetGravityForce(objektCorrectedA);

                    // Calculate perpendicular component of gravity by project gravity vector onto normal
                    float gravityDotNormal = Vector3.Dot(Fg, collisionInfo.normal);
                    Vector3 gravityProjectedNormal = collisionInfo.normal * gravityDotNormal;

                    // Calculate relative velocity determine if we should apply kinetic friction or not
                    Vector3 veloARelativeToB = objektCorrectedA.velocity - objektCorrectedB.velocity;
                    // Project relative velocity onto the surface (e.g. subtract perpendicular component to a plane)
                    float veloDotNormal = Vector3.Dot(veloARelativeToB, collisionInfo.normal);
                    Vector3 veloProjectedNormal = collisionInfo.normal * veloDotNormal; // part of velocity aligned along the normal axis

                    // If dot-normal and gravity are in the same direction, ignore normal force, friction
                    if (gravityDotNormal < 0.0f) // If they are in opposite directions, then do normal force and friction
                    {
                        // Add normal force due to gravity
                        Vector3 Fn = -gravityProjectedNormal;
                        Debug.DrawRay(objektCorrectedA.transform.position, Fn, Color.green);

                        objektCorrectedA.netForce += Fn;
                        objektCorrectedB.netForce -= Fn;

                        // Subtract velocity aligned with normal axis to the velocity of the plane projected perpendicular to collision normal.
                        Vector3 veloARelativeToBProjectedOntoPlane = veloARelativeToB - veloProjectedNormal; // reltive motion between A and B in-plane

                        // Friction
                        if (veloARelativeToBProjectedOntoPlane.sqrMagnitude > 0.00001)
                        {
                            // Magnitude of friction is coefficient of friction times normal force magnitude
                            float coeffcientOfFriction = Mathf.Clamp01(objektCorrectedA.grippiness * objektCorrectedB.grippiness);
                            float frictionMagnitude = Fn.magnitude * coeffcientOfFriction;

                            // Ff = u * || Fn || * -(vYouRelativeToBue / ||vYouRelativeToBue||)
                            Vector3 Ff = -veloARelativeToBProjectedOntoPlane.normalized * frictionMagnitude;

                            Debug.DrawRay(objektCorrectedA.transform.position, Ff, new Color(0.6f, 0.3f, 0.0f));

                            objektCorrectedA.netForce += Ff;
                            objektCorrectedB.netForce -= Ff;
                        }
                    }

                    // Bouncing/Applying impulse from collision 
                    if (veloDotNormal < 0) // if objects are moving towards eachother
                    {
                        // Bounce!
                        // Determine coefficient of restitution
                        float restitution;
                        if(veloDotNormal > -1) // Dampens motion if the relative velocity is really small
                        {
                            restitution = 0;
                        } else
                        {
                            restitution = (objektCorrectedA.bounciness * objektCorrectedB.bounciness);
                            //restitution = Mathf.Clamp01(objektCorrectedA.bounciness * objektCorrectedB.bounciness);
                        }
                        Debug.Log("Restitution = " + restitution);

                        // From note step 4: Impulse = (1 + resitution * Dot(v1Rel2, N) * m1 * m2 / (m1 + m2)
                        float deltaV1D = (1.0f + restitution) * veloDotNormal;
                        float impulse1D = deltaV1D * objektCorrectedA.mass * objektCorrectedB.mass / (objektCorrectedA.mass + objektCorrectedB.mass);
                        // Impulse is in the direction of the collisionNormal
                        Vector3 impulse3D = collisionInfo.normal * impulse1D;

                        Debug.DrawRay(objektCorrectedA.transform.position, impulse3D, Color.cyan, 0.2f, false);

                        // Apply change in velocity based on impulse
                        objektCorrectedA.velocity += -impulse3D / objektCorrectedA.mass;
                        objektCorrectedB.velocity += impulse3D / objektCorrectedB.mass;
                    }
                }
            }
        }
    }


    public static CollisionInfo CollisionResponseSphereSphere(FysicsShapeSphere sphereA, FysicsShapeSphere sphereB)
    {
        Vector3 Displacement = sphereA.transform.position - sphereB.transform.position;
        float distance = Displacement.magnitude;
        float overlap = sphereA.radius + sphereB.radius - distance;
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

        bool aStatic = sphereA.GetComponent<FysicsObject>().isStatic;
        bool bStatic = sphereB.GetComponent<FysicsObject>().isStatic;

        Vector3 mtv = collisionNormalBtoA * overlap;

        if (aStatic && bStatic)
        {
            return new CollisionInfo(true, collisionNormalBtoA);
        }
        else if (aStatic && !bStatic)
        {
            sphereB.transform.position -= mtv;
        }
        else if (!aStatic && bStatic)
        {
            sphereA.transform.position += mtv;
        }
        else
        {
            sphereA.transform.position += mtv / 2;
            sphereB.transform.position -= mtv / 2;
        }
        return new CollisionInfo(true, collisionNormalBtoA);
    }

    public static CollisionInfo CollisionResponseRects(FysicsShapeRect rectA, FysicsShapeRect rectB)
    {
        Vector3 displacement = rectA.transform.position - rectB.transform.position;

        // Calculate overlap on X and Y axes
        float overlapX = (rectA.width / 2 + rectB.width / 2) - Mathf.Abs(displacement.x);
        float overlapY = (rectA.height / 2 + rectB.height / 2) - Mathf.Abs(displacement.y);

        if (overlapX > 0.0f && overlapY > 0.0f)
        {
            // Resolve collision on the axis with the smallest overlap
            Vector3 collisionNormalBtoA;
            Vector3 mtv;
            if (overlapX < overlapY)
            {
                collisionNormalBtoA = new Vector3(Mathf.Sign(displacement.x), 0, 0);
                mtv = collisionNormalBtoA * overlapX;
            }
            else
            {
                collisionNormalBtoA = new Vector3(0, Mathf.Sign(displacement.y), 0);
                mtv = collisionNormalBtoA * overlapY;
            }

            // Adjust positions based on static/dynamic flags
            bool aStatic = rectA.GetComponent<FysicsObject>().isStatic;
            bool bStatic = rectB.GetComponent<FysicsObject>().isStatic;

            if (aStatic && !bStatic)
            {
                rectB.transform.position -= mtv;
            }
            else if (!aStatic && bStatic)
            {
                rectA.transform.position += mtv;
            }
            else if (!aStatic && !bStatic)
            {
                rectA.transform.position += mtv / 2;
                rectB.transform.position -= mtv / 2;
            }

            return new CollisionInfo(true, collisionNormalBtoA);
        }

        return new CollisionInfo(false, Vector3.zero);
    }


    public static CollisionInfo CollisionResponseSphereRect(FysicsShapeSphere sphere, FysicsShapeRect rect)
    {
        Vector3 closestPoint = sphere.transform.position;
        closestPoint.x = Mathf.Clamp(closestPoint.x, rect.transform.position.x - rect.width / 2, rect.transform.position.x + rect.width / 2);
        closestPoint.y = Mathf.Clamp(closestPoint.y, rect.transform.position.y - rect.height / 2, rect.transform.position.y + rect.height / 2);
        closestPoint.z = rect.transform.position.z;

        Vector3 displacement = sphere.transform.position - closestPoint;

        float distance = displacement.magnitude;

        if (distance < sphere.radius)
        {
            // Overlap amount
            float overlap = sphere.radius - distance;

            // Collision normal
            Vector3 collisionNormal = displacement.normalized;

            // Resolve static vs dynamic objects
            bool aStatic = sphere.GetComponent<FysicsObject>().isStatic;
            bool bStatic = rect.GetComponent<FysicsObject>().isStatic;

            Vector3 mtv = collisionNormal * overlap; // Minimum Translation Vector

            if (aStatic && bStatic)
            {
                return new CollisionInfo(true, collisionNormal);
            }
            else if (aStatic && !bStatic)
            {
                rect.transform.position -= mtv;
            }
            else if (!aStatic && bStatic)
            {
                sphere.transform.position += mtv;
            }
            else
            {
                sphere.transform.position += mtv / 2;
                rect.transform.position -= mtv / 2;
            }

            return new CollisionInfo(true, collisionNormal);
        }

        // No collision
        return new CollisionInfo(false, Vector3.zero);
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
    //public static bool CollideSpheres(FysicsShapeSphere sphereA, FysicsShapeSphere sphereB)
    //{
    //    Vector3 Displacement= sphereA.transform.position - sphereB.transform.position;
    //    float distance = Displacement.magnitude;
    //    float overlap = (sphereA.radius + sphereB.radius) - distance;
    //    if (overlap > 0.0f)
    //    {
    //        Vector3 collisionNormalBtoA = (Displacement / distance);
    //        Vector3 minTranslationV = collisionNormalBtoA * overlap;
    //        sphereA.transform.position += minTranslationV/2;
    //        sphereB.transform.position -= minTranslationV/2;
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
    //public static bool IsCollidingSpherePlane(FysicsShapeSphere sphere, FysicsShapePlane plane)
    //{
    //    Vector3 planeToSphere = sphere.transform.position - plane.transform.position;
    //    float positionAlongNormal = Vector3.Dot(planeToSphere, plane.Normal());
    //    float distanceToPlane = Mathf.Abs(positionAlongNormal);
    //    float overlap = sphere.radius - distanceToPlane;
    //    if (overlap > 0.0f)
    //    {
    //        //Vector3 minTranslationV = plane.Normal() * overlap;
    //        //sphere.transform.position += minTranslationV;
    //        sphere.transform.position += plane.Normal() * overlap;
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
