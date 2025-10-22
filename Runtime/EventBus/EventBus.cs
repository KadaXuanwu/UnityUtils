using System.Collections.Generic;

namespace KadaXuanwu.Utils.Runtime.EventBus {
    /// <summary>
    /// A static event bus that provides a centralized messaging system for a specific event type.
    /// Allows decoupled communication between different parts of the application without direct references.
    /// Each event type T has its own independent event bus instance.
    /// </summary>
    /// <typeparam name="T">The event type that implements IEvent.</typeparam>
    public static class EventBus<T> where T : IEvent {
        private static readonly HashSet<IEventBinding<T>> Bindings = new();

        /// <summary>
        /// Registers an event binding to receive notifications when events of type T are raised.
        /// </summary>
        /// <param name="binding">The event binding to register.</param>
        public static void Register(EventBinding<T> binding) => Bindings.Add(binding);

        /// <summary>
        /// Unregisters an event binding so it no longer receives event notifications.
        /// </summary>
        /// <param name="binding">The event binding to unregister.</param>
        public static void Unregister(EventBinding<T> binding) => Bindings.Remove(binding);

        /// <summary>
        /// Raises an event, notifying all registered bindings.
        /// Creates a copy of the bindings collection to allow safe modification during iteration.
        /// Invokes both parameterized and parameterless callbacks for each binding.
        /// </summary>
        /// <param name="event">The event data to broadcast to all listeners.</param>
        public static void Raise(T @event) {
            var bindingsBuffer = new HashSet<IEventBinding<T>>(Bindings);
            foreach (var binding in bindingsBuffer) {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }

        /// <summary>
        /// Clears all registered bindings from this event bus.
        /// Called automatically when exiting play mode in the editor.
        /// </summary>
        private static void Clear() {
            Bindings.Clear();
        }
    }
}
