using UnityEngine;
using Game.Input;
using Game.Core;

namespace Game.Car
{
    public class CarMoving : MonoBehaviour
    {
        [Header("Car Settings")]
        [SerializeField] private float _maxSpeed = 50f;
        [SerializeField] private float _acceleration = 15f;
        [SerializeField] private float _brakeForce = 30f;
        [SerializeField] private float _reverseSpeed = 10f;
        [SerializeField] private float _turnSensitivity = 2f;
        [SerializeField] private float _steeringReturnSpeed = 3f;

        [Header("Wheel Colliders")]
        [SerializeField] private WheelCollider _frontLeftWheel;
        [SerializeField] private WheelCollider _frontRightWheel;
        [SerializeField] private WheelCollider _rearLeftWheel;
        [SerializeField] private WheelCollider _rearRightWheel;

        [Header("Wheel Transforms")]
        [SerializeField] private Transform _frontLeftTransform;
        [SerializeField] private Transform _frontRightTransform;
        [SerializeField] private Transform _rearLeftTransform;
        [SerializeField] private Transform _rearRightTransform;

        [Header("Center of Mass")]
        [SerializeField] private Transform _centerOfMass;

        private InputController _inputController;
        private Rigidbody _rigidbody;
        private float _currentSpeed;
        private float _motorTorque;
        private float _brakeTorque;
        private float _steeringAngle;
        private bool _isBraking;
        private bool _isReversing;

        // Suspension settings for realistic physics
        [Header("Suspension")]
        [SerializeField] private float _suspensionDistance = 0.2f;
        [SerializeField] private float _suspensionSpring = 35000f;
        [SerializeField] private float _suspensionDamper = 4500f;
        [SerializeField] private float _suspensionTargetPosition = 0.5f;
        private GameManager _gameManager;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _inputController = ServiceLocator.Instance.GetService<InputController>();
            _gameManager = ServiceLocator.Instance.GetService<GameManager>();
            SetupCarPhysics();
            SetupWheels();
        }

        private void Update()
        {
            if (_gameManager.CurrentControlMode != ControlMode.Car)
                return;
            
            HandleInput();
            UpdateCarPhysics();
            UpdateWheelVisuals();
        }

        private void SetupCarPhysics()
        {
            // Set rigidbody constraints for realistic car behavior
            //_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void SetupWheels()
        {
            // Configure suspension for all wheels
            WheelCollider[] wheels = { _frontLeftWheel, _frontRightWheel, _rearLeftWheel, _rearRightWheel };
            
            foreach (var wheel in wheels)
            {
                if (wheel != null)
                {
                    JointSpring suspension = wheel.suspensionSpring;
                    suspension.spring = _suspensionSpring;
                    suspension.damper = _suspensionDamper;
                    suspension.targetPosition = _suspensionTargetPosition;
                    wheel.suspensionSpring = suspension;

                    wheel.suspensionDistance = _suspensionDistance;
                    wheel.radius = 0.3f;
                    wheel.wheelDampingRate = 0.25f;
                    wheel.forwardFriction = SetFrictionCurve(1.2f, 2f, 0.02f);
                    wheel.sidewaysFriction = SetFrictionCurve(1.5f, 2f, 0.02f);
                }
            }
        }

        private WheelFrictionCurve SetFrictionCurve(float extremumSlip, float extremumValue, float asymptoteValue)
        {
            WheelFrictionCurve friction = new WheelFrictionCurve();
            friction.extremumSlip = extremumSlip;
            friction.extremumValue = extremumValue;
            friction.asymptoteSlip = 2f;
            friction.asymptoteValue = asymptoteValue;
            friction.stiffness = 1f;
            return friction;
        }

        private void HandleInput()
        {
            Vector2 moveInput = _inputController.MoveDirection;
            _isBraking = _inputController.IsBraking;

            // Calculate motor torque based on input
            if (moveInput.y > 0.1f)
            {
                _motorTorque = moveInput.y * _acceleration * _rigidbody.mass;
                _isReversing = false;
            }
            else if (moveInput.y < -0.1f)
            {
                _motorTorque = moveInput.y * _reverseSpeed * _rigidbody.mass;
                _isReversing = true;
            }
            else
            {
                _motorTorque = 0f;
            }

            // Calculate steering angle
            float targetSteeringAngle = moveInput.x * _turnSensitivity;
            
            // Reduce steering at high speeds for realism
            float speedFactor = Mathf.Clamp01(_currentSpeed / _maxSpeed);
            targetSteeringAngle *= (1f - speedFactor * 0.7f);

            _steeringAngle = Mathf.Lerp(_steeringAngle, targetSteeringAngle, Time.deltaTime * _steeringReturnSpeed);

            // Calculate brake torque
            if (_isBraking || (_isReversing && _currentSpeed > 1f))
            {
                _brakeTorque = _brakeForce * _rigidbody.mass;
            }
            else
            {
                _brakeTorque = 0f;
            }
        }

        private void UpdateCarPhysics()
        {
            _currentSpeed = _rigidbody.linearVelocity.magnitude;

            // Apply motor torque to rear wheels (RWD configuration)
            _rearLeftWheel.motorTorque = _motorTorque;
            _rearRightWheel.motorTorque = _motorTorque;

            // Apply steering to front wheels
            _frontLeftWheel.steerAngle = _steeringAngle;
            _frontRightWheel.steerAngle = _steeringAngle;

            // Apply brakes to all wheels
            _frontLeftWheel.brakeTorque = _brakeTorque;
            _frontRightWheel.brakeTorque = _brakeTorque;
            _rearLeftWheel.brakeTorque = _brakeTorque;
            _rearRightWheel.brakeTorque = _brakeTorque;

            // Update wheel physics
            UpdateSingleWheel(_frontLeftWheel);
            UpdateSingleWheel(_frontRightWheel);
            UpdateSingleWheel(_rearLeftWheel);
            UpdateSingleWheel(_rearRightWheel);
        }

        private void UpdateSingleWheel(WheelCollider wheel)
        {
            if (wheel.isGrounded)
            {
                wheel.GetGroundHit(out WheelHit hit);
                Vector3 wheelVelocity = _rigidbody.GetPointVelocity(wheel.transform.position);
                
                // Apply additional forces for realistic behavior
                if (Mathf.Abs(hit.forwardSlip) > 0.1f)
                {
                    _rigidbody.AddForceAtPosition(-hit.forwardSlip * hit.normal * 100f, wheel.transform.position);
                }
            }
        }

        private void UpdateWheelVisuals()
        {
            UpdateWheelTransform(_frontLeftWheel, _frontLeftTransform);
            UpdateWheelTransform(_frontRightWheel, _frontRightTransform);
            UpdateWheelTransform(_rearLeftWheel, _rearLeftTransform);
            UpdateWheelTransform(_rearRightWheel, _rearRightTransform);
        }

        private void UpdateWheelTransform(WheelCollider collider, Transform transform)
        {
            if (collider != null && transform != null)
            {
                Vector3 position;
                Quaternion rotation;
                collider.GetWorldPose(out position, out rotation);
                transform.position = position;
                transform.rotation = rotation;
            }
        }

        public void EnterCar()
        {
            //gameObject.SetActive(true);
        }

        public void ExitCar()
        {
            //gameObject.SetActive(false);
        }

        public Vector3 GetExitPosition()
        {
            // Return a position next to the car for the player to exit
            return transform.position + transform.right * 2f + Vector3.up;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw center of mass
            if (_centerOfMass != null)
            {
                //Gizmos.color = Color.red;
                //Gizmos.DrawSphere(_centerOfMass.position, 0.1f);
            }
        }
    }
}
