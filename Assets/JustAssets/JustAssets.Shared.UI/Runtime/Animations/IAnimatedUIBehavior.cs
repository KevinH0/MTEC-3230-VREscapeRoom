using UnityEngine;

namespace JustAssets.Shared.UI.Animations
{
    public interface IAnimatedUIBehavior
    {
        void Activate();

        void Apply(float progress, bool force = false);

        void Deactivate();

        void Restore();

        void Setup(GameObject parent);
    }
}