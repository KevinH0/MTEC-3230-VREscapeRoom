using System;
using UnityEditor;
using UnityEngine;

namespace JustAssets.UI.StoreMenu
{
    public abstract class ValueOfDrawer<TType> : PropertyDrawer where TType : Enum
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
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

            var currentValue = (TType)(object)findPropertyRelative.intValue;

            Enum newValue = Attribute.GetCustomAttribute(typeof(TType), typeof(FlagsAttribute)) != null
                ? EditorGUI.EnumFlagsField(amountRect, currentValue)
                : EditorGUI.EnumPopup(amountRect, currentValue);

            findPropertyRelative.intValue = (int)(object)newValue;

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}