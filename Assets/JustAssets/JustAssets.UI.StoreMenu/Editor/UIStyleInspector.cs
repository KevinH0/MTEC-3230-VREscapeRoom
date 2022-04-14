using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.UI.StoreMenu.Store.Configuration;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JustAssets.UI.StoreMenu
{
    [CustomEditor(typeof(UIStyle))]
    public class UIStyleInspector : Editor
    {
        public static string[] FieldNames { get; set; }

        public override void OnInspectorGUI()
        {
            Reload();
            
            var style = (UIStyle)target;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find style", GUILayout.Width(100)))
            {
                style.Target = style.GetComponent<Graphic>();
                
                TryFindBestStyles(style);

                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Apply style", GUILayout.Width(100)))
            {
                ApplyStyle(style);
                EditorUtility.SetDirty(style.Target);
            }

            if (GUILayout.Button("Analyze Animations"))
            {
                var oldNames = style.Curves;
                style.Curves = ComputeClipSettingsList(style, out var animationRoot);
                style.Curves.ForEach(x => x.Settings.ForEach(y =>
                    y.StyleName = oldNames.FirstOrDefault(u => u.AnimationClip == x.AnimationClip)?.Settings
                        .FirstOrDefault(v => Math.Abs(v.Time - y.Time) < 0.001f)?.StyleName));

                style.AnimationPath = AnimationUtility.CalculateTransformPath(style.transform, animationRoot);
            }

            EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

        private void TryFindBestStyles(UIStyle style)
        {
            if (ColorPalette.TryFindBestStyle(style.Target.color, out var foundStyleName))
                style.StyleName = foundStyleName;

            foreach (var styleCurve in style.Curves)
            {
                foreach (var styleCurveSetting in styleCurve.Settings)
                {
                    if (ColorPalette.TryFindBestStyle(styleCurveSetting.Color, out var foundStyleNameCurve))
                    {
                        styleCurveSetting.StyleName = foundStyleNameCurve;
                    }
                }
            }
        }

        private static void ApplyStyle(UIStyle style)
        {
            Color oldColor;
            Color newColor;
            using (var serObject = new SerializedObject(style.Target))
            {
                SerializedProperty colorPropA = serObject.FindProperty("m_Color");

                oldColor = colorPropA.colorValue;
                newColor = style.GetColor(style.StyleName);

                if (PrefabUtility.IsPartOfPrefabInstance(style.Target))
                {
                    PrefabUtility.RevertPropertyOverride(colorPropA, InteractionMode.AutomatedAction);
                    serObject.ApplyModifiedProperties();
                }
            }

            var serOb = new SerializedObject(style.Target);
            SerializedProperty colorProp = serOb.FindProperty("m_Color");
            Color revertedColor = colorProp.colorValue;

            if (revertedColor != oldColor && oldColor == newColor)
            {
                newColor = oldColor;
                oldColor = revertedColor;
            }

            if (newColor != oldColor)
            {
                colorProp.colorValue = newColor;
                serOb.ApplyModifiedProperties();
            }
        }

        private static List<UIStyle.ClipSettings> ComputeClipSettingsList(Component component, out Transform animationRoot)
        {
            var foundKeyframes = AnimationClipUtility.FindColorKeyframes(component, out animationRoot);

            var clipSettingsList = foundKeyframes.Select(x =>
                new UIStyle.ClipSettings(x.Key, x.Value.GetKeyframes())).ToList();
            return clipSettingsList;
        }

        private void Reload()
        {
            var uiStyle = (UIStyle)target;

            if (ColorPalette != null)
                return;

            ColorPalette = uiStyle.Styler?.ColorPalette ?? FallbackColorPalette;
            FieldNames = ColorPalette?.ColorNames?.Select(x=>x.Value).ToArray();
        }

        public UIColorPalette ColorPalette { get; set; }

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

    }
}