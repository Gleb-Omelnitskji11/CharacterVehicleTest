using UnityEngine;

namespace Core
{
    public abstract class BaseMovementController : MonoBehaviour
    {
        [SerializeField] protected bool _enabled = true;
        
        public bool Enabled 
        { 
            get => _enabled; 
            set => _enabled = value; 
        }

        protected virtual void Update()
        {
            if (_enabled)
            {
                UpdateMovement();
            }
        }

        public abstract void UpdateMovement();

        public virtual void SetActive(bool active)
        {
            _enabled = active;
            enabled = active;
        }
    }
}
