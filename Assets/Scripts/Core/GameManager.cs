using UnityEngine;
using Game.Input;
using Game.Player;
using Game.Car;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Player Setup")]
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform _playerSpawnPoint;

        [Header("Camera Setup")]
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Camera _carCamera;

        [Header("UI")]
        [SerializeField] private GameObject _interactionPrompt;

        private ControlMode _currentControlMode;
        private PlayerMoving _playerMoving;
        private CarMoving _currentCar;
        private InputController _inputController;

        public ControlMode CurrentControlMode => _currentControlMode;
        public PlayerMoving PlayerMoving => _playerMoving;
        public CarMoving CurrentCar => _currentCar;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
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
            // Register services
            ServiceLocator.Instance.RegisterService<GameManager>(this);
            
            // Find or spawn player
            if (_playerMoving == null)
            {
                if (_playerPrefab != null)
                {
                    GameObject player = Instantiate(_playerPrefab, _playerSpawnPoint.position, _playerSpawnPoint.rotation);
                    _playerMoving = player.GetComponent<PlayerMoving>();
                }
                else
                {
                    _playerMoving = FindObjectOfType<PlayerMoving>();
                }
            }

            // Get input controller
            _inputController = ServiceLocator.GetService<InputController>();

            // Set initial control mode
            _currentControlMode = ControlMode.Player;
            SetupCameras();
        }

        private void SetupCameras()
        {
            switch (_currentControlMode)
            {
                case ControlMode.Player:
                    if (_playerCamera != null) _playerCamera.gameObject.SetActive(true);
                    if (_carCamera != null) _carCamera.gameObject.SetActive(false);
                    break;
                case ControlMode.Car:
                    if (_playerCamera != null) _playerCamera.gameObject.SetActive(false);
                    if (_carCamera != null) _carCamera.gameObject.SetActive(true);
                    break;
            }
        }

        private void HandleInteractionInput()
        {
            if (_inputController != null && _inputController.IsInteracting)
            {
                HandleInteraction();
            }
        }

        private void UpdateInteractionPrompt()
        {
            if (_interactionPrompt != null)
            {
                bool showPrompt = false;
                
                if (_currentControlMode == ControlMode.Player && _playerMoving != null)
                {
                    showPrompt = _playerMoving.CanInteract && _playerMoving.NearbyCar != null;
                }
                else if (_currentControlMode == ControlMode.Car)
                {
                    showPrompt = true; // Always show exit prompt when in car
                }

                _interactionPrompt.SetActive(showPrompt);
            }
        }

        public void HandleInteraction()
        {
            switch (_currentControlMode)
            {
                case ControlMode.Player:
                    TryEnterCar();
                    break;
                case ControlMode.Car:
                    ExitCar();
                    break;
            }
        }

        private void TryEnterCar()
        {
            if (_playerMoving != null && _playerMoving.CanInteract && _playerMoving.NearbyCar != null)
            {
                _currentCar = _playerMoving.NearbyCar.GetComponent<CarMoving>();
                
                if (_currentCar != null)
                {
                    EnterCar();
                }
            }
        }

        private void EnterCar()
        {
            if (_currentCar == null || _playerMoving == null) return;

            // Switch control mode
            _currentControlMode = ControlMode.Car;

            // Disable player
            _playerMoving.EnterCar();

            // Enable car
            _currentCar.EnterCar();

            // Setup cameras
            SetupCameras();

            Debug.Log("Player entered car");
        }

        private void ExitCar()
        {
            if (_currentCar == null || _playerMoving == null) return;

            // Get exit position from car
            Vector3 exitPosition = _currentCar.GetExitPosition();

            // Switch control mode
            _currentControlMode = ControlMode.Player;

            // Disable car
            _currentCar.ExitCar();

            // Enable player at exit position
            _playerMoving.ExitCar(exitPosition);

            // Clear current car reference
            _currentCar = null;

            // Setup cameras
            SetupCameras();

            Debug.Log("Player exited car");
        }

        public void SetControlMode(ControlMode mode)
        {
            _currentControlMode = mode;
            SetupCameras();
        }

        public void RegisterCar(CarMoving car)
        {
            // This can be used for car management if needed
        }

        private void OnDrawGizmosSelected()
        {
            if (_playerSpawnPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_playerSpawnPoint.position, 0.5f);
                Gizmos.DrawLine(_playerSpawnPoint.position, _playerSpawnPoint.position + _playerSpawnPoint.forward * 2f);
            }
        }
    }
}
