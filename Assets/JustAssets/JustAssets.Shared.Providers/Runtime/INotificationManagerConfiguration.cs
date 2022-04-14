using UnityEngine;

namespace JustAssets.Shared.Providers
{
    public interface INotificationManagerConfiguration
    {
        MonoBehaviour Template { get; }

        GameObject UIContainerTemplate { get; }

        double DisplayDuration { get; }

        int MaxNotifications { get; }

        Sprite DefaultIcon { get; }

        Sprite GetIcon(object iconId);
    }
}