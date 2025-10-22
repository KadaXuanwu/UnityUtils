using System.Collections;
using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.Systems {
    /// <summary>
    /// Singleton MonoBehaviour that allows starting coroutines from non-MonoBehaviour classes.
    /// Provides static access to Unity's coroutine system for use anywhere in the codebase.
    /// </summary>
    public class CoroutineRunner : MonoBehaviour {
        private static CoroutineRunner S { get; set; }

        private void Awake() {
            if (S != null && S != this) {
                Destroy(gameObject);
            }
            else {
                S = this;
            }
        }

        /// <summary>
        /// Starts a new coroutine from any class.
        /// </summary>
        /// <param name="routine">The IEnumerator coroutine to start.</param>
        /// <returns>A Coroutine reference that can be used to stop the coroutine.</returns>
        public static Coroutine StartNew(IEnumerator routine) {
            return S.StartCoroutine(routine);
        }

        /// <summary>
        /// Stops a running coroutine.
        /// </summary>
        /// <param name="coroutine">The Coroutine reference to stop.</param>
        public static void Stop(Coroutine coroutine) {
            S.StopCoroutine(coroutine);
        }
    }
}
