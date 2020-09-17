namespace Invaders.Locator
{
    public interface IService
    {
        void RegisterService(IService service);
        void UnregisterService();
    }
}