using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KadaXuanwu.Utils.Runtime.DataStructures {
    /// <summary>
    /// A bidirectional dictionary that maintains mappings between keys and values in both directions.
    /// Allows efficient lookup by either key or value. Both keys and values must be unique.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class BiDirectionalDictionary<TKey, TValue> : ICloneable, IEnumerable<KeyValuePair<TKey, TValue>> {
        private readonly Dictionary<TKey, TValue> _keyToValue = new();
        private readonly Dictionary<TValue, TKey> _valueToKey = new();

        /// <summary>
        /// Gets a list of all values in the dictionary.
        /// </summary>
        public IList<TValue> Values => _valueToKey.Keys.ToList();

        /// <summary>
        /// Gets a list of all keys in the dictionary.
        /// </summary>
        public IList<TKey> Keys => _keyToValue.Keys.ToList();

        /// <summary>
        /// Gets the number of key-value pairs in the dictionary.
        /// </summary>
        public int Count => _keyToValue.Count;

        /// <summary>
        /// Creates a shallow copy of the bidirectional dictionary.
        /// </summary>
        /// <returns>A new BiDirectionalDictionary instance with the same key-value pairs.</returns>
        public object Clone() {
            BiDirectionalDictionary<TKey, TValue> clonedDictionary = new();

            foreach (KeyValuePair<TKey, TValue> pair in _keyToValue) {
                clonedDictionary.Add(pair.Key, pair.Value);
            }

            return clonedDictionary;
        }

        /// <summary>
        /// Retrieves the key associated with the specified value.
        /// </summary>
        /// <param name="value">The value to look up.</param>
        /// <returns>The key associated with the value, or default(TKey) if not found.</returns>
        public TKey GetKey(TValue value) {
            return _valueToKey.GetValueOrDefault(value);
        }

        /// <summary>
        /// Retrieves the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with the key, or default(TValue) if not found.</returns>
        public TValue GetValue(TKey key) {
            return _keyToValue.GetValueOrDefault(key);
        }

        /// <summary>
        /// Adds a key-value pair to the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>True if the pair was added successfully; false if the key or value already exists.</returns>
        public bool Add(TKey key, TValue value) {
            if (_keyToValue.ContainsKey(key)) {
                return false;
            }

            if (_valueToKey.ContainsKey(value)) {
                return false;
            }

            _keyToValue.Add(key, value);
            _valueToKey.Add(value, key);
            return true;
        }

        /// <summary>
        /// Updates the key associated with a specific value.
        /// </summary>
        /// <param name="value">The value whose key should be updated.</param>
        /// <param name="newKey">The new key to associate with the value.</param>
        /// <returns>True if the key was updated successfully; false if the value doesn't exist or the new key is already in use.</returns>
        public bool UpdateKey(TValue value, TKey newKey) {
            if (!_valueToKey.TryGetValue(value, out TKey oldKey)) {
                return false;
            }

            if (_keyToValue.ContainsKey(newKey)) {
                return false;
            }

            _keyToValue.Remove(oldKey);
            _valueToKey[value] = newKey;
            _keyToValue[newKey] = value;
            return true;
        }

        /// <summary>
        /// Updates the value associated with a specific key.
        /// </summary>
        /// <param name="key">The key whose value should be updated.</param>
        /// <param name="newValue">The new value to associate with the key.</param>
        /// <returns>True if the value was updated successfully; false if the key doesn't exist or the new value is already in use.</returns>
        public bool UpdateValue(TKey key, TValue newValue) {
            if (!_keyToValue.TryGetValue(key, out TValue oldValue)) {
                return false;
            }

            if (_valueToKey.ContainsKey(newValue)) {
                return false;
            }

            _valueToKey.Remove(oldValue);
            _keyToValue[key] = newValue;
            _valueToKey[newValue] = key;
            return true;
        }

        /// <summary>
        /// Removes a key-value pair from the dictionary by key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True if the pair was removed successfully; false if the key doesn't exist.</returns>
        public bool RemoveByKey(TKey key) {
            if (!_keyToValue.TryGetValue(key, out TValue value)) {
                return false;
            }

            _keyToValue.Remove(key);
            _valueToKey.Remove(value);
            return true;
        }

        /// <summary>
        /// Removes a key-value pair from the dictionary by value.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>True if the pair was removed successfully; false if the value doesn't exist.</returns>
        public bool RemoveByValue(TValue value) {
            if (!_valueToKey.TryGetValue(value, out TKey key)) {
                return false;
            }

            _valueToKey.Remove(value);
            _keyToValue.Remove(key);
            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the key-value pairs.
        /// </summary>
        /// <returns>An enumerator for the dictionary.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _keyToValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
