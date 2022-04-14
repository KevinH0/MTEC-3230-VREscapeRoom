using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JustAssets.UI.StoreMenu.Store
{
    public sealed class UIPlusMinusSlider : MonoBehaviour
    {
        [SerializeField] private Button _plusButton;

        [SerializeField] private Button _minusButton;

        [SerializeField] private TMP_Text _text;

        public event Action<int> CountChanged;

        private int _count;
        private int _maxCount;

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value; 
                OnCountChanged();
            }
        }

        public void OnEnable()
        {
            _plusButton.onClick.AddListener(AddOne);
            _minusButton.onClick.AddListener(SubtractOne);
        }

        public void OnDisable()
        {
            _plusButton.onClick.RemoveAllListeners();
            _minusButton.onClick.RemoveAllListeners();
        }
        
        private void SubtractOne()
        {
            int newCount = Count - 1;
            if (newCount < 1)
                newCount = _maxCount;

            Count = newCount;

            UpdateDisplay();
        }

        private void AddOne()
        {
            int newCount = Count + 1;
            if (newCount > _maxCount)
                newCount = 1;

            Count = newCount;

            UpdateDisplay();
        }

        public void Set(int count, int maximum)
        {
            Count = count;
            _maxCount = maximum;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            _text.text = Count.ToString();
        }

        private void OnCountChanged()
        {
            CountChanged?.Invoke(Count);
        }
    }
}