using Core;
using Game.Core;
using UnityEngine;

namespace Game.Input
{
    public interface IInputCommand
    {
        void Execute();
    }

    public class MoveCommand : IInputCommand
    {
        private readonly Vector2 _direction;
        private readonly bool _isSprinting;

        public MoveCommand(Vector2 direction, bool isSprinting)
        {
            _direction = direction;
            _isSprinting = isSprinting;
        }

        public void Execute()
        {
            // Implementation will be handled by the active controller
            ServiceLocator.Instance.GetService<InputController>().HandleMove(_direction, _isSprinting);
        }
    }

    public class LookCommand : IInputCommand
    {
        private readonly Vector2 _lookDirection;

        public LookCommand(Vector2 lookDirection)
        {
            _lookDirection = lookDirection;
        }

        public void Execute()
        {
            ServiceLocator.Instance.GetService<InputController>().HandleLook(_lookDirection);
        }
    }

    public class InteractCommand : IInputCommand
    {
        public void Execute()
        {
            ServiceLocator.Instance.GetService<InputController>().HandleInteract();
        }
    }

    public class BrakeCommand : IInputCommand
    {
        private readonly bool _isBraking;

        public BrakeCommand(bool isBraking)
        {
            _isBraking = isBraking;
        }

        public void Execute()
        {
            ServiceLocator.Instance.GetService<InputController>().HandleBrake(_isBraking);
        }
    }
}
