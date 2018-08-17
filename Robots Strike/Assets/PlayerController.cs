using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float lookSensitivity = 3f;

    private PlayerMotor motor;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
       
    }

    private void Update()
    {
        /* SEGMENT ODPOWIADAJĄCY ZA RUCH */

        // Calculate movement velocity as a 3D vector
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        // final movement Vector
        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;

        // apply movement
        motor.Move(_velocity);


        /* SEGMENT ODPOWIADAJĄCY ZA OBRACANIE */

        // Calculate rotation as a 3D vector
        // (turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        // apply rotation
        motor.Rotate(_rotation);


        /* SEGMENT ODPOWIADAJĄCY ZA KAMERĘ(ruch w górę i w dół) */

        // Calculate camera rotation as a 3D vector
        // (turning around)
        float _xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 _cameraRotation = new Vector3(_xRot, 0f, 0f) * lookSensitivity;

        // apply camera rotation
        motor.RotateCamera(_cameraRotation);
    }
}
