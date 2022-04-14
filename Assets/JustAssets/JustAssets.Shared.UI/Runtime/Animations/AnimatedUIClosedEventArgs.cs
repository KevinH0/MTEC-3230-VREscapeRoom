using System;

namespace JustAssets.Shared.UI.Animations
{
    public class AnimatedUIClosedEventArgs : EventArgs
    {
        public bool IsCanceled { get; }

        public AnimatedUIClosedEventArgs(bool isCanceled)
        {
            IsCanceled = isCanceled;
        }
    }
}