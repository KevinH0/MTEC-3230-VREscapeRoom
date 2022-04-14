using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace JustAssets.Shared.UI.Animations
{
    public class AnimatedUI : MonoBehaviour
    {
        [FormerlySerializedAs("_useAnimator")]
        public bool UseAnimator;

        private AnimatedUIClosedEventArgs _closedEventArgs;

        private IAnimationTypeHandle _handle;

        private bool _initialized;

        private HorizontalOrVerticalLayoutGroup _layoutInfluencingAnimation;

        private EMenuState _state = EMenuState.Hidden;

        [SerializeField]
        private AnimationClip[] _animationsClips = new AnimationClip[0];

        [SerializeField]
        private bool _isLayoutActiveWhenAnimating = false;

        [SerializeField]
        private float _menuAnimationSpeed = 2f;

        [SerializeField]
        private bool _setInactiveOnHidden = true;

        [SerializeField]
        private bool _startHidden = false;

        public event Action<AnimatedUI, AnimatedUIClosedEventArgs> Hidden;

        public event Action<AnimatedUI, bool> Post;

        public event Action<AnimatedUI, bool> Pre;

        public bool IsVisible => State == EMenuState.Visible || State == EMenuState.Appearing;

        public AnimationBehaviorHandler BehaviorHandler { get; private set; }

        public virtual EMenuState State
        {
            get => _state;
            protected set
            {
                _state = value;

                UpdateLayout();
            }
        }

        public bool SetInactiveOnHidden
        {
            get => _setInactiveOnHidden;
            set => _setInactiveOnHidden = value;
        }

        public bool InfluencedByLayout => _layoutInfluencingAnimation != null;

        public AnimationClip[] AnimationsClips
        {
            get => _animationsClips;
            set => _animationsClips = value;
        }

        public float MenuAnimationSpeed => _menuAnimationSpeed;

        /// <summary>
        ///     Hides the UI animated.
        /// </summary>
        public virtual void Hide()
        {
            Hide(null);
        }

        /// <summary>
        ///     Hides the UI animated.
        /// </summary>
        public virtual void Hide(AnimatedUIClosedEventArgs closedEventArgs)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (State == EMenuState.Hiding)
                return;

            if (State == EMenuState.Hidden)
                return;

            State = EMenuState.Hiding;

            _handle.Hide();

            _closedEventArgs = closedEventArgs;
        }

        public IEnumerator Hiding(float waitUntil = 1f)
        {
            Hide();

            var wait = new WaitForEndOfFrame();

            while (State != EMenuState.Hidden && _handle.AnimationProgress < waitUntil)
                yield return wait;
        }

        public void OnDisable()
        {
            if (_initialized)
                DeInit();
        }

        public void OnEnable()
        {
            if (!_initialized)
                Init();
        }

        public void Play(AnimationClip clip)
        {
            var component = GetComponent<Animation>();
            component[clip.name].speed = 1f;
            component.Play(clip.name);
        }

        public void ResetAnimationBehaviors(bool force = false)
        {
            BehaviorHandler.Apply(false, 1f, force);
        }

        public void ReSetup()
        {
            BehaviorHandler.Setup(gameObject);
            BehaviorHandler.Activate();
            ResetAnimationBehaviors(true);
        }

        /// <summary>
        ///     Shows the UI animated.
        /// </summary>
        public virtual void Show()
        {
            if (!_initialized)
                Init();

            if (State == EMenuState.Visible)
                return;

            if (State == EMenuState.Appearing)
                return;

            // Immediately close old content.
            if (IsVisible)
                _handle.Cancel();

            State = EMenuState.Appearing;

            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            _handle.Show();
        }

        public IEnumerator Showing(float waitUntil = 1f)
        {
            Show();

            var wait = new WaitForEndOfFrame();

            while (State != EMenuState.Visible && _handle.AnimationProgress < waitUntil)
                yield return wait;
        }

        protected virtual void DeInit()
        {
            _initialized = false;
        }

        /// <summary>
        ///     Hides the UI and plays the animation for it.
        /// </summary>
        protected void Fold()
        {
            if (_startHidden)
                _handle.SeekHideAnimationToEnd();
        }

        protected virtual void Init()
        {
            if (!_initialized)
            {
                _initialized = true;
                BehaviorHandler = new AnimationBehaviorHandler(new List<IAnimatedUIBehavior>(GetComponents<IAnimatedUIBehavior>()));
                _layoutInfluencingAnimation = gameObject.GetComponentInParent<HorizontalOrVerticalLayoutGroup>(true);

                gameObject.SetActive(true);
                _handle = UseAnimator
                    ? (IAnimationTypeHandle)new AnimatorTypeHandle(this, _startHidden)
                    : new AnimationTypeHandle(this, _menuAnimationSpeed, AnimationsClips, _startHidden);

                _handle.BeforeAnimation += OnPre;
                _handle.BeforeSetup += OnSetup;
                _handle.AfterAnimation += OnPost;
                _handle.Hidden += OnHidden;
                _handle.StateChanged += OnStateChanged;
            }

            if (_startHidden)
                ReSetup();
        }

        protected virtual void OnHidden()
        {
            Hidden?.Invoke(this, _closedEventArgs);
        }

        /// <summary>
        ///     When the transition state completed and the dialog is visible/hidden now.
        /// </summary>
        /// <param name="showing">If entirely visible now.</param>
        protected virtual IEnumerator OnPost(bool showing)
        {
            yield return null;

            Post?.Invoke(this, showing);
        }

        protected virtual IEnumerator OnPre(bool showing)
        {
            yield return null;

            Pre?.Invoke(this, showing);
        }

        protected virtual IEnumerator OnSetup(bool showing)
        {
            yield return null;
        }

        /// <summary>
        ///     Shows the UI and plays the show animation for it.
        /// </summary>
        protected void Unfold()
        {
            _handle.SeekShowAnimationToEnd();
        }

        private void OnStateChanged(EMenuState newState)
        {
            State = newState;
        }

        private void UpdateLayout()
        {
            if (!_isLayoutActiveWhenAnimating && _layoutInfluencingAnimation != null)
                _layoutInfluencingAnimation.enabled = _state == EMenuState.Visible || _state == EMenuState.Hidden;
        }
    }
}