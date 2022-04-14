using System.Linq;
using JustAssets.UI.StoreMenu.Store.Configuration;
using UnityEditor;
using UnityEngine;

namespace JustAssets.UI.StoreMenu
{
    [CustomEditor(typeof(UIColorPalette))]
    public class UIColorPaletteInspector : Editor
    {
        private Vector3 _hsvOffset;

        private string[] _ignoreList = new[] { "Confirm", "Decline", "Deterioration", "Improvement", "Positive", "Negative" };

        public override void OnInspectorGUI()
        {
            var style = (UIColorPalette)target;

            _ignoreList = EditorGUILayout.TextArea(string.Join("\n", _ignoreList)).Split('\n');

            EditorGUILayout.BeginHorizontal();
            _hsvOffset = EditorGUILayout.Vector3Field("HSV", _hsvOffset);
            if (GUILayout.Button("Modify", GUILayout.Width(100)))
            {
                foreach (UIStyleName styleColorName in style.ColorNames)
                {
                    if (_ignoreList.Any(x=>styleColorName.Value.Contains(x)))
                        continue;

                    Color color = style[styleColorName];

                    if (_hsvOffset != Vector3.zero)
                    {
                        Color.RGBToHSV(color, out var h, out var s, out var v);
                        h = (h + _hsvOffset.x) % 1f;
                        s = Mathf.Clamp01(s + _hsvOffset.y);
                        v = Mathf.Clamp01(v + _hsvOffset.z);
                        color = Color.HSVToRGB(h, s, v);
                    }

                    style.UpdateStyle(styleColorName, color);
                }

                EditorUtility.SetDirty(style);
            }
            EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }
    }
}