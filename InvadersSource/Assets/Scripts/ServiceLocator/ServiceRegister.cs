using UnityEngine;

namespace Invaders.Locator
{
    public class ServiceRegister<T> : MonoBehaviour, IService
    {
        private void OnDisable() => UnregisterService();

        public void RegisterService(IService service) => ServiceLocator.Register<T>(service);

        public void UnregisterService() => ServiceLocator.Unregister<T>();
    }
}