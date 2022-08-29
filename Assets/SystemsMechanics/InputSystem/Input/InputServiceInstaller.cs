using Services.Input.EventsProviders.Generic;
using Zenject;

namespace Services.Input
{
    public class InputServiceInstaller : Installer<InputServiceInstaller>
    {
        public override void InstallBindings()
        {
            
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR 
            Container
                .BindInterfacesAndSelfTo<TouchInputEventsProvider>()
                .AsSingle()
                .NonLazy();
#else
            Container
                .BindInterfacesAndSelfTo<MouseInputEventsProvider>()
                .AsSingle()
                .NonLazy();
#endif
            
            Container
                .BindInterfacesAndSelfTo<InputService>()
                .AsSingle()
                .NonLazy();
        }
    }
}