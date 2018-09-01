using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;
    [SerializeField]
    private float thrusterForce = 1000f;
    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    [SerializeField]
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount ()
    {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    // Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    private void Update()
    {
        if(PauseMenu.isOn)
        {
            return;
        }

        // setting target position for sprring
        // this makes the physics at right when it comes to
        // applying gravity from flying over objects
        RaycastHit _hit;

        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            // Debug.Log("working!");
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        /* SEGMENT ODPOWIADAJĄCY ZA RUCH */

        // Calculate movement velocity as a 3D vector
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        // final movement Vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        // animate movement!!
        animator.SetFloat("ForwardVelocity", _zMov);

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

        float _cameraRotationX = _xRot * lookSensitivity;

        // apply camera rotation
        motor.RotateCamera(_cameraRotationX);


        // local variable
        Vector3 _thrusterForce = Vector3.zero;

        // calculate thruster force based on player input
        if(Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if(thrusterFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;

            SetJointSettings(jointSpring);
        }

        // limit the amount of variable "thrusterFuelAmount"
        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0, 1);

        // apply thruster force
        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
