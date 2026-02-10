using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Core;

namespace Game.Input
{
    public class InputController : MonoBehaviour
    {
        private InputSystem_Actions _inputActions;
        private Vector2 _moveDirection;
        private Vector2 _deltaLookDirection;
        private bool _pressingSprinting;
        private bool _pressingInteracting;
        private bool _pressingBraking;

        public Vector2 MoveDirection => _moveDirection;
        public Vector2 DeltaLookDirection => _deltaLookDirection;
        public bool PressingSprinting => _pressingSprinting;
        public bool PressingInteracting => _pressingInteracting;
        public bool PressingBraking => _pressingBraking;

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
            _deltaLookDirection = context.ReadValue<Vector2>();
        }

        private void OnLookCanceled(InputAction.CallbackContext context)
        {
            _deltaLookDirection = Vector2.zero;
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            _pressingSprinting = true;
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            _pressingSprinting = false;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            _pressingInteracting = true;
        }

        private void OnInteractCanceled(InputAction.CallbackContext context)
        {
            _pressingInteracting = false;
        }

        private void OnBrakePerformed(InputAction.CallbackContext context)
        {
            _pressingBraking = true;
        }

        private void OnBrakeCanceled(InputAction.CallbackContext context)
        {
            _pressingBraking = false;
        }

        // Public methods for command pattern
        public void HandleMove(Vector2 direction, bool isSprinting)
        {
            _moveDirection = direction;
            _pressingSprinting = isSprinting;
        }

        public void HandleLook(Vector2 lookDirection)
        {
            _deltaLookDirection = lookDirection;
        }

        public void HandleInteract()
        {
            _pressingInteracting = true;
        }

        public void HandleBrake(bool isBraking)
        {
            _pressingBraking = isBraking;
        }

        private void Update()
        {
            // Reset interaction flag after one frame
            if (_pressingInteracting)
            {
                _pressingInteracting = false;
            }
        }
    }
}