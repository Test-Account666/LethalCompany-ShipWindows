// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using HarmonyLib;

namespace ShipWindows.Patches.WindowManager;

[HarmonyPatch(typeof(HUDManager))]
public static class WindowManagerCreatePatch {
    [HarmonyPatch(nameof(HUDManager.Awake))]
    [HarmonyPostfix]
    public static void CreateWindowManager() => ShipWindows.windowManager = new();
}