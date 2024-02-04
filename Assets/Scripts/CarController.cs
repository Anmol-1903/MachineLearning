using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
public class CarController : MonoBehaviour
{
    [SerializeField] float _topSpeed = 50f;
    [SerializeField] float _maxSteerAngle = 30f;
    [SerializeField] float _torque = 500f;
    [SerializeField] float _brakingForce = 1000f;

    [Header("Wheels Colliders")]
    [SerializeField] WheelCollider _fl;
    [SerializeField] WheelCollider _fr;
    [SerializeField] WheelCollider _rl;
    [SerializeField] WheelCollider _rr;

    [Header("Wheels Transforms")]
    [SerializeField] Transform _flT;
    [SerializeField] Transform _frT;
    [SerializeField] Transform _rlT;
    [SerializeField] Transform _rrT;

    [Header("DriveType")]
    [SerializeField] bool _front;
    [SerializeField] bool _rear;

    PlayerController playerController;
    Rigidbody rb;
    float dotProduct, _vertical, _horizontal;
    bool handbraking;

    private void Awake()
    {
        playerController = new PlayerController();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        playerController.Enable();
        playerController.Drive.Accelarate.performed += Accelarate_performed;
        playerController.Drive.Accelarate.canceled += Accelarate_canceled;
        playerController.Drive.Steer.performed += Steer_performed;
        playerController.Drive.Steer.canceled += Steer_canceled;
        playerController.Drive.Handbrake.performed += Handbrake_performed;
        playerController.Drive.Handbrake.canceled += Handbrake_canceled;
    }
    private void OnDisable()
    {
        playerController.Disable();
        playerController.Drive.Accelarate.performed -= Accelarate_performed;
        playerController.Drive.Accelarate.canceled -= Accelarate_canceled;
        playerController.Drive.Steer.performed -= Steer_performed;
        playerController.Drive.Steer.canceled -= Steer_canceled;
        playerController.Drive.Handbrake.performed -= Handbrake_performed;
        playerController.Drive.Handbrake.canceled -= Handbrake_canceled;
    }
    private void FixedUpdate()
    {
        CarDrive();
        CarSteer();
        UpdateWheelPos(_fl, _flT);
        UpdateWheelPos(_fr, _frT);
        UpdateWheelPos(_rl, _rlT);
        UpdateWheelPos(_rr, _rrT);
        GamepadManager.Instance.SetTriggerValue(_vertical);
        GamepadManager.Instance.SetStickValue(_horizontal);
        GamepadManager.Instance.SetButtonValue(handbraking);
    }
    void CarDrive()
    {
        Vector3 carVelocity = rb.velocity;
        dotProduct = Vector3.Dot(transform.forward, carVelocity);
        if ((_vertical < 0 && dotProduct > 0.1f) || (_vertical > 0 && dotProduct < -0.1f))      //ABS braking
        {
            _fl.brakeTorque = _brakingForce;
            _fr.brakeTorque = _brakingForce;
            _rl.brakeTorque = _brakingForce;
            _rr.brakeTorque = _brakingForce;
        }
        else if(!handbraking)
        {
            _fl.brakeTorque = 0;
            _fr.brakeTorque = 0;
            _rl.brakeTorque = 0;
            _rr.brakeTorque = 0;
        }
        if (dotProduct < _topSpeed)
        {
            if (_front)
            {
                _fl.motorTorque = _vertical * _torque;
                _fr.motorTorque = _vertical * _torque;
            }
            if (_rear)
            {
                _rl.motorTorque = _vertical * _torque;
                _rr.motorTorque = _vertical * _torque;
            }
        }
    }

    void CarSteer()
    {
        _fl.steerAngle = _horizontal * _maxSteerAngle;
        _fr.steerAngle = _horizontal * _maxSteerAngle;
    }

    void UpdateWheelPos(WheelCollider c, Transform w)
    {
        c.GetWorldPose(out Vector3 pos, out Quaternion rot);
        w.position = pos;
        w.rotation = rot;
    }

    private void Handbrake_canceled(InputAction.CallbackContext obj)
    {
        handbraking = false;
        _rl.brakeTorque = 0;
        _rr.brakeTorque = 0;
    }

    private void Steer_canceled(InputAction.CallbackContext obj)
    {
        _horizontal = 0;
    }

    private void Accelarate_canceled(InputAction.CallbackContext obj)
    {
        _vertical = 0;
    }

    private void Handbrake_performed(InputAction.CallbackContext obj)
    {
        handbraking = true;
        _rl.brakeTorque = Mathf.Infinity;
        _rr.brakeTorque = Mathf.Infinity;
    }

    private void Steer_performed(InputAction.CallbackContext obj)
    {
        _horizontal = obj.ReadValue<float>();
    }

    private void Accelarate_performed(InputAction.CallbackContext obj)
    {
        _vertical = obj.ReadValue<float>();
    }
}