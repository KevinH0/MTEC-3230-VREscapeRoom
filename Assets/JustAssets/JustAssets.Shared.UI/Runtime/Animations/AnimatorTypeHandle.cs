using System;
using System.Collections;
using UnityEngine;

namespace JustAssets.Shared.UI.Animations
{
    public class AnimatorTypeHandle : IAnimationTypeHandle
    {
        private readonly AnimatedUI _animatedUI;

        private float _animationProgress;

        private Animator _animator;

        private Coroutine _coroutine;

        public event Func<bool, IEnumerator> AfterAnimation;

        public event Func<bool, IEnumerator> BeforeAnimation;

        public event Func<bool, IEnumerator> BeforeSetup;

        public event Action Hidden;

        public event Action<EMenuState> StateChanged;

        public AnimatorTypeHandle(AnimatedUI animatedUI, bool startHidden)
        {
            _animatedUI = animatedUI;
            _animator = _animatedUI.gameObject.GaddComponent<Animator>(true);

            if (startHidden)
                SeekHideAnimatorToEnd();
        }

        public void Cancel()
        {
            if (_coroutine == null)
                return;

            _animatedUI.StopCoroutine(_coroutine);
            OnHidden();
        }

        public void Show()
        {
            _coroutine = _animatedUI.StartCoroutine(ToggleAnimator(true));
        }

        public void Hide()
        {
            _coroutine = _animatedUI.StartCoroutine(ToggleAnimator(false));
        }

        public float AnimationProgress { get; private set; }

        public void SeekHideAnimationToEnd()
        {
        }

        public void SeekShowAnimationToEnd()
        {
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

        protected virtual void OnHidden()
        {
            Hidden?.Invoke();
        }

        protected virtual void OnStateChanged(EMenuState obj)
        {
            StateChanged?.Invoke(obj);
        }

        private void SeekHideAnimatorToEnd()
        {
            // Seek animator state hide to the end.
            _animator.SetTrigger("Hide");
            _animator.Update(10f);
            _animator.ResetTrigger("Hide");
        }

        private IEnumerator ToggleAnimator(bool show)
        {
            OnBeforeSetup(show);

            _animatedUI.BehaviorHandler.Activate();

            _animationProgress = 0f;
            UpdateAnimation(show);

            var wait = new WaitForEndOfFrame();
            yield return wait;

            yield return OnBeforeAnimation(show);

            if (_animator != null)
            {
                var animatorState = show ? "Show" : "Hide";
                _animator.speed = _animatedUI.MenuAnimationSpeed;
                _animator.SetTrigger(animatorState);

                AnimatorStateInfo animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                var length = animatorStateInfo.length;
                var time = animatorStateInfo.normalizedTime * length;
                var startTime = Time.time - time;
                _animationProgress = 0f;

                while (_animationProgress < 1f)
                {
                    var normalizedTime = (Time.time - startTime) / length;

                    _animationProgress = normalizedTime;
                    UpdateAnimation(show);

                    yield return wait;
                }
            }

            _animationProgress = 1f;
            UpdateAnimation(show);

            OnStateChanged(show ? EMenuState.Visible : EMenuState.Hidden);

            yield return OnAfterAnimation(show);

            if (_animator != null && show)
            {
                const string idleTrigger = "Idle";
                if (_animator.HasParameter(idleTrigger))
                {
                    _animator.speed = _animatedUI.MenuAnimationSpeed;
                    _animator.SetTrigger(idleTrigger);
                }
            }

            if (!show)
            {
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
            _animatedUI.BehaviorHandler.Apply(show, _animationProgress);
        }
    }
}