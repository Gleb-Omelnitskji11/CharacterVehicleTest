using Core;
using Input;
using UnityEngine;

namespace Vehicles
{
    public class CarMoving : BaseMovementController
    {
        [Header("Wheels")] [SerializeField] private WheelCollider _frontLeft;
        [SerializeField] private WheelCollider _frontRight;
        [SerializeField] private WheelCollider _rearLeft;
        [SerializeField] private WheelCollider _rearRight;

        [Header("Wheel Meshes")] [SerializeField]
        private Transform _frontLeftMesh;

        [SerializeField] private Transform _frontRightMesh;
        [SerializeField] private Transform _rearLeftMesh;
        [SerializeField] private Transform _rearRightMesh;

        [Header("Car Settings")] [SerializeField]
        private float _motorTorque = 1500f;

        [SerializeField] private float _maxSteerAngle = 30f;
        [SerializeField] private float _brakeForce = 3000f;
        [SerializeField] private float _antiRollForce = 5000f;

        [SerializeField] private Rigidbody _rb;

        private Vector2 _directionInput;
        private bool _brakeInput;
        private GameObject _driver;

        private IInputController _inputController;

        public bool HasDriver => _driver != null;

        private void Start()
        {
            _inputController = ServiceLocator.Instance.GetService<IInputController>();
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
            UpdateWheelVisuals();
        }

        private void FixedUpdate()
        {
            HandleMotor();
            HandleSteering();
            ApplyAntiRoll(_frontLeft, _frontRight);
            ApplyAntiRoll(_rearLeft, _rearRight);
        }

        private void HandleMotor()
        {
            _rearLeft.motorTorque = _directionInput.y * _motorTorque;
            _rearRight.motorTorque = _directionInput.y * _motorTorque;
        }

        private void HandleSteering()
        {
            float steerAngle = _directionInput.x * _maxSteerAngle;
            _frontLeft.steerAngle = steerAngle;
            _frontRight.steerAngle = steerAngle;
        }

        private void HandleBrakes()
        {
            float force = _brakeInput ? _brakeForce : 0f;

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
            wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            mesh.position = pos;
            mesh.rotation = rot;
        }

        public void SetDriver(GameObject driver)
        {
            _driver = driver;
        }

        public void RemoveDriver()
        {
            _driver = null;
        }

        public Vector3 GetExitPosition()
        {
            // This should be configured per car, for now return transform position + offset
            return transform.position + transform.right * 2f;
        }
    }
}