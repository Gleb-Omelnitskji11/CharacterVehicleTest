using UnityEngine;
using Game.Input;
using Game.Core;

namespace Game.Player
{
    public class PlayerMoving : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _groundCheckDistance = 0.4f;
        [SerializeField] private LayerMask _groundMask;

        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private float _animationSmoothTime = 0.1f;

        [Header("Interaction")]
        [SerializeField] private float _interactionRadius = 3f;
        [SerializeField] private LayerMask _carLayer;

        private CharacterController _characterController;
        private InputController _inputController;
        private Camera _mainCamera;
        private Vector3 _velocity;
        private bool _isGrounded;
        private float _currentSpeed;
        private float _animationBlend;

        private const string ANIMATOR_SPEED = "Speed";
        private const string ANIMATOR_IS_GROUNDED = "IsGrounded";

        public bool CanInteract { get; private set; }
        public GameObject NearbyCar { get; private set; }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _inputController = ServiceLocator.GetService<InputController>();
            _mainCamera = Camera.main;
            _currentSpeed = _walkSpeed;
        }

        private void Update()
        {
            if (!ServiceLocator.HasService<GameManager>() || 
                ServiceLocator.GetService<GameManager>().CurrentControlMode != ControlMode.Player)
                return;

            CheckGround();
            HandleMovement();
            HandleRotation();
            ApplyGravity();
            CheckForNearbyCars();
            UpdateAnimations();
        }

        private void CheckGround()
        {
            Vector3 spherePosition = transform.position + Vector3.up * _groundCheckDistance;
            _isGrounded = Physics.CheckSphere(spherePosition, _groundCheckDistance, _groundMask);

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }

        private void HandleMovement()
        {
            Vector2 moveInput = _inputController.MoveDirection;
            bool isSprinting = _inputController.IsSprinting;

            // Calculate target speed
            float targetSpeed = isSprinting ? _sprintSpeed : _walkSpeed;
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * 10f);

            // Get camera-relative movement direction
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            moveDirection = _mainCamera.transform.forward * moveDirection.z + _mainCamera.transform.right * moveDirection.x;
            moveDirection.y = 0;
            moveDirection.Normalize();

            // Apply movement
            Vector3 movement = moveDirection * _currentSpeed * Time.deltaTime;
            _characterController.Move(movement);

            // Update animation blend
            _animationBlend = Mathf.Lerp(_animationBlend, moveDirection.magnitude, Time.deltaTime / _animationSmoothTime);
        }

        private void HandleRotation()
        {
            Vector2 moveInput = _inputController.MoveDirection;
            
            if (moveInput.magnitude > 0.1f)
            {
                // Get camera-relative movement direction
                Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
                moveDirection = _mainCamera.transform.forward * moveDirection.z + _mainCamera.transform.right * moveDirection.x;
                moveDirection.y = 0;
                moveDirection.Normalize();

                // Rotate towards movement direction
                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
                }
            }
        }

        private void ApplyGravity()
        {
            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void CheckForNearbyCars()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _interactionRadius, _carLayer);
            CanInteract = false;
            NearbyCar = null;

            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Car"))
                {
                    CanInteract = true;
                    NearbyCar = collider.gameObject;
                    break;
                }
            }
        }

        private void UpdateAnimations()
        {
            if (_animator != null)
            {
                _animator.SetFloat(ANIMATOR_SPEED, _animationBlend);
                _animator.SetBool(ANIMATOR_IS_GROUNDED, _isGrounded);
            }
        }

        public void EnterCar()
        {
            gameObject.SetActive(false);
        }

        public void ExitCar(Vector3 exitPosition)
        {
            transform.position = exitPosition;
            gameObject.SetActive(true);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw interaction radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _interactionRadius);

            // Draw ground check
            Gizmos.color = Color.green;
            Vector3 spherePosition = transform.position + Vector3.up * _groundCheckDistance;
            Gizmos.DrawWireSphere(spherePosition, _groundCheckDistance);
        }
    }
}
