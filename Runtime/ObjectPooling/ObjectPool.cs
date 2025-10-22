using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.ObjectPooling {
    /// <summary>
    /// Singleton object pooling system that manages reusable GameObject instances to reduce instantiation overhead.
    /// Supports automatic expansion, delayed return, and tracks active/available objects per pool.
    /// </summary>
    public class ObjectPool : MonoBehaviour {
        /// <summary>
        /// Gets the singleton instance of the ObjectPool.
        /// </summary>
        public static ObjectPool S { get; private set; }

        /// <summary>
        /// Configuration for an individual object pool.
        /// </summary>
        [System.Serializable]
        public class Pool {
            /// <summary>
            /// The prefab to pool.
            /// </summary>
            public GameObject prefab;

            /// <summary>
            /// Number of instances to pre-instantiate at start.
            /// </summary>
            public int initialSize = 0;

            /// <summary>
            /// Number of instances to create when the pool runs out.
            /// </summary>
            public int expandSize = 5;

            /// <summary>
            /// Whether the pool can create new instances when exhausted.
            /// </summary>
            public bool allowExpansion = true;
        }

        /// <summary>
        /// List of pools to initialize at start.
        /// </summary>
        public List<Pool> pools;

        private readonly Dictionary<GameObject, Queue<GameObject>> _poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        private readonly Dictionary<GameObject, Pool> _poolSettings = new Dictionary<GameObject, Pool>();
        private readonly Dictionary<GameObject, GameObject> _activeObjects = new Dictionary<GameObject, GameObject>();
        private readonly Dictionary<GameObject, int> _activeCounts = new Dictionary<GameObject, int>();

        private void Awake() {
            if (S != null && S != this) {
                Destroy(gameObject);
            }
            else {
                S = this;
            }
        }

        private void Start() {
            InitializePools();
        }

        private void InitializePools() {
            if (pools == null) {
                pools = new List<Pool>();
                return;
            }

            foreach (Pool pool in pools) {
                if (pool == null) {
                    continue;
                }

                GameObject prefab = pool.prefab;
                if (prefab == null) {
                    continue;
                }

                if (_poolDictionary.ContainsKey(prefab)) {
                    continue;
                }

                Queue<GameObject> objectPool = new Queue<GameObject>();
                _poolDictionary.Add(prefab, objectPool);
                _poolSettings.Add(prefab, pool);
                _activeCounts[prefab] = 0;

                for (int i = 0; i < pool.initialSize; i++) {
                    GameObject obj = CreatePooledObject(prefab);
                    if (obj != null) {
                        objectPool.Enqueue(obj);
                    }
                }
            }
        }

        private GameObject CreatePooledObject(GameObject prefab) {
            if (prefab == null) {
                return null;
            }

            GameObject obj = Instantiate(prefab);
            if (obj == null) {
                return null;
            }

            obj.SetActive(false);

            PooledObject pooledObj = obj.GetComponent<PooledObject>();
            if (pooledObj == null) {
                pooledObj = obj.AddComponent<PooledObject>();
            }

            pooledObj.SetPool(this, prefab);

            return obj;
        }

        private void ExpandPool(GameObject prefab) {
            if (prefab == null) {
                return;
            }

            if (!_poolSettings.TryGetValue(prefab, out Pool poolSettings) || !poolSettings.allowExpansion) {
                return;
            }

            if (!_poolDictionary.TryGetValue(prefab, out Queue<GameObject> pool)) {
                return;
            }

            for (int i = 0; i < poolSettings.expandSize; i++) {
                GameObject obj = CreatePooledObject(prefab);
                if (obj != null) pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// Spawns an object from the pool at the specified position with default rotation.
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="position">World position to spawn at.</param>
        /// <returns>The spawned GameObject, or null if pool doesn't exist or is exhausted.</returns>
        public GameObject SpawnFromPool(GameObject prefab, Vector3 position) {
            return SpawnFromPool(prefab, position, Quaternion.identity);
        }

        /// <summary>
        /// Spawns an object from the pool at the specified position with automatic return after duration.
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="duration">Time in seconds before automatically returning to pool.</param>
        /// <returns>The spawned GameObject, or null if pool doesn't exist or is exhausted.</returns>
        public GameObject SpawnFromPool(GameObject prefab, Vector3 position, float duration) {
            return SpawnFromPool(prefab, position, Quaternion.identity, duration);
        }

        /// <summary>
        /// Spawns an object from the pool at the specified position and rotation.
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">World rotation to apply.</param>
        /// <returns>The spawned GameObject, or null if pool doesn't exist or is exhausted.</returns>
        public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation) {
            return SpawnFromPoolInternal(prefab, position, rotation, -1f);
        }

        /// <summary>
        /// Spawns an object from the pool at the specified position and rotation with automatic return after duration.
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">World rotation to apply.</param>
        /// <param name="duration">Time in seconds before automatically returning to pool.</param>
        /// <returns>The spawned GameObject, or null if pool doesn't exist or is exhausted.</returns>
        public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation, float duration) {
            return SpawnFromPoolInternal(prefab, position, rotation, duration);
        }

        private GameObject SpawnFromPoolInternal(GameObject prefab, Vector3 position, Quaternion rotation, float duration) {
            if (prefab == null) {
                Debug.LogWarning("SpawnFromPool called with null prefab.");
                return null;
            }

            if (!_poolDictionary.TryGetValue(prefab, out Queue<GameObject> pool)) {
                Debug.LogWarning($"Pool for prefab {prefab.name} doesn't exist!");
                return null;
            }

            GameObject objectToSpawn = null;
            while (pool.Count > 0) {
                objectToSpawn = pool.Dequeue();
                if (objectToSpawn != null) break;
            }

            if (objectToSpawn == null) {
                ExpandPool(prefab);
                while (pool.Count > 0) {
                    objectToSpawn = pool.Dequeue();
                    if (objectToSpawn != null) break;
                }

                if (objectToSpawn == null) {
                    Debug.LogWarning($"Pool for {prefab.name} is exhausted and cannot expand!");
                    return null;
                }
            }

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.SetPositionAndRotation(position, rotation);

            _activeObjects[objectToSpawn] = prefab;
            _activeCounts.TryGetValue(prefab, out int count);
            _activeCounts[prefab] = count + 1;

            if (duration > 0f) {
                StartCoroutine(ReturnToPoolAfterDelay(objectToSpawn, duration));
            }

            return objectToSpawn;
        }

        /// <summary>
        /// Returns an object to the pool immediately, making it available for reuse.
        /// </summary>
        /// <param name="obj">The GameObject to return.</param>
        public void ReturnToPool(GameObject obj) {
            if (obj == null) return;
            if (!_activeObjects.TryGetValue(obj, out GameObject prefab)) {
                Debug.LogWarning($"Trying to return object {obj.name} that wasn't spawned from pool!");
                SafeDeactivate(obj);
                return;
            }

            _activeObjects.Remove(obj);
            if (_activeCounts.TryGetValue(prefab, out int count)) {
                _activeCounts[prefab] = Mathf.Max(0, count - 1);
            }

            SafeDeactivate(obj);

            if (_poolDictionary.TryGetValue(prefab, out Queue<GameObject> pool)) {
                pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// Returns an object to the pool after a delay.
        /// </summary>
        /// <param name="obj">The GameObject to return.</param>
        /// <param name="delay">Time in seconds before returning to pool.</param>
        public void ReturnToPool(GameObject obj, float delay) {
            if (obj == null || delay <= 0f) {
                ReturnToPool(obj);
                return;
            }

            StartCoroutine(ReturnToPoolAfterDelay(obj, delay));
        }

        private IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay) {
            yield return new WaitForSeconds(delay);
            ReturnToPool(obj);
        }

        private void SafeDeactivate(GameObject obj) {
            if (obj == null) {
                return;
            }

            if (obj.activeInHierarchy) {
                obj.SetActive(false);
            }

            obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Deactivates a GameObject by returning it to the pool.
        /// Alias for ReturnToPool().
        /// </summary>
        /// <param name="obj">The GameObject to deactivate.</param>
        public void DeactivateGameObject(GameObject obj) {
            ReturnToPool(obj);
        }

        /// <summary>
        /// Logs statistics for all pools including available, active, and total counts.
        /// </summary>
        public void LogPoolStats() {
            foreach (var kvp in _poolDictionary) {
                GameObject prefab = kvp.Key;
                int available = kvp.Value?.Count ?? 0;
                _activeCounts.TryGetValue(prefab, out int active);
                int total = available + active;
                Debug.Log($"Pool {prefab.name}: available={available}, active={active}, total={total}");
            }
        }

        /// <summary>
        /// Gets pool statistics for a specific prefab.
        /// </summary>
        /// <param name="prefab">The prefab to get stats for.</param>
        /// <returns>Tuple containing (available, active, total) counts.</returns>
        public (int available, int active, int total) GetPoolInfo(GameObject prefab) {
            if (prefab == null) {
                return (0, 0, 0);
            }

            int available = _poolDictionary.TryGetValue(prefab, out Queue<GameObject> pool) ? pool.Count : 0;
            _activeCounts.TryGetValue(prefab, out int active);
            return (available, active, available + active);
        }

        internal void NotifyDestroyed(GameObject obj) {
            if (obj == null) {
                return;
            }

            if (_activeObjects.TryGetValue(obj, out GameObject prefab)) {
                _activeObjects.Remove(obj);
                if (_activeCounts.TryGetValue(prefab, out int count)) _activeCounts[prefab] = Mathf.Max(0, count - 1);
            }
        }
    }

    /// <summary>
    /// Component automatically added to pooled objects to manage their lifecycle.
    /// Provides a convenience method to return the object to its pool.
    /// </summary>
    public class PooledObject : MonoBehaviour {
        private ObjectPool _pool;
        private GameObject _prefab;

        /// <summary>
        /// Sets the pool and prefab reference for this pooled object.
        /// Called automatically by ObjectPool during object creation.
        /// </summary>
        /// <param name="pool">The ObjectPool managing this object.</param>
        /// <param name="prefab">The original prefab this object was created from.</param>
        public void SetPool(ObjectPool pool, GameObject prefab) {
            _pool = pool;
            _prefab = prefab;
        }

        /// <summary>
        /// Returns this object to its pool for reuse.
        /// </summary>
        public void ReturnToPool() {
            if (_pool != null) _pool.ReturnToPool(gameObject);
        }

        private void OnDestroy() {
            if (_pool != null) _pool.NotifyDestroyed(gameObject);
        }
    }
}
