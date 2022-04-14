using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace JustAssets.Shared.UI.Animations
{
    public static class AnimationExtensions
    {
        public static AnimationState GetAnimationState(this Animation animation, int animationIndex)
        {
            var index = 0;
            foreach (AnimationState animationState in animation)
            {
                if (index == animationIndex)
                    return animationState;

                index++;
            }

            return null;
        }

        public static AnimationState GetAnimationState(this Animation animation, string animationsClipName)
        {
            foreach (AnimationState animationState in animation)
            {
                if (animationState.name != animationsClipName)
                    continue;

                return animationState;
            }

            return null;
        }

        public static bool HasParameter(this Animator animator, string paramName)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (string.Equals(param.name, paramName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static IEnumerator HideTogetherWith(AnimatedUI that, params AnimatedUI[] other)
        {
            var wait = new WaitForEndOfFrame();

            that.Hide();
            foreach (AnimatedUI animatedUI in other)
                animatedUI.Hide();

            while (that.State != EMenuState.Hidden || other.Any(x => x.State != EMenuState.Hidden))
                yield return wait;
        }

        public static IEnumerator ShowTogetherWith(AnimatedUI that, params AnimatedUI[] other)
        {
            var wait = new WaitForEndOfFrame();

            that.Show();
            foreach (AnimatedUI animatedUI in other)
                animatedUI.Show();

            while (that.State != EMenuState.Visible || other.Any(x => x.State != EMenuState.Visible))
                yield return wait;
        }

        internal static T GaddComponent<T>(this GameObject gameObject, bool includeInactive = false) where T : Component
        {
            var component = gameObject.GetComponentInChildren<T>(includeInactive);
            return component != null && component.transform == gameObject.transform ? component : gameObject.AddComponent<T>();
        }
    }
}