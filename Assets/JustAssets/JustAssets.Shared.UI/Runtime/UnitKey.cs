using System;

namespace JustAssets.Shared.UI
{
    /// <summary>
    ///     The unit id reference.
    /// </summary>
    [Serializable]
    public class UnitKey
    {
        public int ID;

        public static implicit operator UnitKey(int id)
        {
            UnitKey key = new UnitKey { ID = id };
            return key;
        }
    }
}