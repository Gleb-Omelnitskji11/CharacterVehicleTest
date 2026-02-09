using Game.Core;
using Game.Input;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _characterOffset = new Vector3(0f, 3.87f, -4.2f);
    [SerializeField] private Vector3 _carOffset = new Vector3(0f, 3.87f, -5.2f);
    [SerializeField] private float _sensitivity = 120f;

    [Header("Smoothing")] [SerializeField] private float _positionSmoothTime = 0.05f;

    private float _yaw;
    private float _pitch;
    private Vector3 _velocity;

    private Vector3 _cameraOffset;

    private InputController _input;

    private bool _initialized;
    private GameManager _gameManager;

    private void Awake()
    {
        // Register services
        ServiceLocator.Instance.RegisterService<ThirdPersonCamera>(this);
    }

    private void Start()
    {
        _input = ServiceLocator.Instance.GetService<InputController>();
        _gameManager = ServiceLocator.Instance.GetService<GameManager>();
        // стартовий поворот
        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;
    }

    public void SetTarget(Transform target, ControlMode mode)
    {
        _target = target;
        _initialized = true;
        _cameraOffset = mode == ControlMode.Car ? _carOffset : _characterOffset;
    }

    private void LateUpdate()
    {
        if (!_initialized) return;

        HandleRotation();
        HandlePosition();
    }

    private void HandleRotation()
    {
        Vector2 look = _input.LookDirection;

        _yaw += look.x * Time.deltaTime * _sensitivity;
        _pitch -= look.y * Time.deltaTime * _sensitivity;

        _pitch = Mathf.Clamp(_pitch, -30f, 60f);

        Quaternion camRotation = Quaternion.Euler(_pitch, _yaw, 0f);

        transform.position = _target.position + camRotation * _cameraOffset;
        transform.rotation = camRotation;
    }

    private void HandlePosition()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);

        Vector3 desiredPosition =
            _target.position +
            rotation * _cameraOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _velocity,
            _positionSmoothTime
        );

        transform.rotation = rotation;
    }
}