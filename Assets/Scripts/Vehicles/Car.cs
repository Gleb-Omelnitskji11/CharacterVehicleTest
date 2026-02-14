using Core;
using UnityEngine;
using Zenject;

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

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        public override bool CanInteract()
        {
            if (_gameStateManager.CurrentControlMode == ControlMode.Character)
            {
                float distance = (transform.position - _gameStateManager.CurrentPlayerTransform.position).sqrMagnitude;
                if (_distance >= distance)
                {
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
            _carMoving.SetDriver(true);
            _isDraven = true;
        }
        
        private void TryExitCar()
        {
            _gameStateManager.TryExitCar(_movingHolder, OnExitCar);
        }

        private void OnExitCar()
        {
            _movingHolder.transform.position = _exitPoint.position;
            _movingHolder.gameObject.SetActive(true);
            _carMoving.SetDriver(false);
            _isDraven = false;
        }
    }
}