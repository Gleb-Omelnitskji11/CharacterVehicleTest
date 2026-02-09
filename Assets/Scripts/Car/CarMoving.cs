using UnityEngine;
using Game.Input;
using Game.Core;

namespace Game.Car
{
    public class CarMoving : MonoBehaviour
    {
        [Header("Wheels")] public WheelCollider frontLeft;
        public WheelCollider frontRight;
        public WheelCollider rearLeft;
        public WheelCollider rearRight;

        [Header("Wheel Meshes")] public Transform frontLeftMesh;
        public Transform frontRightMesh;
        public Transform rearLeftMesh;
        public Transform rearRightMesh;

        [Header("Car Settings")] public float motorTorque = 1500f;
        public float maxSteerAngle = 30f;
        public float brakeForce = 3000f;

        [Header("Suspension / Stability")] public float antiRollForce = 5000f;

        private Rigidbody _rb;

        private Vector2 _directionInput;
        private bool _brakeInput;
        private InputController _inputController;
        private GameManager _gameManager;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = new Vector3(0, -0.5f, 0);
        }


        private void Start()
        {
            _inputController = ServiceLocator.Instance.GetService<InputController>();
            _gameManager = ServiceLocator.Instance.GetService<GameManager>();
        }

        private void Update()
        {
            if (_gameManager.CurrentControlMode != ControlMode.Car)
                return;
            
            _directionInput = _inputController.MoveDirection;
            _brakeInput = _inputController.IsBraking;
            UpdateWheelVisuals();
        }

        private void FixedUpdate()
        {
            HandleMotor();
            HandleSteering();
            HandleBrakes();
            ApplyAntiRoll(frontLeft, frontRight);
            ApplyAntiRoll(rearLeft, rearRight);
        }

        // ---------------- MOVEMENT ----------------

        private void HandleMotor()
        {
            rearLeft.motorTorque = _directionInput.y * motorTorque;
            rearRight.motorTorque = _directionInput.y * motorTorque;
        }

        private void HandleSteering()
        {
            float steerAngle = _directionInput.x * maxSteerAngle;
            frontLeft.steerAngle = steerAngle;
            frontRight.steerAngle = steerAngle;
        }

        private void HandleBrakes()
        {
            float force = _brakeInput ? brakeForce : 0f;

            frontLeft.brakeTorque = force;
            frontRight.brakeTorque = force;
            rearLeft.brakeTorque = force;
            rearRight.brakeTorque = force;
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

            float antiRoll = (travelL - travelR) * antiRollForce;

            _rb.AddForceAtPosition(-left.transform.up * antiRoll, left.transform.position);
            _rb.AddForceAtPosition(right.transform.up * antiRoll, right.transform.position);
        }

        // ---------------- VISUALS ----------------

        private void UpdateWheelVisuals()
        {
            UpdateWheel(frontLeft, frontLeftMesh);
            UpdateWheel(frontRight, frontRightMesh);
            UpdateWheel(rearLeft, rearLeftMesh);
            UpdateWheel(rearRight, rearRightMesh);
        }

        private void UpdateWheel(WheelCollider collider, Transform mesh)
        {
            collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            mesh.position = pos;
            mesh.rotation = rot;
        }
    }
}