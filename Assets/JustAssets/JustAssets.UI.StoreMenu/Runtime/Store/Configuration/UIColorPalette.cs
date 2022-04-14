using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable CollectionNeverUpdated.Local

namespace JustAssets.UI.StoreMenu.Store.Configuration
{
    [CreateAssetMenu(menuName = "Scriptable Object/StoreMenu UI Color Palette")]
    public class UIColorPalette : ScriptableObject
    {
        public static readonly UIStyleName ImprovementText = UIStyleName.From(nameof(ImprovementText));

        public static readonly UIStyleName DeteriorationText = UIStyleName.From(nameof(DeteriorationText));

        public static readonly UIStyleName NeutralText = UIStyleName.From(nameof(NeutralText));

        [SerializeField, FormerlySerializedAs("Colors")]
        private List<ColorSetting> _colors = new List<ColorSetting>();

        private Dictionary<UIStyleName, Color> _lookup;

        public Color this[UIStyleName styleName]
        {
            get
            {
                UpdateLookup();
                return _lookup.TryGetValue(styleName, out Color color) ? color : Color.white;
            }
        }

        public ICollection<UIStyleName> ColorNames
        {
            get
            {
                UpdateLookup();
                return _lookup.Keys;
            }
        }

        public void Add(ColorSetting colorSetting)
        {
            _colors.Add(colorSetting);
            _colors.Sort((a,b) => String.Compare(a.Name.Value, b.Name.Value, StringComparison.Ordinal));
            UpdateLookup();
        }

        [Serializable]
        public class ColorSetting
        {
            public Color Color;

            public UIStyleName Name;
        }

        public bool TryFindBestStyle(Color searchColor, out UIStyleName foundStyleName)
        {
            var colorPaletteColors = new List<ColorSetting>(_colors);
            colorPaletteColors.Sort((a, b) =>
            {
                Color xColor = a.Color;
                Color yColor = b.Color;
                Color zColor = searchColor;

                var absA = GetSimilarity(xColor, zColor);
                var absB = GetSimilarity(yColor, zColor);
                return absA.CompareTo(absB);
            });

            ColorSetting closestColor = colorPaletteColors.FirstOrDefault();
            if (closestColor != null && GetSimilarity(closestColor.Color, searchColor) < 0.05f)
            {
                foundStyleName = closestColor.Name;
                return true;
            }

            foundStyleName = null;
            return false;
        }

        private static float GetSimilarity(Color xColor, Color zColor)
        {
            return Math.Abs(xColor.r - zColor.r) + Math.Abs(xColor.g - zColor.g) + Math.Abs(xColor.b - zColor.b);
        }

        private void UpdateLookup()
        {
            if (_lookup == null || _lookup.Count != _colors.Count)
            {
                Dictionary<UIStyleName, Color> dictionary = new Dictionary<UIStyleName, Color>();
                foreach (ColorSetting color in _colors)
                    dictionary[color.Name] = color.Color;
                _lookup = dictionary;
            }
        }

        public void UpdateStyle(UIStyleName styleColorName, Color color)
        {
            _colors.RemoveAll(x => x.Name == styleColorName);
            Add(new ColorSetting { Name = styleColorName, Color = color });
        }
    }
}