using Core;
using UnityEngine;

namespace Vehicles
{
    public class Car : InteractiveObject
    {
        [Header("Car Components")]
        [SerializeField] private CarMoving _carMoving;
        [SerializeField] private GameObject _carModel;
        [SerializeField] private Rigidbody _carRb;
        
        [SerializeField] private float _distance;
        [SerializeField] private Transform _exitPoint;
        private GameStateManager _gameStateManager;
        private bool _isDraven;

        private Transform _movingHolder;

        private void Start()
        {
            _gameStateManager = ServiceLocator.Instance.GetService<GameStateManager>();
        }

        public override bool CanInteract()
        {
            if (_gameStateManager.CurrentControlMode == ControlMode.Character)
            {
                float distance = (transform.position - _gameStateManager.CurrentPlayerTransform.position).sqrMagnitude;
                if (_distance >= distance)
                {
                    Debug.Log("Can Interact");
                    return true;
                }
            }
            else if (_gameStateManager.CurrentControlMode == ControlMode.Vehicle && _isDraven)
            {
                return true;
            }

            return false;
        }

        public override void Interact()
        {
            if(_gameStateManager.CurrentControlMode == ControlMode.Character)
            {
                TryEnterCar();
            }
            else
            {
                TryExitCar();
            }
        }

        private void TryEnterCar()
        {
            _gameStateManager.TryEnterCar(transform, OnEnterCar);
        }

        private void OnEnterCar()
        {
            _movingHolder = _gameStateManager.CurrentPlayerTransform;
            _movingHolder.gameObject.SetActive(false);
            _carMoving.enabled = true;
        }
        
        private void TryExitCar()
        {
            _gameStateManager.TryExitCar(_movingHolder, OnExitCar);
        }

        private void OnExitCar()
        {
            _movingHolder.transform.position = _exitPoint.position;
            _movingHolder.gameObject.SetActive(true);
            _carMoving.enabled = false;
        }
    }
}