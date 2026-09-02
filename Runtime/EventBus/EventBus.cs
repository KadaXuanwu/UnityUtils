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
        private static readonly Stack<List<IEventBinding<T>>> BufferPool = new();

        /// <summary>
        /// Announces this bus to <see cref="EventBusUtil"/> the first time anything touches it, so
        /// that ClearAllBuses can reach it.
        /// This replaces scanning for IEvent types up front, which only ever looked inside
        /// Assembly-CSharp and Assembly-CSharp-firstpass and therefore missed every event type
        /// declared behind an asmdef. Registering on first use is also more accurate: a bus nobody
        /// has touched holds no bindings and has nothing to clear.
        /// </summary>
        static EventBus() {
            EventBusUtil.RegisterBus(typeof(T), typeof(EventBus<T>), Clear);
        }

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
        /// Raises an event, invoking both the parameterized and parameterless callback of every
        /// registered binding.
        ///
        /// Iterates a snapshot, because a handler is allowed to register or unregister while the
        /// event is being delivered. The snapshot is a pooled list rather than a fresh collection:
        /// raising used to allocate a whole HashSet every call, which is the kind of per-frame
        /// garbage that shows up as a hitch under Unity's non-generational collector.
        ///
        /// Unregistering during a raise takes effect immediately - a binding removed by an earlier
        /// handler is skipped rather than still being called. The alternative matches C# multicast
        /// delegates, but here the usual shape is a handler destroying an object whose teardown
        /// unregisters it, and calling that binding anyway means running a handler on a dead
        /// object. Registering during a raise takes effect on the next one.
        /// </summary>
        /// <param name="event">The event data to broadcast to all listeners.</param>
        public static void Raise(T @event) {
            var buffer = RentBuffer();
            try {
                buffer.AddRange(Bindings);
                for (var i = 0; i < buffer.Count; i++) {
                    var binding = buffer[i];
                    if (!Bindings.Contains(binding)) {
                        continue;
                    }

                    binding.OnEvent.Invoke(@event);
                    binding.OnEventNoArgs.Invoke();
                }
            }
            finally {
                ReturnBuffer(buffer);
            }
        }

        /// <summary>
        /// A raise nested inside a handler rents its own buffer, so re-entrancy needs no special
        /// case. The pool therefore only ever grows to the deepest nesting actually reached.
        /// </summary>
        private static List<IEventBinding<T>> RentBuffer() {
            return BufferPool.Count > 0 ? BufferPool.Pop() : new List<IEventBinding<T>>();
        }

        /// <summary>
        /// Cleared on the way back in, so a pooled buffer never keeps a binding - and whatever it
        /// captured - alive after the raise that used it.
        /// </summary>
        private static void ReturnBuffer(List<IEventBinding<T>> buffer) {
            buffer.Clear();
            BufferPool.Push(buffer);
        }

        /// <summary>
        /// Clears all registered bindings from this event bus.
        /// Called for every known bus by <see cref="EventBusUtil.ClearAllBuses"/> when play mode ends.
        /// </summary>
        public static void Clear() {
            Bindings.Clear();
        }
    }
}
