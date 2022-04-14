using System;

namespace JustAssets.Shared.Providers
{
    public interface IDialogManager
    {
        event Action AllDialogsClosed;

        bool AnyDialogOpen { get; }

        void Next();

        void HideAll();

        void ChangeSelection(float yDelta);

        void ShowDialog(IDialogConfigurationProvider data, Action<int> callback);
    }
}