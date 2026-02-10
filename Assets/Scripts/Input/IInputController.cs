using System;
using UnityEngine;

namespace Input
{
    public interface IInputController
    {
        Vector2 MoveDirection { get; }
        Vector2 DeltaLookDirection { get; }

        event Action<bool> OnHoldSprinting;
        event Action<bool> OnHoldBraking;
        event Action OnPressingInteracting;
    }
}