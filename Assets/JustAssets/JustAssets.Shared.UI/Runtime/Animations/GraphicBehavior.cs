using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if SUPPORT_COFFEE_EFFECTS
using BaseMeshEffect = Coffee.UIExtensions.BaseMeshEffect;
#endif

namespace JustAssets.Shared.UI.Animations
{
    public abstract class GraphicBehavior<T> : MonoBehaviour, IAnimatedUIBehavior
    {
        [SerializeField]
        private bool _ignoreOtherBehaviorManagers;

        protected List<Behaviour> ComponentsToDeactivate = new List<Behaviour>();

        protected Dictionary<Component, Color> Graphics = new Dictionary<Component, Color>();

        public void Setup(GameObject parent)
        {
            var newGraphics = new List<Component>();
            if (!_ignoreOtherBehaviorManagers)
            {
                newGraphics.AddRange(parent.GetComponentsInChildrenUntil<Graphic, T>());
                newGraphics.AddRange(parent.GetComponentsInChildrenUntil<BaseMeshEffect, T>());
            }
            else
            {
                newGraphics.AddRange(parent.GetComponentsInChildren<Graphic>());
                newGraphics.AddRange(parent.GetComponentsInChildren<BaseMeshEffect>());
            }
            newGraphics = newGraphics.Where(x => !Graphics.ContainsKey(x)).ToList();

            foreach (var graphic in newGraphics)
                switch (graphic)
                {
                    case Graphic graphics:
                        Graphics.Add(graphics, graphics.color);
                        break;
                    case Shadow shadow:
                        Graphics.Add(shadow, shadow.effectColor);
                        break;
#if SUPPORT_COFFEE_EFFECTS
                    case UIShadow shadow:
                        Graphics.Add(shadow, shadow.effectColor);
                        break;
                    case UIEffect effect:
                        Graphics.Add(effect, new Color(effect.effectFactor,0,0));
                        break;
#endif
                }

            SetComponentsToDeactivate(parent);
        }

        private void SetComponentsToDeactivate(GameObject parent)
        {
            AddComponentsToDeactivate<Animator>(parent);
            AddComponentsToDeactivate<Selectable>(parent);
        }

        private void AddComponentsToDeactivate<TComp>(GameObject parent) where TComp : Behaviour
        {
            var newComponents = parent.GetComponentsInChildrenUntil<TComp, T>()
                .Where(x => x.enabled && !ComponentsToDeactivate.Contains(x)).ToList();
            ComponentsToDeactivate.AddRange(newComponents);
        }

        public abstract void Apply(float progress, bool force = false);

        public void Activate()
        {
            ComponentsToDeactivate.ForEach(x => x.enabled = false);
        }

        public void Deactivate()
        {
            ComponentsToDeactivate.ForEach(x => x.enabled = true);
        }

        public abstract void Restore();
    }
}