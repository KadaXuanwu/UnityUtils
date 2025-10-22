using System;

namespace KadaXuanwu.Utils.Runtime.DataStructures {
    /// <summary>
    /// A struct that represents a float value constrained to the range [0, 1].
    /// Automatically clamps any assigned value to stay within valid bounds.
    /// Useful for representing percentages, ratios, or normalized values.
    /// </summary>
    public struct NormalizedFloat {
        private float _value;

        /// <summary>
        /// Initializes a new instance with the specified value, clamped to [0, 1].
        /// </summary>
        /// <param name="value">The initial value to clamp and store.</param>
        public NormalizedFloat(float value) {
            _value = Clamp(value);
        }

        /// <summary>
        /// Gets or sets the normalized value. The setter automatically clamps to [0, 1].
        /// </summary>
        public float Value {
            readonly get => _value;
            set => _value = Clamp(value);
        }

        private static float Clamp(float value) => MathF.Max(0f, MathF.Min(1f, value));

        /// <summary>
        /// Implicitly converts a float to a NormalizedFloat, clamping the value to [0, 1].
        /// </summary>
        /// <param name="value">The float value to convert.</param>
        public static implicit operator NormalizedFloat(float value) => new(value);

        /// <summary>
        /// Implicitly converts a NormalizedFloat to a float, returning the clamped value.
        /// </summary>
        /// <param name="normalizedFloat">The NormalizedFloat to convert.</param>
        public static implicit operator float(NormalizedFloat normalizedFloat) => normalizedFloat._value;
    }
}
