using UnityEngine;

namespace JustAssets.UI.StoreMenu
{
    public static class RectExtensions
    {
        public static T As<T>(this object that) where T : class
        {
            return that as T;
        }

        public static T Cast<T>(this object that)
        {
            return (T)that;
        }

        public static Rect Column(this Rect original, int offset, float width = 0)
        {
            return new Rect(original.x + offset, original.y, width == 0 ? original.width - offset : width, original.height);
        }

        public static Rect Indent(this Rect original, int indent)
        {
            return new Rect(original.x + indent, original.y + indent, original.width - 2 * indent, original.height - 2 * original.height);
        }

        public static Rect Line(this Rect original, int lineHeight, int lineIndex)
        {
            return new Rect(original.x, original.y + lineHeight * lineIndex, original.width, lineHeight);
        }

        public static Rect Move(this Rect that, Vector2 offset)
        {
            Rect result = that;
            result.x += offset.x;
            result.y += offset.y;
            return result;
        }

        /// <summary>
        ///     Scales the width, height and position of the given rectangle.
        /// </summary>
        /// <param name="original">The rectangle to manipulate.</param>
        /// <param name="scale">The scale to apply.</param>
        /// <returns>Scaled rectangle.</returns>
        public static Rect Scale(this Rect original, Vector2 scale)
        {
            return new Rect(original.x * scale.x, original.y * scale.y, original.width * scale.x, original.height * scale.y);
        }

        public static Rect SetHeight(this Rect original, int height)
        {
            return new Rect(original.x, original.y, original.width, height);
        }
    }
}