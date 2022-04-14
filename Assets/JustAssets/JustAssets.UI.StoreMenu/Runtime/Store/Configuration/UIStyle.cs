using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace JustAssets.UI.StoreMenu.Store.Configuration
{
    public class UIStyle : MonoBehaviour
    {
        public string AnimationPath;

        public List<ClipSettings> Curves;

        public UIStyleName StyleName;

        public Graphic Target;

        private UIStyler _styler;

        public UIStyler Styler
        {
            get
            {
                if (_styler == null)
                {
                    _styler = GetComponentInParent<UIStyler>();
                }

                return _styler ;
            }
        }

        public void Apply()
        {
            Color targetColor = GetColor(StyleName);
            Target.color = new Color(targetColor.r, targetColor.g, targetColor.b, Target.color.a);

            foreach (var curve in Curves)
            {
                var animationCurves = curve.ToCurves(GetColor);
                foreach (var animationCurve in animationCurves)
                {
                    curve.AnimationClip.SetCurve(AnimationPath, Target.GetType(), "m_Color." + animationCurve.Key, animationCurve.Value);
                }
            }
        }

        public Color GetColor(UIStyleName styleName)
        {
            return Styler[styleName];
        }

        public void Awake()
        {
            Apply();
        }

        [Serializable]
        public class Setting
        {
            public float Time;

            public Vector4 TangentIn;

            public Vector4 TangentOut;

            public Vector4 WeightIn;

            public Vector4 WeightOut;

            public Color Color;

            public UIStyleName StyleName;
            
            public Setting()
            {
            }

            public Setting(float time, Color color, Vector4 tangentIn, Vector4 tangentOut, Vector4 weightIn, Vector4 weightOut)
            {
                Time = time;
                Color = color;
                TangentIn = tangentIn;
                TangentOut = tangentOut;
                WeightIn = weightIn;
                WeightOut = weightOut;
            }
        }

        [Serializable]
        public class ClipSettings
        {
            public AnimationClip AnimationClip;

            public List<Setting> Settings;

            public ClipSettings(AnimationClip animationClip, List<Setting> settings)
            {
                AnimationClip = animationClip;
                Settings = settings;
            }

            public Dictionary<string, AnimationCurve> ToCurves(Func<UIStyleName, Color> getColorForStyle)
            {
                var r = new AnimationCurve(Settings.Select(x => new Keyframe(x.Time, getColorForStyle(x.StyleName).r, x.TangentIn.x, x.TangentOut.x, x.WeightIn.x, x.WeightOut.x)).ToArray());
                var g = new AnimationCurve(Settings.Select(x => new Keyframe(x.Time, getColorForStyle(x.StyleName).g, x.TangentIn.y, x.TangentOut.y, x.WeightIn.y, x.WeightOut.y)).ToArray());
                var b = new AnimationCurve(Settings.Select(x => new Keyframe(x.Time, getColorForStyle(x.StyleName).b, x.TangentIn.z, x.TangentOut.z, x.WeightIn.z, x.WeightOut.z)).ToArray());
                //var a = new AnimationCurve(Settings.Select(x => new Keyframe(x.Time, getColorForStyle(x.StyleName).a, x.TangentIn.w, x.TangentOut.w, x.WeightIn.w, x.WeightOut.w)).ToArray());

                return new Dictionary<string, AnimationCurve>
                {
                    { "r", r },
                    { "g", g },
                    { "b", b },
                    //{ "a", a }
                };
            }
        }
        
    }
}