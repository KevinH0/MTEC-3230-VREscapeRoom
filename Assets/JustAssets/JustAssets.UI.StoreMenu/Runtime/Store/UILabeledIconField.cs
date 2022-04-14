using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JustAssets.UI.StoreMenu.Store
{
    public class UILabeledIconField : MonoBehaviour
    {
        [SerializeField] private TMP_Text _labelText;

        [SerializeField] private TMP_Text _typeText;

        [SerializeField] private TMP_Text _valueText;

        [SerializeField] private Image _fieldIcon;

        public void Init(string value)
        {
            _valueText.text = value;
        }
    }
}