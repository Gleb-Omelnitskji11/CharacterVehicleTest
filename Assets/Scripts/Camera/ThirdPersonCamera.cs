using Core;
using Core.EventBus;
using Input;
using UnityEngine;

namespace Camera
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Offset")] [SerializeField] private Vector3 _characterOffset = new Vector3(0f, 3.87f, -4.2f);
        [SerializeField] private Vector3 _carOffset = new Vector3(0f, 3.87f, -5.2f);

        [Header("Smoothing")] [SerializeField] private float _sensitivity = 120f;
        [SerializeField] private float _positionSmoothTime = 0.05f;

        private Transform _target;

        private float _yaw;
        private float _pitch;
        private Vector3 _velocity;

        private Vector3 _cameraOffset;


        private bool _initialized;
        private bool _lookAvailable;

        private GameStateManager _gameStateManager;
        private IEventBus _eventBus;
        private IInputController _input;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService<ThirdPersonCamera>(this);
        }

        private void Start()
        {
            GetServices();
            Subscribe();
            SetParams();
            UpdateSettings(null);
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<ControlModeChangedSignal>(UpdateSettings);
        }

        private void GetServices()
        {
            _input = ServiceLocator.Instance.GetService<IInputController>();
            _gameStateManager = ServiceLocator.Instance.GetService<GameStateManager>();
            _eventBus = ServiceLocator.Instance.GetService<IEventBus>();
        }

        private void SetParams()
        {
            Vector3 angles = transform.eulerAngles;
            _yaw = angles.y;
            _pitch = angles.x;
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<ControlModeChangedSignal>(UpdateSettings);
        }

        private void UpdateSettings(ControlModeChangedSignal signal)
        {
            _cameraOffset = _gameStateManager.CurrentControlMode == ControlMode.Vehicle ? _carOffset : _characterOffset;
            _target = _gameStateManager.CurrentPlayerTransform;
            _initialized = true;
            _lookAvailable = _gameStateManager.CurrentControlMode == ControlMode.Character;
        }

        private void LateUpdate()
        {
            if (!_initialized) return;

            if (_lookAvailable)
                CalculateInput();
            else
                SnapBehindTarget();
            
            HandlePositionAndRotation();
        }

        private void CalculateInput()
        {
            Vector2 deltaLook = _input.DeltaLookDirection;

            _yaw += deltaLook.x * Time.deltaTime * _sensitivity;
            _pitch -= deltaLook.y * Time.deltaTime * _sensitivity;
            _pitch = Mathf.Clamp(_pitch, -30f, 60f);
        }

        private void SnapBehindTarget()
        {
            Vector3 forward = _target.forward;
            forward.y = 0f;
            forward.Normalize();

            _yaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

            _pitch = 15f;
        }

        private void HandlePositionAndRotation()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);

            Vector3 desiredPosition = _target.position + rotation * _cameraOffset;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition,
                ref _velocity, _positionSmoothTime);


            transform.rotation = rotation;
        }
    }
}