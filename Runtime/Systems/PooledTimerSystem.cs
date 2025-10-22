using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace KadaXuanwu.Utils.Runtime.Systems {
    /// <summary>
    /// High-performance timer system using object pooling and a sorted linked list for efficient timer management.
    /// Processes timers in O(1) best case and supports keyed timers that can be updated or cancelled.
    /// Ideal for handling many simultaneous timers (e.g., cooldowns, buffs, delayed actions).
    /// </summary>
    public class PooledTimerSystem : MonoBehaviour {
        private class TimerNode {
            public Action OnComplete;
            public float TargetTime;
            public TimerNode Next;
            public bool IsActive;
            public string Key;
        }

        private ObjectPool<TimerNode> _timerPool;
        private TimerNode _activeTimers;
        private readonly HashSet<string> _activeTimerKeys = new();

        private void Awake() {
            _timerPool = new ObjectPool<TimerNode>(
                createFunc: () => new TimerNode(),
                actionOnGet: (timer) => timer.IsActive = true,
                actionOnRelease: (timer) => {
                    timer.IsActive = false;
                    timer.Next = null;
                    timer.OnComplete = null;
                },
                defaultCapacity: 100
            );
        }

        private void Update() {
            float currentTime = Time.time;

            while (_activeTimers != null && _activeTimers.TargetTime <= currentTime) {
                var timer = _activeTimers;
                _activeTimers = _activeTimers.Next;

                if (timer.IsActive) {
                    _activeTimerKeys.Remove(timer.Key);
                    timer.OnComplete?.Invoke();
                }

                _timerPool.Release(timer);
            }
        }

        /// <summary>
        /// Adds a keyed timer that can be updated or cancelled.
        /// If a timer with the same key exists, it will be updated with the new duration.
        /// </summary>
        /// <param name="key">Unique identifier for the timer.</param>
        /// <param name="duration">Time in seconds until the timer completes.</param>
        /// <param name="immediateAction">Action to invoke immediately when the timer is added/updated.</param>
        /// <param name="onComplete">Action to invoke when the timer expires.</param>
        /// <param name="setDuration">Optional callback to set duration on a component (e.g., UI slider).</param>
        public void AddTimer(string key, float duration, Action immediateAction, Action onComplete,
            Action<float> setDuration = null) {
            // If we already have this timer active, update its duration
            if (!_activeTimerKeys.Add(key)) {
                UpdateExistingTimer(key, duration, onComplete, setDuration);
                return;
            }

            // Call the immediate action
            immediateAction?.Invoke();

            // Set the duration if needed
            setDuration?.Invoke(duration);

            // Add timer
            AddNewTimer(key, duration, onComplete);
        }

        /// <summary>
        /// Adds an anonymous timer without a key. Cannot be cancelled or updated.
        /// </summary>
        /// <param name="duration">Time in seconds until the timer completes.</param>
        /// <param name="immediateAction">Action to invoke immediately when the timer is added.</param>
        /// <param name="onComplete">Action to invoke when the timer expires.</param>
        /// <param name="setDuration">Optional callback to set duration on a component (e.g., UI slider).</param>
        public void AddTimer(float duration, Action immediateAction, Action onComplete,
            Action<float> setDuration = null) {
            // Call the immediate action
            immediateAction?.Invoke();

            // Set the duration if needed
            setDuration?.Invoke(duration);

            // Add timer without a key
            AddNewTimerWithoutKey(duration, onComplete);
        }

        /// <summary>
        /// Cancels a timer by key, preventing its completion callback from executing.
        /// </summary>
        /// <param name="key">The unique identifier of the timer to cancel.</param>
        public void CancelTimer(string key) {
            var current = _activeTimers;
            TimerNode prev = null;

            while (current != null) {
                if (current.Key == key) {
                    if (prev == null) {
                        _activeTimers = current.Next;
                    }
                    else {
                        prev.Next = current.Next;
                    }

                    _activeTimerKeys.Remove(key);
                    _timerPool.Release(current);
                    break;
                }

                prev = current;
                current = current.Next;
            }
        }

        private void AddNewTimer(string key, float duration, Action onComplete) {
            var timer = _timerPool.Get();
            timer.Key = key;
            timer.TargetTime = Time.time + duration;
            timer.OnComplete = onComplete;

            // Insert into sorted linked list
            if (_activeTimers == null || _activeTimers.TargetTime > timer.TargetTime) {
                timer.Next = _activeTimers;
                _activeTimers = timer;
                return;
            }

            var current = _activeTimers;
            while (current.Next != null && current.Next.TargetTime <= timer.TargetTime) {
                current = current.Next;
            }

            timer.Next = current.Next;
            current.Next = timer;
        }

        private void UpdateExistingTimer(string key, float newDuration, Action onComplete,
            Action<float> setDuration = null) {
            // Remove existing timer
            var current = _activeTimers;
            TimerNode prev = null;

            while (current != null) {
                if (current.Key == key) {
                    if (prev == null) {
                        _activeTimers = current.Next;
                    }
                    else {
                        prev.Next = current.Next;
                    }

                    _timerPool.Release(current);
                    break;
                }

                prev = current;
                current = current.Next;
            }

            // Set new duration if needed
            setDuration?.Invoke(newDuration);

            // Add new timer
            AddNewTimer(key, newDuration, onComplete);
        }

        private void AddNewTimerWithoutKey(float duration, Action onComplete) {
            var timer = _timerPool.Get();
            timer.TargetTime = Time.time + duration;
            timer.OnComplete = onComplete;

            // Insert into sorted linked list
            if (_activeTimers == null || _activeTimers.TargetTime > timer.TargetTime) {
                timer.Next = _activeTimers;
                _activeTimers = timer;
                return;
            }

            var current = _activeTimers;
            while (current.Next != null && current.Next.TargetTime <= timer.TargetTime) {
                current = current.Next;
            }

            timer.Next = current.Next;
            current.Next = timer;
        }
    }
}
