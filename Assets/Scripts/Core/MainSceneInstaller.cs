using Camera;
using Input;
using UnityEngine;
using Zenject;

namespace Core
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private InteractManager _interactManager;
        [SerializeField] private ThirdPersonCamera _thirdPersonCamera;

        public override void InstallBindings()
        {
            Container.Bind<GameStateManager>().AsCached();
            Container.Bind<IInputController>().FromInstance(_inputController);
            Container.Bind<InteractManager>().FromInstance(_interactManager);
            Container.Bind<ThirdPersonCamera>().FromInstance(_thirdPersonCamera);
        }
    }
}