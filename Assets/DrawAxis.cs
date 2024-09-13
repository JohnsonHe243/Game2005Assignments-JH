using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAxis : MonoBehaviour
{
    public float lineLength = 4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(lineLength, 0, 0), Color.red);
        Debug.DrawLine(transform.position, transform.position + new Vector3(0, lineLength, 0), Color.green);
        Debug.DrawLine(transform.position, transform.position + new Vector3(0, 0, lineLength), Color.blue);
    }
}
