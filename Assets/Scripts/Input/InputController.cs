using UnityEngine;
using UnityEngine.InputSystem;
using Game.Core;

namespace Game.Input
{
    public class InputController : MonoBehaviour
    {
        private InputSystem_Actions _inputActions;
        private Vector2 _moveDirection;
        private Vector2 _lookDirection;
        private bool _isSprinting;
        private bool _isInteracting;
        private bool _isBraking;

        public Vector2 MoveDirection => _moveDirection;
        public Vector2 LookDirection => _lookDirection;
        public bool IsSprinting => _isSprinting;
        public bool IsInteracting => _isInteracting;
        public bool IsBraking => _isBraking;

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
            ServiceLocator.Instance.RegisterService<InputController>(this);
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();

            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            _inputActions.Player.Look.performed += OnLookPerformed;
            _inputActions.Player.Look.canceled += OnLookCanceled;
            _inputActions.Player.Sprint.performed += OnSprintPerformed;
            _inputActions.Player.Sprint.canceled += OnSprintCanceled;
            _inputActions.Player.Interact.performed += OnInteractPerformed;
            _inputActions.Player.Interact.canceled += OnInteractCanceled;

            // Car controls
            _inputActions.Player.Brake.performed += OnBrakePerformed;
            _inputActions.Player.Brake.canceled += OnBrakeCanceled;
        }

        private void OnDisable()
        {
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Look.performed -= OnLookPerformed;
            _inputActions.Player.Look.canceled -= OnLookCanceled;
            _inputActions.Player.Sprint.performed -= OnSprintPerformed;
            _inputActions.Player.Sprint.canceled -= OnSprintCanceled;
            _inputActions.Player.Interact.performed -= OnInteractPerformed;
            _inputActions.Player.Interact.canceled -= OnInteractCanceled;
            _inputActions.Player.Brake.performed -= OnBrakePerformed;
            _inputActions.Player.Brake.canceled -= OnBrakeCanceled;

            _inputActions.Player.Disable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveDirection = Vector2.zero;
        }

        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            _lookDirection = context.ReadValue<Vector2>();
        }

        private void OnLookCanceled(InputAction.CallbackContext context)
        {
            _lookDirection = Vector2.zero;
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            _isSprinting = true;
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            _isSprinting = false;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            _isInteracting = true;
        }

        private void OnInteractCanceled(InputAction.CallbackContext context)
        {
            _isInteracting = false;
        }

        private void OnBrakePerformed(InputAction.CallbackContext context)
        {
            _isBraking = true;
        }

        private void OnBrakeCanceled(InputAction.CallbackContext context)
        {
            _isBraking = false;
        }

        // Public methods for command pattern
        public void HandleMove(Vector2 direction, bool isSprinting)
        {
            _moveDirection = direction;
            _isSprinting = isSprinting;
        }

        public void HandleLook(Vector2 lookDirection)
        {
            _lookDirection = lookDirection;
        }

        public void HandleInteract()
        {
            _isInteracting = true;
            ServiceLocator.Instance.GetService<GameManager>().HandleInteraction();
        }

        public void HandleBrake(bool isBraking)
        {
            _isBraking = isBraking;
        }

        private void Update()
        {
            // Reset interaction flag after one frame
            if (_isInteracting)
            {
                _isInteracting = false;
            }
        }
    }
}