using UnityEngine;
using Game.Player;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMoving _playerMoving;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        // Get components if not assigned
        if (_playerMoving == null)
            _playerMoving = GetComponent<PlayerMoving>();
        
        if (_animator == null)
            _animator = GetComponent<Animator>();

        // Validate components
        if (_playerMoving == null)
            Debug.LogError("PlayerMoving component is missing!");
        
        if (_characterController == null)
            Debug.LogError("CharacterController component is missing!");
        
        if (_animator == null)
            Debug.LogWarning("Animator component is missing - animations won't work!");
    }

    private void Start()
    {
        // Ensure the player is properly initialized
        if (_characterController != null)
        {
            _characterController.enabled = true;
        }
    }
}
