using System.Collections.Generic;
using UnityEngine;

namespace JustAssets.Shared.Providers
{
    public interface IDialogConfigurationProvider
    {
        string CaptionLocaKey { get; }

        bool IsSelfClosing { get; }

        IList<string> ChoiceLocaKeys { get; }

        string EmotionName { get; }

        string AudioId { get; }

        bool HasOwner { get; }

        int UnitId { get; }

        Transform OverrideTarget { get; }

        int LookAtId { get; }
    }
}