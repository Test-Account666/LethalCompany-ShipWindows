// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using HarmonyLib;

namespace ShipWindows.Patches.BuildManagerFix;

[HarmonyPatch(typeof(ShipBuildModeManager))]
public class BuildManagerFixPatch {
    [HarmonyPatch(nameof(ShipBuildModeManager.Awake))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void ChangeLayerMasks(ShipBuildModeManager __instance) {
        __instance.placementMask |= (1 << 28);
        __instance.placementMaskAndBlockers |= (1 << 28);
        __instance.placeableShipObjectsMask |= (1 << 28);
    }
}