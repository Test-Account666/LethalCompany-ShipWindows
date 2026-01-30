// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;

namespace ShipWindows.SoftDependencies;

public static class DependencyChecker {
    private static readonly Dictionary<string, bool> _DependencyDictionary = [
    ];

    public static bool thereIsAShipModInstalled;

    public static bool IsCelestialTintInstalled() => IsDependencyInstalled("CelestialTint");

    public static bool IsWiderShipInstalled() => IsDependencyInstalled("mborsh.WiderShipMod");

    public static bool IsTwoStoryShipInstalled() => IsDependencyInstalled("MelanieMelicious.2StoryShip");

    public static bool IsShipBuilderInstalled() => IsDependencyInstalled("mborsh.ShipBuilder");

    public static bool IsAnyShipModInstalled() => IsTwoStoryShipInstalled() || IsWiderShipInstalled() || IsShipBuilderInstalled() || thereIsAShipModInstalled;

    public static bool IsDependencyInstalled(string dependencyName) {
        var containsKey = _DependencyDictionary.TryGetValue(dependencyName, out var installed);

        if (containsKey) return installed;

        installed = Chainloader.PluginInfos.Values.Any(metadata => metadata.Metadata.GUID.Contains(dependencyName));

        _DependencyDictionary[dependencyName] = installed;

        return installed;
    }
}