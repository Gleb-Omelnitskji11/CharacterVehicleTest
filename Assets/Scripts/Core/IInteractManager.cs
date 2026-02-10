using Player;
using Vehicles;

namespace Core
{
    public interface IInteractManager
    {
        PlayerHolder PlayerHolder { get; }
        bool ChosenInteract { get; }
        void SetInteractionObject(InteractiveObject interactiveObject);
        void UpdateInteractionPrompt(bool showPrompt);
    }
}
