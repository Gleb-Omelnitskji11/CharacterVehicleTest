using System;
using Core.EventBus;
using UnityEngine;

namespace Core
{
    public class GameStateManager : MonoBehaviour
    {
        [Header("State Management")]
        [SerializeField] private ControlMode _initialControlMode = ControlMode.Character;

        public Transform CurrentPlayerTransform { get; private set; }

        private IEventBus _eventBus;

        public ControlMode CurrentControlMode { get; private set; }

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService<GameStateManager>(this);
        }

        private void Start()
        {
            GetServices();
        }

        private void GetServices()
        {
            _eventBus = ServiceLocator.Instance.GetService<IEventBus>();
        }

        public void SetInitialState(Transform player)
        {
            CurrentControlMode = _initialControlMode;
            CurrentPlayerTransform = player;
            _eventBus.Publish<ControlModeChangedSignal>(new ControlModeChangedSignal(CurrentControlMode));
        }
        
        public void TryEnterCar(Transform car, Action onSuccess)
        {
            if (CurrentControlMode == ControlMode.Vehicle)
                return;

            onSuccess.Invoke();
            CurrentPlayerTransform = car;
            CurrentControlMode = ControlMode.Vehicle;
            _eventBus.Publish<ControlModeChangedSignal>(new ControlModeChangedSignal(CurrentControlMode));
        }

        public void TryExitCar(Transform player, Action onSuccess)
        {
            if (CurrentControlMode != ControlMode.Vehicle)
                return;

            onSuccess.Invoke();
            CurrentPlayerTransform = player;
            CurrentControlMode = ControlMode.Character;
            _eventBus.Publish<ControlModeChangedSignal>(new ControlModeChangedSignal(CurrentControlMode));
        }
    }
}