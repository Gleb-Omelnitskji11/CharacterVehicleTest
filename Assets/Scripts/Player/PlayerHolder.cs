using Core.EventBus;
using UnityEngine;

namespace Player
{
    public class PlayerHolder : MonoBehaviour
    {
        [SerializeField] private CharacterMoving _characterMoving;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;
        private IEventBus _eventBus;
    }
}
