// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using ShipWindows.Api;
using ShipWindows.Config;

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
            // Floor Window Config
            new((config, windowInfo) => {
                if (!windowInfo.windowName.Equals("Floor Window")) return;

                WindowConfig.enableUnderLights = config.Bind($"{windowInfo.windowName} ({windowInfo.windowType})", "4. Spawn Underlights", true,
                                                             "If set to true, will spawn additional floodlights underneath the ship");
            }),

            // Right Window Config
            new((config, windowInfo) => {
                if (!windowInfo.windowName.Equals("Right Window")) return;

                WindowConfig.movePosters = config.Bind($"{windowInfo.windowName} ({windowInfo.windowType})", "4. Move Posters", true,
                                                       "If set to true, will move the poster that's obstructing the window");
            }),
        ];

        return additionalConfigActions.ToArray();
    }
}