using System;
using System.Collections.Generic;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Example
{
    [CreateAssetMenu(menuName = "Scriptable Object/Localization Data")]
    public class LocalizationData : ScriptableObject
    {
        public List<Mapping> Data = new List<Mapping>();

        [Serializable]
        public class Mapping
        {
            public string Key;

            public string Translation;
        }
    }
}