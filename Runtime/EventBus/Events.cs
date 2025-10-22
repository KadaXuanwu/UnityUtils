namespace KadaXuanwu.Utils.Runtime.EventBus {
    /// <summary>
    /// Marker interface for all event types used with the EventBus system.
    /// Implement this interface on structs to create custom events.
    /// Events are typically implemented as structs to avoid heap allocations.
    /// </summary>
    public interface IEvent { }

    // Usage example
    /*
    public struct ClientLeft : IEvent
    {
        /// <summary>
        /// The unique identifier of the client that left.
        /// </summary>
        public ulong ClientId;
    }
    */
}
