using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JustAssets.Shared.UI
{
    public static class GameObjectExtensions
    {       
        public static T GetComponentInParentUpwards<T>(this GameObject gameObject, bool includeInactive) where T : Behaviour
        {
            var transform = gameObject.transform;
            while (transform != null)
            {
                T component;
                if (includeInactive)
                    component = gameObject.GetComponent<T>();
                else
                    component = gameObject.GetComponents<T>().FirstOrDefault(x => x.enabled);

                if (component != null)
                    return component;

                transform = transform.parent;
            }

            return null;
        }

        public static T GetComponentInParent<T>(this GameObject that, bool includeInactive)
        {
            var parent = that.transform.parent;
            if (parent == null || (!parent.gameObject.activeInHierarchy && !includeInactive))
                return default;

            var components = parent.GetComponents<T>();

            return components.Length > 0 ? components[0] : default;
        }

        public static IEnumerable<T> GetComponentsInChildrenUntil<T, TExcept>(this GameObject that)
        {
            var nodeComponents = that.GetComponents<T>();

            foreach (T nodeComponent in nodeComponents)
            {
                yield return nodeComponent;
            }

            foreach (Transform child in that.transform)
            {
                bool hasException = child.GetComponents<TExcept>().Length > 0;

                if (!hasException)
                {
                    foreach (T x1 in GetComponentsInChildrenUntil<T, TExcept>(child.gameObject))
                        yield return x1;
                }
            }
        }
    }
}