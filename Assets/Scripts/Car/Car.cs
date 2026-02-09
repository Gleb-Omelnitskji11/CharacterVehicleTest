using UnityEngine;
using Game.Core;

namespace Game.Car
{
    public class Car : MonoBehaviour
    {
        [Header("Car Components")] [SerializeField]
        private CarMoving _carMoving;

        [SerializeField] private GameObject _carModel;
        [SerializeField] private string _carName = "Default Car";

        public CarMoving CarMoving => _carMoving;
        public string CarName => _carName;
        private void Start()
        {
            //ServiceLocator.Instance.GetService<GameManager>().RegisterCar(_carMoving);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw car name
            // #if UNITY_EDITOR
            // UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, _carName);
            // #endif
        }
    }
}