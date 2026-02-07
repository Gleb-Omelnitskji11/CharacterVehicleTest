using UnityEngine;
using Game.Input;

namespace Game.Core
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("Initialization Settings")]
        [SerializeField] private bool _autoInitialize = true;
        [SerializeField] private GameObject _inputControllerPrefab;
        [SerializeField] private GameObject _gameManagerPrefab;

        private void Awake()
        {
            if (_autoInitialize)
            {
                InitializeServices();
            }
        }

        private void InitializeServices()
        {
            // Initialize InputController
            if (!ServiceLocator.HasService<InputController>())
            {
                GameObject inputControllerObj;
                
                if (_inputControllerPrefab != null)
                {
                    inputControllerObj = Instantiate(_inputControllerPrefab);
                }
                else
                {
                    inputControllerObj = new GameObject("InputController");
                    inputControllerObj.AddComponent<InputController>();
                }
                
                DontDestroyOnLoad(inputControllerObj);
                Debug.Log("InputController initialized");
            }

            // Initialize GameManager
            if (!ServiceLocator.HasService<GameManager>())
            {
                GameObject gameManagerObj;
                
                if (_gameManagerPrefab != null)
                {
                    gameManagerObj = Instantiate(_gameManagerPrefab);
                }
                else
                {
                    gameManagerObj = new GameObject("GameManager");
                    gameManagerObj.AddComponent<GameManager>();
                }
                
                DontDestroyOnLoad(gameManagerObj);
                Debug.Log("GameManager initialized");
            }

            Debug.Log("All services initialized successfully");
        }

        private void OnDestroy()
        {
            // Clean up services when destroyed
            if (ServiceLocator.Instance != null)
            {
                ServiceLocator.Instance.ClearServices();
            }
        }
    }
}
