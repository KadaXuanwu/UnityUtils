using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KadaXuanwu.Utils.Runtime.EventBus {
    /// <summary>
    /// Keeps track of every <see cref="EventBus{T}"/> that has actually been used, so all of them
    /// can be cleared at once when play mode ends.
    ///
    /// Buses announce themselves from their static constructor rather than being discovered by
    /// reflection. The previous approach asked PredefinedAssemblyUtil for IEvent types, which by
    /// design only looks in Assembly-CSharp and Assembly-CSharp-firstpass - so any event type
    /// declared inside an asmdef was never found, and its bindings survived play mode with
    /// references to destroyed objects still in them.
    /// </summary>
    public static class EventBusUtil {
        private static readonly List<Type> RegisteredEventTypes = new();
        private static readonly List<Type> RegisteredBusTypes = new();
        private static readonly List<Action> ClearActions = new();

        /// <summary>
        /// The event types that currently have a live bus, in the order the buses were first used.
        /// A type only appears here once something has registered, raised or cleared its bus.
        /// </summary>
        public static IReadOnlyList<Type> EventTypes => RegisteredEventTypes;

        /// <summary>
        /// The closed EventBus types matching <see cref="EventTypes"/>, in the same order.
        /// </summary>
        public static IReadOnlyList<Type> EventBusTypes => RegisteredBusTypes;

#if UNITY_EDITOR
        public static PlayModeStateChange PlayModeState { get; set; }

        /// <summary>
        /// Initializes the Unity Editor related components of the EventBusUtil.
        /// The [InitializeOnLoadMethod] attribute causes this method to be called every time a script
        /// is loaded or when the game enters Play Mode in the Editor. This is useful to initialize
        /// fields or states of the class that are necessary during the editing state that also apply
        /// when the game enters Play Mode.
        /// The method sets up a subscriber to the playModeStateChanged event to allow
        /// actions to be performed when the Editor's play mode changes.
        /// </summary>
        [InitializeOnLoadMethod]
        public static void InitializeEditor() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state) {
            PlayModeState = state;
            if (state == PlayModeStateChange.ExitingPlayMode) {
                ClearAllBuses();
            }
        }
#endif

        /// <summary>
        /// Called by <see cref="EventBus{T}"/> the first time that bus is touched. Not part of the
        /// public surface: buses register themselves and nothing else should.
        /// </summary>
        /// <param name="eventType">The event type the bus carries.</param>
        /// <param name="busType">The closed EventBus type.</param>
        /// <param name="clear">Clears that bus's bindings.</param>
        internal static void RegisterBus(Type eventType, Type busType, Action clear) {
            RegisteredEventTypes.Add(eventType);
            RegisteredBusTypes.Add(busType);
            ClearActions.Add(clear);
        }

        /// <summary>
        /// Runs before any scene loads, in both Play Mode and a build. With domain reloading on
        /// there is nothing to do - the statics are already fresh - but with Enter Play Mode
        /// Options set to skip the reload, the buses survive from the previous session, and this is
        /// what guarantees a run always starts with no listeners.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize() {
            ClearAllBuses();
        }

        /// <summary>
        /// Clears (removes all listeners from) every event bus that has been used.
        /// </summary>
        public static void ClearAllBuses() {
            for (int i = 0; i < ClearActions.Count; i++) {
                ClearActions[i].Invoke();
            }
        }
    }
}
