using System;
using System.Linq;
using JustAssets.Shared.Providers;
using JustAssets.UI.StoreMenu.Example;
using UnityEditor;
using UnityEngine;

namespace Assets.JustAssets.Example
{
    [CustomPropertyDrawer(typeof(ItemId))]
    public sealed class ItemIdDrawer : PropertyDrawer
    {
        private int[] _values;

        private string[] _displayedOptions;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize();
            
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var amountRect = new Rect(position.x, position.y, position.width, position.height);

            SerializedProperty findPropertyRelative = property.FindPropertyRelative("_value");
            
            if (property.name == "Id")
            {
                findPropertyRelative.intValue = EditorGUI.IntField(amountRect, findPropertyRelative.intValue);
            }
            else
            {
                var selectedIndex = Array.IndexOf(_values, findPropertyRelative.intValue);
                var newIndex = EditorGUI.Popup(amountRect, selectedIndex, _displayedOptions);
                if (newIndex >= 0 && _values.Length > newIndex)
                    findPropertyRelative.intValue = _values[newIndex];
            }
            
            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public void Initialize()
        {
            if (_values != null)
                return;

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ItemDataConfiguration)}");
            if (guids.Length != 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var instance = AssetDatabase.LoadAssetAtPath<ItemDataConfiguration>(path);
                var instanceData = instance.Data;
                _values = instanceData.Keys.Select(x=>x.Value).Append(-1).ToArray();
                _displayedOptions = instanceData.Values.Select(x => x.Name).Append("Invalid").ToArray();
            }
        }
    }
}