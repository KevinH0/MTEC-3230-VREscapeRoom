namespace JustAssets.Shared.Providers
{
    public interface INotificationManager
    {
        void AddNotification(string message, object iconId = default);
    }
}