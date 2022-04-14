using UnityEngine;

namespace JustAssets.UI.StoreMenu.Store.Configuration
{
    public class UIStyler : MonoBehaviour
    {
        [SerializeField]
        private UIColorPalette _colorPalette;

        [SerializeField]
        private float _hueOffset;

        [SerializeField]
        private float _saturationOffset;

        [SerializeField]
        private float _valueOffset;

        public Color this[UIStyleName styleName]
        {
            get
            {
                Color color = _colorPalette[styleName];

                if (_hueOffset != 0f || _valueOffset != 0f || _saturationOffset != 0f)
                {
                    Color.RGBToHSV(color, out var h, out var s, out var v);
                    h = (h + _hueOffset) % 1f;
                    s = Mathf.Clamp01(s + _saturationOffset);
                    v = Mathf.Clamp01(v + _valueOffset);
                    color = Color.HSVToRGB(h, s, v);
                }

                return color;
            }
        }

        public UIColorPalette ColorPalette => _colorPalette;
    }
}