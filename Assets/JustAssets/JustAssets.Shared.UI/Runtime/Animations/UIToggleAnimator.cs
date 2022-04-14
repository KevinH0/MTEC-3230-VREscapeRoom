using UnityEngine;
using UnityEngine.UI;

namespace JustAssets.Shared.UI.Animations
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Toggle))]
    public class UIToggleAnimator : MonoBehaviour
    {
        private Toggle _toggle;
        private Animator _animator;

        [SerializeField]
        private string _checkedParameter = "Checked";

        private int _checkedParameterId;

        public void OnEnable()
        {
            _checkedParameterId = Animator.StringToHash(_checkedParameter);
            _toggle = GetComponent<Toggle>();
            _animator = GetComponent<Animator>();
            _toggle.onValueChanged.AddListener(OnToggleChanged);

            if (_toggle.isOn)
                OnToggleChanged(true);
        }

        public void OnDisable()
        {
            _toggle?.onValueChanged.RemoveListener(OnToggleChanged);
        }

        public void Update()
        {
            if (_toggle.isOn != _animator.GetBool(_checkedParameterId))
                OnToggleChanged(_toggle.isOn);
        }

        private void OnToggleChanged(bool isOn)
        {
            _animator.SetBool(_checkedParameterId, isOn);
        }
    }
}
