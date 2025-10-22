using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.Systems {
    /// <summary>
    /// Generic manager for temporary or permanent modifiers identified by string keys.
    /// Supports automatic expiration and provides event notification when modifiers change.
    /// Useful for buff/debuff systems, status effects, or feature flags.
    /// </summary>
    /// <typeparam name="T">The type of modifier value to store.</typeparam>
    public class ModifierManager<T> {
        /// <summary>
        /// Gets a read-only view of all active modifiers.
        /// </summary>
        public IReadOnlyDictionary<string, T> Modifiers => _modifiers;

        /// <summary>
        /// Event raised whenever modifiers are added, removed, or reset.
        /// </summary>
        public event Action ModifiersChanged;

        private readonly Dictionary<string, T> _modifiers = new();

        /// <summary>
        /// Adds a permanent modifier with the specified key.
        /// If the key already exists, no action is taken.
        /// </summary>
        /// <param name="key">Unique identifier for the modifier.</param>
        /// <param name="value">The modifier value to store.</param>
        public void AddModifier(string key, T value) {
            if (_modifiers.TryAdd(key, value)) {
                ModifiersChanged?.Invoke();
            }
        }

        /// <summary>
        /// Adds a temporary modifier that automatically expires after the specified duration.
        /// If the key already exists, no action is taken.
        /// </summary>
        /// <param name="key">Unique identifier for the modifier.</param>
        /// <param name="value">The modifier value to store.</param>
        /// <param name="duration">Time in seconds before the modifier is automatically removed.</param>
        public void AddModifier(string key, T value, float duration) {
            if (_modifiers.TryAdd(key, value)) {
                ModifiersChanged?.Invoke();
                CoroutineRunner.StartNew(CoroutineRemoveModifier(key, duration));
            }
        }

        /// <summary>
        /// Removes a modifier immediately by key.
        /// </summary>
        /// <param name="key">The modifier key to remove.</param>
        public void RemoveModifier(string key) {
            if (_modifiers.ContainsKey(key)) {
                _modifiers.Remove(key);
                ModifiersChanged?.Invoke();
            }
        }

        /// <summary>
        /// Schedules a modifier for removal after a delay.
        /// Only schedules removal if the key currently exists.
        /// </summary>
        /// <param name="key">The modifier key to remove.</param>
        /// <param name="delay">Time in seconds before removal.</param>
        public void RemoveModifier(string key, float delay) {
            if (_modifiers.ContainsKey(key)) {
                CoroutineRunner.StartNew(CoroutineRemoveModifier(key, delay));
            }
        }

        /// <summary>
        /// Removes all modifiers and raises the ModifiersChanged event.
        /// </summary>
        public void ResetModifiers() {
            _modifiers.Clear();
            ModifiersChanged?.Invoke();
        }

        private IEnumerator CoroutineRemoveModifier(string key, float delay) {
            yield return new WaitForSeconds(delay);

            RemoveModifier(key);
        }

        internal void AddModifier(object keyDisableEffects, bool v) {
            throw new NotImplementedException();
        }
    }
}
