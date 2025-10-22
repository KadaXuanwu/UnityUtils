using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.Extensions {
    /// <summary>
    /// Extension methods for Unity's Color struct that provide convenient alpha channel manipulation.
    /// </summary>
    public static class ColorExtensions {
        /// <summary>
        /// Creates a new color with the specified alpha value while preserving RGB channels.
        /// </summary>
        /// <param name="color">The source color.</param>
        /// <param name="alpha">The new alpha value to apply (0-1 range).</param>
        /// <returns>A new Color with the updated alpha channel.</returns>
        public static Color SetAlpha(this Color color, float alpha) {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// Creates a new color using RGB from the new color and alpha from the old color.
        /// </summary>
        /// <param name="newColor">The color to use for RGB channels.</param>
        /// <param name="oldColor">The color whose alpha channel to preserve.</param>
        /// <returns>A new Color combining newColor's RGB with oldColor's alpha.</returns>
        public static Color WithAlphaFromColor(this Color newColor, Color oldColor) {
            newColor.a = oldColor.a;
            return newColor;
        }
    }
}