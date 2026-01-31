// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;

namespace ShipWindows.Api;

public class WindowRegistry {
    private readonly HashSet<WindowInfo> _windows = [
    ];

    public IReadOnlyCollection<WindowInfo> Windows => _windows;

    public void UnregisterWindow(WindowInfo window) {
        var source = Assembly.GetCallingAssembly().GetName().Name;

        _windows.Remove(window);

        ShipWindows.Logger.LogDebug($"Unregistering window {window.windowName} from {source}!");
    }

    public void RegisterWindow(WindowInfo window, Action<ConfigFile, WindowInfo>? configAction = null) {
        RegisterWindow(window, configAction != null
            ? [
                configAction,
            ]
            : null);
    }

    public void RegisterWindow(WindowInfo window, Action<ConfigFile, WindowInfo>[]? configAction = null) {
        configAction ??= [
        ];

        var source = Assembly.GetCallingAssembly().GetName().Name;

        var windowName = window.windowName;

        var isEnabled = ShipWindows.Instance.Config
                                   .Bind($"{windowName} ({window.windowType})", "1. Enabled", !window.deactivatedByDefault,
                                       $"If {windowName} is enabled").Value;
        if (!isEnabled) return;

        var alwaysUnlocked = ShipWindows.Instance.Config
                                        .Bind($"{windowName} ({window.windowType})", "2. Always unlocked", false,
                                            $"If {windowName} is always unlocked").Value;
        window.alwaysUnlocked = alwaysUnlocked;

        var price = ShipWindows.Instance.Config
                               .Bind($"{windowName} ({window.windowType})", "3. Unlock Cost", window.cost, $"Cost to unlock {windowName}")
                               .Value;
        window.cost = price;

        var allowEnemyTrigger = ShipWindows.Instance.Config
                                           .Bind($"{windowName} ({window.windowType})", "4. Allow Enemy Triggering",
                                               window.allowEnemyTriggering,
                                               "If set to true, will allow enemies to trigger through the window").Value;
        window.allowEnemyTriggering = allowEnemyTrigger;

        foreach (var action in configAction) action.Invoke(ShipWindows.Instance.Config, window);

        var alreadyExists = _windows.Any(info => info.windowName.Equals(windowName));

        if (alreadyExists) throw new DuplicateNameException($"There is already a window with name {windowName}! Source: {source}");

        var typeAlreadyExists = _windows.Any(info => info.windowType.Equals(window.windowType));

        if (typeAlreadyExists)
            throw new DuplicateNameException($"Window {windowName} has duplicate window type {window.windowType}! Source: {source}");

        _windows.Add(window);

        ShipWindows.Logger.LogDebug($"Registering window {windowName} from {source}!");
    }
}