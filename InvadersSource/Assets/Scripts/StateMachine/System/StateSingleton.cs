namespace Invaders.StateMachine
{
    public class StateSingleton<T> where T : new()
    {
        private static T _instance;
        public static T MakeInstatnce
        {
            get
            {
                if (_instance == null)
                    _instance = new T();

                return _instance;
            }
        }
    }
}