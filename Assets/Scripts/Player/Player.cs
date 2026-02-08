using UnityEngine;
using Game.Player;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMoving _playerMoving;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _characterController.enabled = true;
    }
}
