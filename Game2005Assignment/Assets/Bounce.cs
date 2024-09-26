using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float vel = 0.0f;

    // 60 fps or 60 frames / 1 second
    // 0.02 seconds, then we can convert to framerate by
    // dividing 1 second by frequency
    // rate = 1 / frequency

    // Update is called once per frame
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime; // 0.02s, updating at 50 fps!
        float acc = Physics.gravity.y; // -9.81

        vel = vel + acc * dt;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + vel * dt,
            transform.position.z
        );

        // When the ball goes below the ground plane, flip its velocity.
        if (transform.position.y <= 0)
        {
            vel = -vel; // -9.8 * -1 = 9.8 
                        // 9.8 *-1 = -9.8
        }
    }
}
