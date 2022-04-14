using System;
using System.Collections;

namespace JustAssets.Shared.UI.Animations
{
    public interface IAnimationTypeHandle
    {
        event Func<bool, IEnumerator> AfterAnimation;

        event Func<bool, IEnumerator> BeforeAnimation;

        event Func<bool, IEnumerator> BeforeSetup;

        event Action Hidden;

        event Action<EMenuState> StateChanged;

        float AnimationProgress { get; }

        void Cancel();

        void Hide();

        void SeekHideAnimationToEnd();

        void SeekShowAnimationToEnd();

        void Show();
    }
}