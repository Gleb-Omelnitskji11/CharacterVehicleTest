using Input;
using UnityEngine;
using Zenject;

namespace Vehicles
{
    public class CarMoving : BaseMovementController
    {
        [Header("Wheels")]
        [SerializeField] private WheelCollider _frontLeft;
        [SerializeField] private WheelCollider _frontRight;
        [SerializeField] private WheelCollider _rearLeft;
        [SerializeField] private WheelCollider _rearRight;

        [Header("Wheel Meshes")]
        [SerializeField] private Transform _frontLeftMesh;
        [SerializeField] private Transform _frontRightMesh;
        [SerializeField] private Transform _rearLeftMesh;
        [SerializeField] private Transform _rearRightMesh;

        [Header("Car Settings")]
        [SerializeField] private float _motorTorque = 1500f;

        [SerializeField] private float _maxSteerAngle = 30f;
        [SerializeField] private float _brakeForce = 3000f;
        [SerializeField] private float _brakeCommon = 20f;
        [SerializeField] private float _antiRollForce = 7000f;
        
        [SerializeField] private float _steerSmoothSpeed = 6f;

        [SerializeField] private Rigidbody _rb;

        private Vector2 _directionInput;
        private bool _brakeInput;
        private bool _isDriver;
        
        private float _wheelVisualRotation;
        private float _currentSteerAngle;

        private IInputController _inputController;

        [Inject]
        public void Construct(IInputController inputController)
        {
            _inputController = inputController;
            _inputController.OnHoldBraking += SetBraking;
        }

        private void OnDestroy()
        {
            if(_inputController != null)
                _inputController.OnHoldBraking -= SetBraking;
        }

        private void SetBraking(bool isBraking)
        {
            _brakeInput = isBraking;
            HandleBrakes();
        }

        public override void UpdateMovement()
        {
            _directionInput = _inputController.MoveDirection;
            
            UpdateWheelSpin();
            UpdateWheelVisuals();
        }
        
        private void FixedUpdate()
        {
            HandleBrakes();

            if (_isDriver)
            {
                HandleMotor();
                HandleSteering();
            }
            else
            {
                StopMotor();
            }

            ApplyAntiRoll(_frontLeft, _frontRight);
            ApplyAntiRoll(_rearLeft, _rearRight);
        }
        
        private void StopMotor()
        {
            _rearLeft.motorTorque = 0f;
            _rearRight.motorTorque = 0f;
        }

        private void HandleMotor()
        {
            _rearLeft.motorTorque = _directionInput.y * _motorTorque;
            _rearRight.motorTorque = _directionInput.y * _motorTorque;
        }
        
        private void HandleSteering()
        {
            float steerAngle = _directionInput.x * _maxSteerAngle;
            _currentSteerAngle = Mathf.Lerp(_currentSteerAngle, steerAngle, Time.fixedDeltaTime * _steerSmoothSpeed);

            _frontLeft.steerAngle = _currentSteerAngle;
            _frontRight.steerAngle = _currentSteerAngle;
        }
        
        private void UpdateWheelSpin()
        {
            float rpm = _rearLeft.rpm;
            float spinAmount = rpm * 6f * Time.deltaTime; 
            _wheelVisualRotation += spinAmount;
        }

        private void HandleBrakes()
        {
            float force = 0f;

            if (_brakeInput)
            {
                force = _brakeForce;
            }
            else if (Mathf.Abs(_directionInput.y) < 0.01f)
            {
                force = _brakeCommon; 
            }

            _frontLeft.brakeTorque = force;
            _frontRight.brakeTorque = force;
            _rearLeft.brakeTorque = force;
            _rearRight.brakeTorque = force;
        }

        // ---------------- SUSPENSION ----------------

        private void ApplyAntiRoll(WheelCollider left, WheelCollider right)
        {
            bool leftGrounded = left.GetGroundHit(out WheelHit leftHit);
            bool rightGrounded = right.GetGroundHit(out WheelHit rightHit);

            if (!leftGrounded || !rightGrounded)
                return;

            if (left.suspensionDistance <= 0f || right.suspensionDistance <= 0f)
                return;

            float travelL = (-left.transform.InverseTransformPoint(leftHit.point).y - left.radius)
                            / left.suspensionDistance;

            float travelR = (-right.transform.InverseTransformPoint(rightHit.point).y - right.radius)
                            / right.suspensionDistance;

            float antiRoll = (travelL - travelR) * _antiRollForce;

            _rb.AddForceAtPosition(-left.transform.up * antiRoll, left.transform.position);
            _rb.AddForceAtPosition(right.transform.up * antiRoll, right.transform.position);
        }

        private void UpdateWheelVisuals()
        {
            UpdateWheel(_frontLeft, _frontLeftMesh);
            UpdateWheel(_frontRight, _frontRightMesh);
            UpdateWheel(_rearLeft, _rearLeftMesh);
            UpdateWheel(_rearRight, _rearRightMesh);
        }
        
        private void UpdateWheel(WheelCollider wheelCollider, Transform mesh)
        {
            wheelCollider.GetWorldPose(out Vector3 pos, out _);

            mesh.position = pos;

            Quaternion steerRot = Quaternion.Euler(0f, wheelCollider.steerAngle, 0f);
            Quaternion spinRot = Quaternion.Euler(_wheelVisualRotation, 0f, 0f);

            mesh.rotation = wheelCollider.transform.rotation * steerRot * spinRot;
        }

        public void SetDriver(bool draven)
        {
            _isDriver = draven;
            _directionInput = Vector2.zero;
            SetBraking(false);
        }
    }
}