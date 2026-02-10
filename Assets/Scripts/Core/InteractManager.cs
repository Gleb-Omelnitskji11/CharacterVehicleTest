using System.Collections.Generic;
using Car;
using Game.Core;
using Game.Input;
using Player;
using UnityEngine;

namespace Core
{
    public class InteractManager : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] private PlayerHolder _player;

        [Header("Cars")]
        [SerializeField] private List<InteractiveObject> _devices;

        [Header("UI")]
        [SerializeField] private GameObject _interactionPrompt;
        
        [Header("Camera")]
        [SerializeField] private ThirdPersonCamera _thirdPersonCamera;
        
        private InputController _inputController;

        private ControlMode _currentControlMode;
        private InteractiveObject _objectToInteract;
        public ControlMode CurrentControlMode => _currentControlMode;
        public GameObject CurrentPlayerMover { get; private set; }
        public PlayerHolder PlayerHolder { get; private set; }
        public bool ChosenInteract { get; private set; }
        public Transform CurrentCamera => _thirdPersonCamera.transform;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService<InteractManager>(this);
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            HandleInteractionInput();
            UpdateInteractionPrompt();
        }

        private void InitializeGame()
        {
            // Get services
            _inputController = ServiceLocator.Instance.GetService<InputController>();
            _thirdPersonCamera = ServiceLocator.Instance.GetService<ThirdPersonCamera>();
            
            // Set initial control mode
            PlayerHolder = _player;
            CurrentPlayerMover = PlayerHolder.gameObject;
            _currentControlMode = ControlMode.Character;
            SetupCameras();
        }

        private void SetupCameras()
        {
            CurrentPlayerMover = _currentControlMode == ControlMode.Character
                ? PlayerHolder.gameObject
                : _objectToInteract.gameObject;
            _thirdPersonCamera.UpdateSettings();
        }

        private void HandleInteractionInput()
        {
            if (_inputController.PressingInteracting)
            {
                HandleInteraction();
            }
        }

        private void UpdateInteractionPrompt()
        {
            bool showPrompt = SetInteractionObject();
            _interactionPrompt.SetActive(showPrompt);
        }

        private bool SetInteractionObject()
        {
            if (ChosenInteract && _objectToInteract.CanInteract())
            {
                return true;
            }

            ChosenInteract = false;
            foreach (var device in _devices)
            {
                if (device.CanInteract())
                {
                    _objectToInteract = device;
                    ChosenInteract = true;
                    return true;
                }
            }

            return false;
        }

        private void HandleInteraction()
        {
            _objectToInteract.Interact();
        }

        public void TryEnterCar()
        {
            if (_currentControlMode == ControlMode.Car)
                return;

            EnterCar();
        }

        public void TryExitCar()
        {
            if (_currentControlMode != ControlMode.Car)
                return;

            ExitCar();
        }

        private void EnterCar()
        {
            PlayerHolder.gameObject.SetActive(false);
            SetControlMode(ControlMode.Car);
            Debug.Log("Player entered car");
        }

        private void ExitCar()
        {
            PlayerHolder.gameObject.SetActive(true);
            SetControlMode(ControlMode.Character);
            ChosenInteract = false;

            Debug.Log("Player exited car");
        }

        private void SetControlMode(ControlMode mode)
        {
            _currentControlMode = mode;
            SetupCameras();
        }
    }
}