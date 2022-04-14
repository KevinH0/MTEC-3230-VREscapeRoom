using TMPro;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Store
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedStoreUIText : MonoBehaviour
    {
        [SerializeField]
        private string _localizationKey = null;

        private TMP_Text _text;

        public string LocalizationKey => _localizationKey;

        public void OnEnable()
        {
            _text = GetComponent<TMP_Text>();

            StoreUILocalizer.Instance.Register(this);
        }

        public void OnDisable()
        {
            StoreUILocalizer.Instance.Unregister(this);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}
