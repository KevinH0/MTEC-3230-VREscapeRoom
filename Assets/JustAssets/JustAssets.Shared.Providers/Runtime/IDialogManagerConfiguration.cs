using UnityEngine;

namespace JustAssets.Shared.Providers
{
    public interface IDialogManagerConfiguration
    {
        MonoBehaviour Template { get; }

        GameObject UIContainerTemplate { get; }
    }
}