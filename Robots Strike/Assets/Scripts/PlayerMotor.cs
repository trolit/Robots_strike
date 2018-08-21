using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();


    }

    // gets a movement vector 
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    
    // gets a rotational vector 
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    // gets a rotational vector for the camera
    public void RotateCamera(Vector3 _cameraRotation)
    {
        cameraRotation = _cameraRotation;
    }

    // run every physix iteration
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();

    }

    // perform movement based on velocity variable
    void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            // move a player from position that he currently is + velocity vector
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler (rotation));

        // if there is cam perform rotation
        if(cam != null)
        {
            // without - it would be reversed so it would be like this:
            // moving mouse up - screen goes down
            // moving mouse down - screen goes up
            cam.transform.Rotate(-cameraRotation);
        }
    }
}
