using UnityEngine;

namespace JustAssets.Shared.Providers
{
    public interface IGestureProvider
    {
        void PlayUISound(string uiSelection);

        void PlayUnitSound(string dialogAudio);

        void PlayEmotion(GameObject sender, string emotionName);
    }
}