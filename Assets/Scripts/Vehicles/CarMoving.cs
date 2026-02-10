using Core;
using UnityEngine;
using Game.Input;
using Game.Core;

namespace Game.Vehicles
{
    public class CarMoving : MonoBehaviour
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
        [SerializeField] private float _antiRollForce = 5000f;

        private Rigidbody _rb;

        private Vector2 _directionInput;
        private bool _brakeInput;

        private InputController _inputController;
        private InteractManager _interactManager;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = new Vector3(0, -0.5f, 0);
        }


        private void Start()
        {
            _inputController = ServiceLocator.Instance.GetService<InputController>();
            _interactManager = ServiceLocator.Instance.GetService<InteractManager>();
        }

        private void Update()
        {
            if (_interactManager.CurrentControlMode != ControlMode.Car)
                return;

            _directionInput = _inputController.MoveDirection;
            _brakeInput = _inputController.PressingBraking;
            UpdateWheelVisuals();
        }

        private void FixedUpdate()
        {
            HandleMotor();
            HandleSteering();
            HandleBrakes();
            ApplyAntiRoll(_frontLeft, _frontRight);
            ApplyAntiRoll(_rearLeft, _rearRight);
        }

        public void EnterCar()
        {
            this.enabled = true;
        }

        public void ExitCar()
        {
            this.enabled = false;
        }


        // ---------------- MOVEMENT ----------------

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

        // ---------------- VISUALS ----------------

        private void UpdateWheelVisuals()
        {
            UpdateWheel(_frontLeft, _frontLeftMesh);
            UpdateWheel(_frontRight, _frontRightMesh);
            UpdateWheel(_rearLeft, _rearLeftMesh);
            UpdateWheel(_rearRight, _rearRightMesh);
        }

        private void UpdateWheel(WheelCollider collider, Transform mesh)
        {
            collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            mesh.position = pos;
            mesh.rotation = rot;
        }
    }
}