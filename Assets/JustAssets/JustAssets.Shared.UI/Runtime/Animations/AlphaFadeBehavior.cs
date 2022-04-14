using System;
using UnityEngine.UI;

namespace JustAssets.Shared.UI.Animations
{
    public class AlphaFadeBehavior : GraphicBehavior<AlphaFadeBehavior>
    {
        private float _lastProgress = -1f;

        public override void Apply(float progress, bool force = false)
        {
            if (!force && Math.Abs(_lastProgress - progress) < 0.001f)
                return;

            if (Graphics != null)
                SetAlpha(progress);

            _lastProgress = progress;
        }

        public override void Restore()
        {
            if (Graphics == null)
                return;

            SetAlpha(1);

            _lastProgress = -1f;
        }

        private void SetAlpha(float alpha)
        {
            foreach (var graphic in Graphics)
            {
                var originalAlpha = graphic.Value.a;
                switch (graphic.Key)
                {
                    case Graphic graphics:
                        var color = graphics.color;
                        color.a = alpha * originalAlpha;
                        graphics.color = color;
                        break;
                    case Shadow shadow:
                        var meshColor = shadow.effectColor;
                        meshColor.a = alpha * originalAlpha;
                        shadow.effectColor = meshColor;
                        break;
#if SUPPORT_COFFEE_EFFECTS
                    case UIShadow effect:
                        var effectColor = effect.effectColor;
                        effectColor.a = alpha * originalAlpha;
                        effect.effectColor = effectColor;
                        break;
#endif
                }
            }
        }
    }
}