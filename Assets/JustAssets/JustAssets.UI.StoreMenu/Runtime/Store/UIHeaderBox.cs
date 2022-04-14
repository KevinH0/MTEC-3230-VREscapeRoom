using TMPro;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Store
{
    public class UIHeaderBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void Init(string flavorText)
        {
            _text.text = flavorText;
        }
    }
}