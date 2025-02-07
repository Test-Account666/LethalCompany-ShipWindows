using UnityEngine;

namespace ShipWindows.Api.events;

public struct WindowEventArguments(WindowInfo windowInfo, GameObject? windowObject = null) {
    public bool cancelled = false;
    public WindowInfo windowInfo = windowInfo;
    public GameObject? windowObject = windowObject;
}