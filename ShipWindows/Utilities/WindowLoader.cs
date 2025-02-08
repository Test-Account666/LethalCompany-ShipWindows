using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using ShipWindows.Api;

namespace ShipWindows.Utilities;

internal static class WindowLoader {
    public static void LoadWindows() {
        var windowInfos = ShipWindows.mainAssetBundle.LoadAllAssets<WindowInfo>();

        windowInfos ??= [
        ];

        //TODO: Figure out a better system for additional configs
        var additionalConfigActions = GetAdditionalConfigActions();

        foreach (var windowInfo in windowInfos) {
            if (!windowInfo) continue;

            ShipWindows.windowRegistry.RegisterWindow(windowInfo, additionalConfigActions);
        }
    }

    private static Action<ConfigFile, WindowInfo>[] GetAdditionalConfigActions() {
        List<Action<ConfigFile, WindowInfo>> additionalConfigActions = [
            new((config, windowInfo) => {
                if (!windowInfo.windowName.Equals("Floor Window")) return;

                config.Bind($"{windowInfo.windowName} ({windowInfo.windowType})", "4. Spawn Underlights", true);
            }),
        ];

        return additionalConfigActions.ToArray();
    }
}