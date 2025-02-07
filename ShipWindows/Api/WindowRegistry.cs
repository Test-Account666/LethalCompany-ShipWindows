using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ShipWindows.Api;

public class WindowRegistry {
    private readonly HashSet<WindowInfo> _windows = [
    ];

    public IReadOnlyCollection<WindowInfo> GetWindows() => _windows;

    public void UnregisterWindow(WindowInfo window) {
        var source = Assembly.GetCallingAssembly().GetName().Name;

        _windows.Remove(window);

        ShipWindows.Logger.LogDebug($"Unregistering window {window.windowName} from {source}!");
    }

    public void RegisterWindow(WindowInfo window) {
        var source = Assembly.GetCallingAssembly().GetName().Name;

        var windowName = window.windowName.ToLower();

        var isEnabled = ShipWindows.Instance.Config.Bind($"{windowName} ({window.windowType})", "1. Enabled", true, $"If {windowName} is enabled").Value;
        if (!isEnabled) return;

        var alwaysUnlocked = ShipWindows.Instance.Config
                                        .Bind($"{windowName} ({window.windowType})", "2. Always unlocked", false, $"If {windowName} is always unlocked").Value;
        window.alwaysUnlocked = alwaysUnlocked;

        var price = ShipWindows.Instance.Config.Bind($"{windowName} ({window.windowType})", "2. Unlock Cost", window.cost, $"Cost to unlock {windowName}").Value;
        window.cost = price;

        var alreadyExists = _windows.Any(info => info.windowName.ToLower().Equals(windowName.ToLower()));

        if (alreadyExists) throw new DuplicateNameException($"There is already a window with name {windowName}! Source: {source}");

        var typeAlreadyExists = _windows.Any(info => info.windowType.ToLower().Equals(window.windowType.ToLower()));

        if (typeAlreadyExists) throw new DuplicateNameException($"Window {windowName} has duplicate window type {window.windowType}! Source: {source}");

        _windows.Add(window);

        ShipWindows.Logger.LogDebug($"Registering window {windowName} from {source}!");
    }
}