using Core;
using Game.Core;
using Game.Input;
using UnityEngine;

namespace Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 _characterOffset = new Vector3(0f, 3.87f, -4.2f);
        [SerializeField] private Vector3 _carOffset = new Vector3(0f, 3.87f, -5.2f);
        [SerializeField] private float _sensitivity = 120f;

        [Header("Smoothing")]
        [SerializeField] private float _positionSmoothTime = 0.05f;

        private Transform _target;

        private float _yaw;
        private float _pitch;
        private Vector3 _velocity;

        private Vector3 _cameraOffset;

        private InputController _input;

        private InteractManager _interactManager;
        private bool _initialized;

        private void Awake()
        {
            // Register services
            ServiceLocator.Instance.RegisterService<ThirdPersonCamera>(this);
        }

        private void Start()
        {
            Vector3 angles = transform.eulerAngles;
            _yaw = angles.y;
            _pitch = angles.x;
        }

        public void UpdateSettings()
        {
            if (!_initialized)
            {
                GetServices();
                _initialized = true;
            }

            _cameraOffset = _interactManager.CurrentControlMode == ControlMode.Car ? _carOffset : _characterOffset;
            _target = _interactManager.CurrentPlayerMover.transform;
        }

        private void GetServices()
        {
            _input = ServiceLocator.Instance.GetService<InputController>();
            _interactManager = ServiceLocator.Instance.GetService<InteractManager>();
        }

        private void LateUpdate()
        {
            if (!_initialized) return;

            CalculateInput();
            HandlePositionAndRotation();
        }

        private void CalculateInput()
        {
            Vector2 deltaLook = _input.DeltaLookDirection;

            _yaw += deltaLook.x * Time.deltaTime * _sensitivity;
            _pitch -= deltaLook.y * Time.deltaTime * _sensitivity;

            _pitch = Mathf.Clamp(_pitch, -30f, 60f);
        }

        private void HandlePositionAndRotation()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);

            Vector3 desiredPosition = _target.position + rotation * _cameraOffset;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref _velocity,
                _positionSmoothTime
            );

            transform.rotation = rotation;
        }
    }
}