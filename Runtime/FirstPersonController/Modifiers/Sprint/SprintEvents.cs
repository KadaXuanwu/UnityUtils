using System;

public class SprintEvents : IModifierEvents {
    /// <summary>
    /// Fired when running state changes.
    /// </summary>
    public event Action<bool> OnRunningChanged;

    public void InvokeRunningChanged(bool isRunning) => OnRunningChanged?.Invoke(isRunning);

    public void ClearSubscribers() {
        OnRunningChanged = null;
    }
}
