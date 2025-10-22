using System.Collections.Generic;

namespace KadaXuanwu.Utils.Runtime.DataStructures {
    /// <summary>
    /// A list that can automatically add or remove elements after a specified time has elapsed.
    /// Elements can be added with an expiration time to auto-remove, or removed with a delay to restore later.
    /// Requires calling Update() each frame with the current time.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class TimedList<T> {
        private readonly List<T> _list = new();
        private readonly Dictionary<T, float> _timestampsAdding = new();
        private readonly Dictionary<T, float> _timestampsRemoving = new();

        /// <summary>
        /// Adds an element to the list immediately without expiration.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void Add(T element) {
            _list.Add(element);
        }

        /// <summary>
        /// Adds an element to the list and schedules it for automatic removal at the specified time.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <param name="expirationTime">The time at which the element should be automatically removed.</param>
        public void Add(T element, float expirationTime) {
            _list.Add(element);
            _timestampsAdding.Add(element, expirationTime);
        }

        /// <summary>
        /// Removes an element from the list immediately.
        /// </summary>
        /// <param name="element">The element to remove.</param>
        /// <returns>True if the element was not in the list and removal failed; false if successfully removed.</returns>
        public bool Remove(T element) {
            if (_list.Contains(element)) {
                return false;
            }

            _list.Remove(element);
            return true;
        }

        /// <summary>
        /// Removes an element from the list and schedules it to be re-added at the specified time.
        /// Useful for temporary removal with automatic restoration.
        /// </summary>
        /// <param name="element">The element to remove.</param>
        /// <param name="expirationTime">The time at which the element should be automatically re-added.</param>
        /// <returns>True if successfully removed; false if the element was not in the list.</returns>
        public bool Remove(T element, float expirationTime) {
            if (!_list.Contains(element)) {
                return false;
            }

            _list.Remove(element);
            _timestampsRemoving.Add(element, expirationTime);
            return true;
        }

        /// <summary>
        /// Gets the underlying list of elements.
        /// </summary>
        /// <returns>The list containing all current elements.</returns>
        public List<T> GetList() {
            return _list;
        }

        /// <summary>
        /// Updates the timed list by checking for expired elements and scheduled restorations.
        /// Must be called regularly (e.g., each frame) with the current time.
        /// </summary>
        /// <param name="currentTime">The current time to compare against scheduled operations.</param>
        public void Update(float currentTime) {
            List<T> itemsToRemove = new();

            foreach (var keyValuePair in _timestampsAdding)
                if (keyValuePair.Value < currentTime) {
                    itemsToRemove.Add(keyValuePair.Key);
                }

            foreach (var itemToRemove in itemsToRemove) {
                _list.Remove(itemToRemove);
                _timestampsAdding.Remove(itemToRemove);
            }

            itemsToRemove.Clear();

            foreach (var keyValuePair in _timestampsRemoving)
                if (keyValuePair.Value < currentTime) {
                    itemsToRemove.Add(keyValuePair.Key);
                }

            foreach (var itemToRemove in itemsToRemove) {
                _list.Add(itemToRemove);
                _timestampsRemoving.Remove(itemToRemove);
            }
        }
    }
}
