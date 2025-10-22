using System;

namespace KadaXuanwu.Utils.Runtime.EventBus {
    /// <summary>
    /// Interface for event bindings that handle event callbacks.
    /// Supports both parameterized callbacks (with event data) and parameterless callbacks.
    /// </summary>
    /// <typeparam name="T">The event type that implements IEvent.</typeparam>
    public interface IEventBinding<T> {
        /// <summary>
        /// Gets or sets the action to invoke when the event is raised with event data.
        /// </summary>
        public Action<T> OnEvent { get; set; }

        /// <summary>
        /// Gets or sets the action to invoke when the event is raised without passing event data.
        /// </summary>
        public Action OnEventNoArgs { get; set; }
    }

    /// <summary>
    /// Represents a binding between an event and its callback handlers.
    /// Allows subscribing to events with either parameterized or parameterless callbacks.
    /// </summary>
    /// <typeparam name="T">The event type that implements IEvent.</typeparam>
    public class EventBinding<T> : IEventBinding<T> where T : IEvent {
        private Action<T> _onEvent = _ => { };
        private Action _onEventNoArgs = () => { };

        Action<T> IEventBinding<T>.OnEvent {
            get => _onEvent;
            set => _onEvent = value;
        }

        Action IEventBinding<T>.OnEventNoArgs {
            get => _onEventNoArgs;
            set => _onEventNoArgs = value;
        }

        /// <summary>
        /// Initializes a new event binding with a parameterized callback.
        /// </summary>
        /// <param name="onEvent">The action to invoke with event data.</param>
        public EventBinding(Action<T> onEvent) => _onEvent = onEvent;

        /// <summary>
        /// Initializes a new event binding with a parameterless callback.
        /// </summary>
        /// <param name="onEventNoArgs">The action to invoke without event data.</param>
        public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs;

        /// <summary>
        /// Adds a parameterless callback to this binding.
        /// </summary>
        /// <param name="onEvent">The action to add.</param>
        public void Add(Action onEvent) => _onEventNoArgs += onEvent;

        /// <summary>
        /// Removes a parameterless callback from this binding.
        /// </summary>
        /// <param name="onEvent">The action to remove.</param>
        public void Remove(Action onEvent) => _onEventNoArgs -= onEvent;

        /// <summary>
        /// Adds a parameterized callback to this binding.
        /// </summary>
        /// <param name="onEvent">The action to add.</param>
        public void Add(Action<T> onEvent) => _onEvent += onEvent;

        /// <summary>
        /// Removes a parameterized callback from this binding.
        /// </summary>
        /// <param name="onEvent">The action to remove.</param>
        public void Remove(Action<T> onEvent) => _onEvent -= onEvent;
    }
}
