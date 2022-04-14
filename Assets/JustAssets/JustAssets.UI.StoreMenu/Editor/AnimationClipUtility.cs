using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.UI.StoreMenu.Store.Configuration;
using UnityEditor;
using UnityEngine;

namespace JustAssets.UI.StoreMenu
{
    internal static class AnimationClipUtility
    {
        public static Dictionary<AnimationClip, CurveData> FindColorKeyframes(Component component, out Transform animationRoot)
        {
            var clips = FindClips(component, out animationRoot);
            var currentPath = AnimationUtility.CalculateTransformPath(component.transform, animationRoot);
            var findKeyframes = FindColorKeyframes(clips, currentPath);
            return findKeyframes;
        }

        internal class CurveData
        {
            private List<TimeAndCurve> Data = new List<TimeAndCurve>();

            public void Add(TimeAndCurve valueTuple)
            {
                Data.Add(valueTuple);
            }

            public List<UIStyle.Setting> GetKeyframes()
            {
                var result = new List<UIStyle.Setting>();

                var dataByTime = new Dictionary<float, List<TimeAndCurve>>();
                foreach (TimeAndCurve timeAndCurve in Data)
                {
                    foreach (Keyframe keyframe in timeAndCurve.Curve.keys)
                    {
                        if (!dataByTime.TryGetValue(keyframe.time, out var list))
                            list = dataByTime[keyframe.time] = new List<TimeAndCurve>();

                        list.Add(timeAndCurve);
                    }
                }

                foreach (var timeAndCurves in dataByTime)
                {
                    var time = timeAndCurves.Key;
                    var curves = timeAndCurves.Value;
                    Keyframe? rValue = curves.FirstOrDefault(x => x.Path == "r")?.Curve.keys.FirstOrDefault(x => Math.Abs(x.time - time) < 0.001f);
                    Keyframe? gValue = curves.FirstOrDefault(x => x.Path == "g")?.Curve.keys.FirstOrDefault(x => Math.Abs(x.time - time) < 0.001f);
                    Keyframe? bValue = curves.FirstOrDefault(x => x.Path == "b")?.Curve.keys.FirstOrDefault(x => Math.Abs(x.time - time) < 0.001f);
                    Keyframe? aValue = curves.FirstOrDefault(x => x.Path == "a")?.Curve.keys.FirstOrDefault(x => Math.Abs(x.time - time) < 0.001f);
                    var color = new Color(rValue?.value ?? 1f, gValue?.value ?? 1f, bValue?.value ?? 1f, aValue?.value ?? 1f);
                    var inTangent = new Vector4(rValue?.inTangent ?? 0f, gValue?.inTangent ?? 0f, bValue?.inTangent ?? 0f, aValue?.inTangent ?? 0f);
                    var outTangent = new Vector4(rValue?.outTangent ?? 0f, gValue?.outTangent ?? 0f, bValue?.outTangent ?? 0f, aValue?.outTangent ?? 0f);
                    var inWeight= new Vector4(rValue?.inWeight ?? 0f, gValue?.inWeight ?? 0f, bValue?.inWeight?? 0f, aValue?.inWeight ?? 0f);
                    var outWeight = new Vector4(rValue?.outWeight ?? 0f, gValue?.outWeight ?? 0f, bValue?.outWeight?? 0f, aValue?.outWeight ?? 0f);
                    result.Add(new UIStyle.Setting(time, color, inTangent, outTangent, inWeight, outWeight));
                }

                return result;
            }

            public class TimeAndCurve
            {
                public TimeAndCurve(string path, AnimationCurve curve)
                {
                    Path = path;
                    Curve = curve;
                }

                public string Path { get; }

                public AnimationCurve Curve { get; }
            }
        }

        private static List<AnimationClip> FindClips(Component component, out Transform animationRoot)
        {
            var clips = new List<AnimationClip>();
            var componentInParent = component.GetComponentInParent<Animation>();
            if (componentInParent != null)
            {
                animationRoot = componentInParent.transform;
                foreach (AnimationState variable in componentInParent)
                    clips.Add(variable.clip);
            }
            else
            {
                var animator = component.GetComponentInParent<Animator>();

                if (animator == null)
                {
                    animationRoot = component.transform;
                    return new List<AnimationClip>();
                }

                animationRoot = animator.transform;
                RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;

                if (runtimeAnimatorController is AnimatorOverrideController overrideController)
                {
                    var overrideClips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                    overrideController.GetOverrides(overrideClips);
                    clips.AddRange(overrideClips.Select(x => x.Value ?? x.Key));
                }
                else
                    clips.AddRange(runtimeAnimatorController.animationClips);

                clips.AddRange(runtimeAnimatorController.animationClips);
            }

            return clips;
        }

        private static Dictionary<AnimationClip, CurveData> FindColorKeyframes(List<AnimationClip> clips, string currentPath)
        {
            var result = new Dictionary<AnimationClip, CurveData>();
            foreach (AnimationClip animationClip in clips)
            {
                foreach (EditorCurveBinding editorCurveBinding in AnimationUtility.GetCurveBindings(animationClip))
                {
                    if (currentPath != editorCurveBinding.path)
                        continue;

                    var colorPropertyName = "m_Color";
                    if (editorCurveBinding.propertyName.StartsWith(colorPropertyName))
                    {
                        AnimationCurve animationCurve = AnimationUtility.GetEditorCurve(animationClip, editorCurveBinding);
                        if (!result.TryGetValue(animationClip, out CurveData data))
                            data = result[animationClip] = new CurveData();
                        data.Add(new CurveData.TimeAndCurve(editorCurveBinding.propertyName.Substring(colorPropertyName.Length + 1), animationCurve));
                    }
                }
            }

            return result;
        }
    }
}