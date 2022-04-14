using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.UI.StoreMenu.Store.Configuration;
using UnityEditor;
using UnityEngine;

namespace JustAssets.UI.StoreMenu
{
    [CustomPropertyDrawer(typeof(UIStyleName))]
    public sealed class UIStyleNameDrawer : PropertyDrawer
    {
        
        private Dictionary<UIColorPalette, string[]> _lookup = new Dictionary<UIColorPalette, string[]>();

        private static UIColorPalette _fallbackColorPalette;

        public static UIColorPalette FallbackColorPalette
        {
            get
            {
                if (_fallbackColorPalette == null)
                {
                    var guids = AssetDatabase.FindAssets($"t:{nameof(UIColorPalette)}");
                    if (guids.Length != 0)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        var instance = AssetDatabase.LoadAssetAtPath<UIColorPalette>(path);
                        _fallbackColorPalette = instance;
                    }
                }

                return _fallbackColorPalette;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            UIColorPalette uiColorPalette = (property.serializedObject.targetObject as UIStyle)?.Styler?.ColorPalette;

            if (uiColorPalette == null && !(property.serializedObject.targetObject is UIColorPalette))
                uiColorPalette = FallbackColorPalette;

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var dropDownRect = new Rect(position.x, position.y, position.width-80, position.height);
            var colorRect = new Rect(position.x + position.width - 80, position.y, 80, position.height);

            SerializedProperty findPropertyRelative = property.FindPropertyRelative("_value");
            var currentValue = findPropertyRelative.stringValue;

            string[] options;
            if (uiColorPalette != null)
            {
                if (!_lookup.TryGetValue(uiColorPalette, out options))
                    options = RefreshOptions(uiColorPalette);
            }
            else
                options = Array.Empty<string>();

            var currentIndex = currentValue != null ? Array.IndexOf(options, currentValue) : -1;

            if (currentIndex < 0)
                DrawCreateStyle(property, findPropertyRelative, uiColorPalette != null ? dropDownRect : position, uiColorPalette);
            else
            {
                var newIndex = EditorGUI.Popup(dropDownRect, currentIndex, options);
                var newStyleName = findPropertyRelative.stringValue;
                findPropertyRelative.stringValue = newIndex > 0 ? options[newIndex] : string.Empty;
                EditorGUI.ColorField(colorRect, uiColorPalette[UIStyleName.From(newStyleName)]);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        private void DrawCreateStyle(SerializedProperty parentProp, SerializedProperty targetProp, Rect textFieldRect, UIColorPalette colorPalette)
        {
            if (!string.IsNullOrWhiteSpace(targetProp.stringValue) && colorPalette != null &&
                GUI.Button(textFieldRect.Column((int)(textFieldRect.width - 76), 76), "Create"))
            {
                textFieldRect = textFieldRect.Column(0, textFieldRect.width - 80);
                var propertyPath = parentProp.propertyPath.Substring(0, parentProp.propertyPath.LastIndexOf(".", StringComparison.Ordinal));
                SerializedProperty parentSerializedObject = parentProp.serializedObject.FindProperty(propertyPath);

                Color color = Color.white;
                foreach (SerializedProperty entry in parentSerializedObject)
                {
                    if (entry.type == SerializedPropertyType.Color.ToString())
                    {
                        color = entry.colorValue;
                        break;
                    }
                }

                colorPalette.Add(new UIColorPalette.ColorSetting { Name = UIStyleName.From(targetProp.stringValue), Color = color });
                EditorUtility.SetDirty(colorPalette);
                RefreshOptions(colorPalette);
            }

            targetProp.stringValue = EditorGUI.TextField(textFieldRect, targetProp.stringValue);
        }

        private string[] RefreshOptions(UIColorPalette colorPalette)
        {
            string[] options;
            _lookup[colorPalette] = options = new List<string> { "(Clear)" }.Union(colorPalette.ColorNames.Select(x => x.Value)).ToArray();
            return options;
        }
    }
}