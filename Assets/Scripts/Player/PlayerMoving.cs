using UnityEngine;
using Game.Input;
using Game.Core;

namespace Game.Player
{
    public class PlayerMoving : MonoBehaviour
    {
        private static readonly int Sprint = Animator.StringToHash(ANIMATOR_Sprint);
        private static readonly int Moving = Animator.StringToHash(ANIMATOR_Moving);

        private const string ANIMATOR_Moving = "Moving";
        private const string ANIMATOR_Sprint = "Sprint";

        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _rotationSpeed = 10f;
        
        [Header("Gravity")]
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private float _groundedStickForce = -2f;

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Header("Interaction")]
        [SerializeField] private float _interactionRadius = 3f;
        [SerializeField] private LayerMask _carLayer;

        [Header("Other")]
        [SerializeField] private CharacterController _characterController;
        private InputController _inputController;
        private Vector3 _velocity;
        private float _currentSpeed;

        private bool _animationMoving;
        private bool _animationSprint;
        private GameManager _gameManager;
        public bool CanInteract { get; private set; }
        public GameObject NearbyCar { get; private set; }

        private void Start()
        {
            _inputController = ServiceLocator.Instance.GetService<InputController>();
            _currentSpeed = _walkSpeed;
            _gameManager = ServiceLocator.Instance.GetService<GameManager>();
        }

        private void Update()
        {
            if (_gameManager.CurrentControlMode != ControlMode.Player)
                return;

            Vector3 moveDirection;
            HandleMovement(out moveDirection);
            HandleCharacterRotation(ref moveDirection);
            HandleGravity();
            //CheckForNearbyCars();
            UpdateAnimations();
        }

        private void HandleMovement(out Vector3 moveDirection)
        {
            Vector2 moveInput = _inputController.MoveDirection;
            _animationSprint = _inputController.IsSprinting;
            _animationMoving = moveInput.sqrMagnitude > 0;

            // Calculate target speed
            float targetSpeed = _animationSprint ? _sprintSpeed : _walkSpeed;
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * 10f);

            // Get camera-relative movement direction
            moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            moveDirection.Normalize();

            // Apply movement
            Vector3 movement = moveDirection * _currentSpeed * Time.deltaTime;
            _characterController.Move(movement);
        }
        
        private void HandleGravity()
        {
            if (_characterController.isGrounded)
            {
                if (_velocity.y < 0)
                    _velocity.y = _groundedStickForce;
            }
            else
            {
                _velocity.y += _gravity * Time.deltaTime;
            }

            _characterController.Move(_velocity * Time.deltaTime);
        }
        
        private void HandleCharacterRotation(ref Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
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
            _animator.SetBool(Moving, _animationMoving);
            _animator.SetBool(Sprint, _animationSprint);
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

        // private void OnDrawGizmosSelected()
        // {
        //     // Draw interaction radius
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawWireSphere(transform.position, _interactionRadius);
        //
        //     // Draw ground check
        //     Gizmos.color = Color.green;
        //     Vector3 spherePosition = transform.position + Vector3.up * _groundCheckDistance;
        //     Gizmos.DrawWireSphere(spherePosition, _groundCheckDistance);
        // }
    }
}