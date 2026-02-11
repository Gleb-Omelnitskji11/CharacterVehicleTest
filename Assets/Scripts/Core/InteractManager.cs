using System.Collections.Generic;
using Input;
using Player;
using UnityEngine;
using Vehicles;
using Zenject;

namespace Core
{
    public class InteractManager : MonoBehaviour
    {
        [Header("Character")] [SerializeField] private PlayerHolder _player;

        [Header("Cars")] [SerializeField] private List<InteractiveObject> _devices;

        [Header("UI")] [SerializeField] private GameObject _interactionPrompt;

        private IInputController _inputController;
        private GameStateManager _gameStateManager;

        private InteractiveObject _objectToInteract;
        public PlayerHolder PlayerHolder { get; private set; }
        public bool ChosenInteract { get; private set; }

        [Inject]
        public void Construct(IInputController inputController , GameStateManager gameStateManager)
        {
            _inputController = inputController;
            _gameStateManager = gameStateManager;
        }

        private void Start()
        {
            InitializeGame();
            Subscribe();
        }

        private void Update()
        {
            UpdateInteractionPrompt();
        }

        private void OnDestroy()
        {
            _inputController.OnPressingInteracting -= HandleInteraction;
        }

        private void Subscribe()
        {
            _inputController.OnPressingInteracting += HandleInteraction;
        }

        private void InitializeGame()
        {
            // Set initial control mode
            PlayerHolder = _player;
            _gameStateManager.SetInitialState(_player.transform);
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

            _objectToInteract = null;
            return false;
        }

        private void HandleInteraction()
        {
            _objectToInteract?.Interact();
        }

        public void SetInteractionObject(InteractiveObject interactiveObject)
        {
            _objectToInteract = interactiveObject;
        }

        public void UpdateInteractionPrompt(bool showPrompt)
        {
            _interactionPrompt.SetActive(showPrompt);
        }
    }
}