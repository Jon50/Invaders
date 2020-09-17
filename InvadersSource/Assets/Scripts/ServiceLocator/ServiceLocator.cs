using System.Collections.Generic;

namespace Invaders.Locator
{
    public static class ServiceLocator
    {
        private static List<IService> _services = new List<IService>();

        public static void Register<T>(IService service, bool isSingle = false)
        {
            if (isSingle)
                for (int i = 0; i < _services.Count; i++)
                {
                    if (_services[i].GetType() == typeof(T))
                    {
                        _services[i] = service;
                        return;
                    }
                }

            _services.Add(service);
        }

        public static void Unregister<T>()
        {
            for (int i = 0; i < _services.Count; i++)
            {
                if (_services[i].GetType() == typeof(T))
                    _services.RemoveAt(i);
            }
        }

        public static T Resolve<T>()
        {
            IService _service = default;

            foreach (var service in _services)
            {
                if (service.GetType() == typeof(T))
                {
                    _service = service;
                    break;
                }
            }

            return (T)_service;
        }

        public static List<T> ResolveList<T>()
        {
            var returnList = new List<T>();

            foreach (var service in _services)
                if (service.GetType() == typeof(T))
                    returnList.Add((T)service);

            return returnList;
        }
    }
}