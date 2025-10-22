using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.DataStructures {
    /// <summary>
    /// Base class for implementing the Singleton pattern in Unity.
    /// Inherit from this class to create a singleton MonoBehaviour.
    /// </summary>
    /// <typeparam name="T">The type of the singleton class</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _s;

        /// <summary>
        /// Global access point to the singleton instance
        /// </summary>
        public static T S {
            get {
                if (_s == null) {
                    _s = FindFirstObjectByType<T>();

                    if (_s == null) {
                        GameObject singletonObject = new();
                        _s = singletonObject.AddComponent<T>();
                        singletonObject.name = $"{typeof(T)} (Singleton)";
                    }
                }

                return _s;
            }
        }

        protected virtual void Awake() {
            if (_s == null) {
                _s = this as T;
            }
            else if (_s != this) {
                Debug.LogWarning($"[Singleton] Another instance of {typeof(T)} already exists! Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy() {
            if (_s == this) {
                _s = null;
            }
        }
    }
}
