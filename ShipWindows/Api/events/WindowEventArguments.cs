namespace ShipWindows.Api.events;

public struct WindowEventArguments(WindowInfo windowInfo) {
    public bool cancelled = false;
    public WindowInfo windowInfo = windowInfo;
}