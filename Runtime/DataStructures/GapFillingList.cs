#nullable enable
using System;
using System.Collections.Generic;

namespace KadaXuanwu.Utils.Runtime.DataStructures {
    /// <summary>
    /// A list that reuses gaps created by removed items instead of always appending to the end.
    /// When items are removed, their positions become null and can be filled by new items.
    /// This maintains stable indices for remaining items while efficiently reusing memory.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class GapFillingList<T> {
        private readonly List<T?> _internalList = new();

        /// <summary>
        /// Adds an item to the list. If there are any gaps (null positions) from previously removed items,
        /// the new item fills the first available gap. Otherwise, it's appended to the end.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The index where the item was added.</returns>
        public int Add(T item) {
            // Try to find the first null (gap) and fill it
            for (int i = 0; i < _internalList.Count; i++) {
                if (_internalList[i] == null) {
                    _internalList[i] = item;
                    return i; // Return the index where the item was added
                }
            }

            // If no gap is found, add the item at the end
            _internalList.Add(item);
            return _internalList.Count - 1; // Return the index of the newly added item
        }

        /// <summary>
        /// Removes the item at the specified index by setting it to null, creating a gap that can be reused.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when index is outside the valid range.</exception>
        public void RemoveAt(int index) {
            if (index >= 0 && index < _internalList.Count) {
                _internalList[index] = default;
            }
            else {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
        }

        /// <summary>
        /// Removes the first occurrence of the specified item by setting it to null.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was found and removed; false otherwise.</returns>
        public bool Remove(T item) {
            for (int i = 0; i < _internalList.Count; i++) {
                if (_internalList[i] != null && _internalList[i]!.Equals(item)) {
                    _internalList[i] = default;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        /// <returns>The item at the specified index, or null if the position is a gap.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when index is outside the valid range.</exception>
        public T? Get(int index) {
            if (index >= 0 && index < _internalList.Count) {
                return _internalList[index];
            }

            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item at the specified index, or null if the position is a gap.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when index is outside the valid range.</exception>
        public T? this[int index] {
            get {
                if (index >= 0 && index < _internalList.Count) {
                    return _internalList[index];
                }

                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            set {
                if (index >= 0 && index < _internalList.Count) {
                    _internalList[index] = value;
                }
                else {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                }
            }
        }

        /// <summary>
        /// Gets the total count of positions in the list, including both filled positions and gaps.
        /// </summary>
        public int Count => _internalList.Count;

        /// <summary>
        /// Creates a new list containing all items, including null values for gaps.
        /// </summary>
        /// <returns>A new list with all items and gaps.</returns>
        public List<T?> ToList() {
            return new List<T?>(_internalList);
        }

        /// <summary>
        /// Returns a string representation of the list, including gaps as null values.
        /// </summary>
        /// <returns>A comma-separated string of all items and gaps.</returns>
        public override string ToString() {
            return string.Join(", ", _internalList);
        }
    }
}
