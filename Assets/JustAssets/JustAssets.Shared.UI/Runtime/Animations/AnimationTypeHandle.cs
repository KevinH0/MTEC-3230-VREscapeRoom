using System;
using System.Collections;
using UnityEngine;

namespace JustAssets.Shared.UI.Animations
{
    public class AnimationTypeHandle : IAnimationTypeHandle
    {
        private readonly AnimatedUI _animatedUI;

        private readonly AnimationClip[] _clips;

        private readonly float _menuAnimationSpeed;

        private readonly bool _startHidden;

        private Coroutine _coroutine;

        public event Func<bool, IEnumerator> AfterAnimation;

        public event Func<bool, IEnumerator> BeforeAnimation;

        public event Func<bool, IEnumerator> BeforeSetup;

        public event Action Hidden;

        public event Action<EMenuState> StateChanged;

        public AnimationTypeHandle(AnimatedUI animatedUI, float menuAnimationSpeed, AnimationClip[] clips, bool startHidden)
        {
            _animatedUI = animatedUI;
            _menuAnimationSpeed = menuAnimationSpeed;
            _clips = clips;
            _startHidden = startHidden;

            Animation = _animatedUI.gameObject.GaddComponent<Animation>(true);
            foreach (AnimationClip animationClip in _clips)
            {
                animationClip.legacy = true;
                Animation.AddClip(animationClip, animationClip.name);
            }
        }

        public Animation Animation { get; }

        public void Cancel()
        {
            if (_coroutine == null)
                return;

            _animatedUI.StopCoroutine(_coroutine);
            _coroutine = null;
            OnHidden();
        }

        public void Show()
        {
            _coroutine = _animatedUI.StartCoroutine(Toggle(true));
        }

        public void Hide()
        {
            _coroutine = _animatedUI.StartCoroutine(Toggle(false));
        }

        public float AnimationProgress { get; private set; }

        public void SeekHideAnimationToEnd()
        {
            AnimationState state = _clips.Length >= 2 ? Animation.GetAnimationState(_clips[1].name) : Animation.GetAnimationState(1);

            SeekAnimationToEnd(state);
        }

        public void SeekShowAnimationToEnd()
        {
            AnimationState state = _clips.Length >= 2 ? Animation.GetAnimationState(_clips[0].name) : Animation.GetAnimationState(0);

            SeekAnimationToEnd(state);
        }

        protected virtual IEnumerator OnAfterAnimation(bool arg)
        {
            return AfterAnimation?.Invoke(arg);
        }

        protected virtual IEnumerator OnBeforeAnimation(bool arg)
        {
            return BeforeAnimation?.Invoke(arg);
        }

        protected virtual IEnumerator OnBeforeSetup(bool showing)
        {
            return BeforeSetup?.Invoke(showing);
        }

        protected virtual void OnStateChanged(EMenuState obj)
        {
            StateChanged?.Invoke(obj);
        }

        private void OnHidden()
        {
            Hidden?.Invoke();
        }

        private void SeekAnimationToEnd(AnimationState state)
        {
            if (state != null)
            {
                Animation.enabled = true;

                state.enabled = true;
                state.weight = 1;
                state.normalizedTime = 1;
                state.speed = 1;
                Animation.Sample();
                state.enabled = false;
            }
        }

        private IEnumerator Toggle(bool show)
        {
            yield return OnBeforeSetup(show);

            _animatedUI.BehaviorHandler.Activate();

            if (_startHidden && show)
                SeekHideAnimationToEnd();

            AnimationProgress = 0f;
            UpdateAnimation(show);

            var wait = new WaitForEndOfFrame();
            yield return wait;

            yield return OnBeforeAnimation(show);

            if (Animation != null)
            {
                var animationIndex = show ? 0 : 1;
                AnimationState animationState = _clips.Length >= 2
                    ? Animation.GetAnimationState(_clips[animationIndex].name)
                    : Animation.GetAnimationState(animationIndex);

                if (animationState != null)
                {
                    animationState.speed = _menuAnimationSpeed;
                    animationState.time = 0;
                    animationState.weight = 1;
                    Animation.Play(animationState.clip.name);
                }

                while (Animation.isPlaying)
                {
                    if (animationState != null)
                    {
                        AnimationProgress = animationState.normalizedTime;
                        UpdateAnimation(show);
                    }

                    yield return wait;
                }

                AnimationProgress = 1f;
                UpdateAnimation(show);
            }

            OnStateChanged(show ? EMenuState.Visible : EMenuState.Hidden);

            yield return OnAfterAnimation(show);

            if (Animation != null && show)
            {
                AnimationState state = Animation.GetAnimationState(2);

                if (state != null)
                {
                    state.speed = _menuAnimationSpeed;
                    Animation.Play(state.clip.name);
                }
            }

            if (!show)
            {
                if (_animatedUI.SetInactiveOnHidden)
                    _animatedUI.gameObject.SetActive(false);

                OnHidden();
            }
            else
            {
                _animatedUI.BehaviorHandler.Restore();
                _animatedUI.BehaviorHandler.Deactivate();
            }

            _coroutine = null;
        }

        private void UpdateAnimation(bool show)
        {
            _animatedUI.BehaviorHandler.Apply(show, AnimationProgress);
        }
    }
}