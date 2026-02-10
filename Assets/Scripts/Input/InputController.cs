using System;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputController : MonoBehaviour, IInputController
    {
        private InputSystem_Actions _inputActions;
        private Vector2 _moveDirection;
        private Vector2 _deltaLookDirection;

        public Vector2 MoveDirection => _moveDirection;
        public Vector2 DeltaLookDirection => _deltaLookDirection;
        public event Action<bool> OnHoldSprinting;
        public event Action<bool> OnHoldBraking;
        public event Action OnPressingInteracting;

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
            ServiceLocator.Instance.RegisterService<IInputController>(this);
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
            OnHoldSprinting?.Invoke(true);
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            OnHoldSprinting?.Invoke(false);
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            OnPressingInteracting?.Invoke();
        }

        private void OnBrakePerformed(InputAction.CallbackContext context)
        {
            OnHoldBraking?.Invoke(true);
        }

        private void OnBrakeCanceled(InputAction.CallbackContext context)
        {
            OnHoldBraking?.Invoke(false);
        }
    }
}