using Car;
using Core;
using Game.Core;
using Game.Vehicles;
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
        private InteractManager _interactManager;

        private bool _isDraven;

        private void Start()
        {
            _interactManager = ServiceLocator.Instance.GetService<InteractManager>();
        }

        public override bool CanInteract()
        {
            if (_interactManager.CurrentControlMode == ControlMode.Character)
            {
                float distance = (transform.position - _interactManager.CurrentPlayerMover.transform.position).sqrMagnitude;
                if (_distance >= distance)
                {
                    Debug.Log("Can Interact");
                    return true;
                }
            }
            else if (_interactManager.CurrentControlMode == ControlMode.Car && _isDraven)
            {
                return true;
            }

            return false;
        }

        public override void Interact()
        {
            if(_interactManager.CurrentControlMode == ControlMode.Character)
            {
                EnterCar();
            }
            else
            {
                ExitCar();
            }
        }

        private void EnterCar()
        {
            _carMoving.enabled = true;
            _interactManager.TryEnterCar();
        }
        
        private void ExitCar()
        {
            _carMoving.enabled = false;
            _interactManager.PlayerHolder.transform.position = _exitPoint.position;
            _interactManager.TryExitCar();
        }
    }
}