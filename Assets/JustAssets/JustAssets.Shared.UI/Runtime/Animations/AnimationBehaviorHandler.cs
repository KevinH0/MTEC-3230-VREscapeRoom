using System.Collections.Generic;
using UnityEngine;

namespace JustAssets.Shared.UI.Animations
{
    public class AnimationBehaviorHandler
    {
        private List<IAnimatedUIBehavior> _animationBehaviors;

        public AnimationBehaviorHandler(List<IAnimatedUIBehavior> animationBehaviors)
        {
            _animationBehaviors = animationBehaviors;
        }
        
        public void Activate()
        {
            foreach (var o in _animationBehaviors)
                o.Activate();
        }

        public void Apply(bool show, float normalizedTime, bool force = false)
        {
            foreach (var o in _animationBehaviors)
                o.Apply(show ? normalizedTime : 1 - normalizedTime, force);
        }

        public void Deactivate()
        {
            foreach (var o in _animationBehaviors)
                o.Deactivate();
        }        
        
        public void Restore()
        {
            foreach (var o in _animationBehaviors)
                o.Restore();
        }

        public void Setup(GameObject gameObject)
        {
            foreach (var o in _animationBehaviors)
                o.Setup(gameObject);
        }
    }
}