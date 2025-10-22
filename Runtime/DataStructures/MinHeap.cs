using System;
using System.Collections.Generic;

namespace KadaXuanwu.Utils.Runtime.DataStructures {
    /// <summary>
    /// A min heap data structure where the smallest element is always at the root.
    /// Provides O(log n) insertion and extraction, and O(1) peek operations.
    /// Elements must implement IComparable for ordering.
    /// </summary>
    /// <typeparam name="T">The type of elements in the heap. Must implement IComparable.</typeparam>
    public class MinHeap<T> where T : IComparable<T> {
        /// <summary>
        /// Gets the number of elements currently in the heap.
        /// </summary>
        public int Count => _heap.Count;

        private readonly List<T> _heap = new();

        /// <summary>
        /// Inserts a new value into the heap while maintaining the min heap property.
        /// Time complexity: O(log n).
        /// </summary>
        /// <param name="value">The value to insert.</param>
        public void Insert(T value) {
            _heap.Add(value);
            HeapifyUp(_heap.Count - 1);
        }

        /// <summary>
        /// Removes and returns the minimum element (root) from the heap.
        /// Time complexity: O(log n).
        /// </summary>
        /// <returns>The minimum element, or default(T) if the heap is empty.</returns>
        public T ExtractMin() {
            if (_heap.Count == 0) {
                return default;
            }

            T min = _heap[0];
            _heap[0] = _heap[^1];
            _heap.RemoveAt(_heap.Count - 1);
            HeapifyDown(0);
            return min;
        }

        /// <summary>
        /// Returns the minimum element without removing it from the heap.
        /// Time complexity: O(1).
        /// </summary>
        /// <returns>The minimum element, or default(T) if the heap is empty.</returns>
        public T PeekMin() {
            if (_heap.Count == 0) {
                return default;
            }

            return _heap[0];
        }

        /// <summary>
        /// Removes the first occurrence of the specified value from the heap.
        /// Time complexity: O(n) for search, O(log n) for removal.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        public void Remove(T value) {
            int index = _heap.IndexOf(value);
            if (index == -1) {
                return;
            }

            _heap[index] = _heap[^1];
            _heap.RemoveAt(_heap.Count - 1);

            if (index < _heap.Count) {
                HeapifyUp(index);
                HeapifyDown(index);
            }
        }

        /// <summary>
        /// Removes all elements from the heap.
        /// </summary>
        public void Clear() {
            _heap.Clear();
        }

        private void HeapifyUp(int index) {
            while (index > 0) {
                int parentIndex = (index - 1) / 2;
                if (_heap[index].CompareTo(_heap[parentIndex]) >= 0) {
                    break;
                }

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index) {
            while (index < _heap.Count / 2) {
                int leftChildIndex = 2 * index + 1;
                int rightChildIndex = 2 * index + 2;
                int smallestIndex = index;

                if (leftChildIndex < _heap.Count && _heap[leftChildIndex].CompareTo(_heap[smallestIndex]) < 0) {
                    smallestIndex = leftChildIndex;
                }

                if (rightChildIndex < _heap.Count && _heap[rightChildIndex].CompareTo(_heap[smallestIndex]) < 0) {
                    smallestIndex = rightChildIndex;
                }

                if (smallestIndex == index) {
                    break;
                }

                Swap(index, smallestIndex);
                index = smallestIndex;
            }
        }

        private void Swap(int index1, int index2) {
            (_heap[index1], _heap[index2]) = (_heap[index2], _heap[index1]);
        }
    }
}
