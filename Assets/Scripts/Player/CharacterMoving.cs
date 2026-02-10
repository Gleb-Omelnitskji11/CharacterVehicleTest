using Core;
using UnityEngine;
using Game.Input;
using Game.Core;

namespace Game.Player
{
    public class CharacterMoving : MonoBehaviour
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

        [Header("Other")]
        [SerializeField] private CharacterController _characterController;
        
        private InputController _inputController;
        private Vector3 _velocity;
        private float _currentSpeed;

        private bool _animationMoving;
        private bool _animationSprint;
        private InteractManager _interactManager;
        private Transform _cameraTransform;

        private void Start()
        {
            _inputController = ServiceLocator.Instance.GetService<InputController>();
            _currentSpeed = _walkSpeed;
            _interactManager = ServiceLocator.Instance.GetService<InteractManager>();
            _cameraTransform = _interactManager.CurrentCamera;
        }

        private void Update()
        {
            if (_interactManager.CurrentControlMode != ControlMode.Character)
                return;

            Vector3 moveDirection;
            HandleMovement(out moveDirection);
            HandleCharacterRotation(ref moveDirection);
            HandleGravity();
            UpdateAnimations();
        }

        private void HandleMovement(out Vector3 moveDirection)
        {
            Vector2 moveInput = _inputController.MoveDirection;
            _animationSprint = _inputController.PressingSprinting;
            _animationMoving = moveInput.sqrMagnitude > 0;

            // Calculate target speed
            float targetSpeed = _animationSprint ? _sprintSpeed : _walkSpeed;
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * 10f);

            // Get camera-relative movement direction
            moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            moveDirection = _cameraTransform.transform.forward * moveDirection.z + _cameraTransform.transform.right * moveDirection.x;
            moveDirection.y = 0;
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
    }
}