using UnityEngine;
using Game.Core;

namespace Game.Car
{
    public class Car : MonoBehaviour
    {
        [Header("Car Components")]
        [SerializeField] private CarMoving _carMoving;
        [SerializeField] private GameObject _carModel;
        [SerializeField] private string _carName = "Default Car";

        public CarMoving CarMoving => _carMoving;
        public string CarName => _carName;

        private void Awake()
        {
            // Get components if not assigned
            if (_carMoving == null)
                _carMoving = GetComponent<CarMoving>();
            
            if (_carModel == null)
                _carModel = transform.GetChild(0).gameObject;

            // Validate components
            if (_carMoving == null)
                Debug.LogError($"CarMoving component is missing on {_carName}!");
            
            if (_carModel == null)
                Debug.LogWarning($"Car model is missing on {_carName}!");

            // Set tag for interaction detection
            if (!gameObject.CompareTag("Car"))
            {
                gameObject.tag = "Car";
            }
        }

        private void Start()
        {
            // Register car with game manager if available
            if (ServiceLocator.HasService<GameManager>())
            {
                ServiceLocator.GetService<GameManager>().RegisterCar(_carMoving);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw car name
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, _carName);
            #endif
        }
    }
}
