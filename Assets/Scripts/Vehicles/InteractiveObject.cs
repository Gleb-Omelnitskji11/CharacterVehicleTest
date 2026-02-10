using UnityEngine;

namespace Car
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        public abstract bool CanInteract();
        public abstract void Interact();
    }
}