using UnityEngine;

namespace ShipWindows.Api.events;

public class EventAPI {
    public delegate void WindowEvent(WindowEventArguments windowEvent);

    internal static WindowEventArguments BeforeWindowSpawn(WindowInfo windowInfo) {
        var windowEvent = new WindowEventArguments(windowInfo);

        BeforeWindowSpawned?.Invoke(windowEvent);

        return windowEvent;
    }

    [Tooltip("Cancelling this event is obviously not possible.")]
    internal static WindowEventArguments AfterWindowSpawn(WindowInfo windowInfo) {
        var windowEvent = new WindowEventArguments(windowInfo);

        AfterWindowSpawned?.Invoke(windowEvent);

        return windowEvent;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static event WindowEvent BeforeWindowSpawned;
    public static event WindowEvent AfterWindowSpawned;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}