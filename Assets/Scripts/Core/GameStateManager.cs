using System;
using Core.EventBus;
using UnityEngine;

namespace Core
{
    public class GameStateManager
    {
        private ControlMode _initialControlMode = ControlMode.Character;

        public Transform CurrentPlayerTransform { get; private set; }

        private readonly IEventBus _eventBus;

        public ControlMode CurrentControlMode { get; private set; }

        public GameStateManager(IEventBus eventBus)
        {
            _eventBus = eventBus;
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