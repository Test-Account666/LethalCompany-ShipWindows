// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using System.Collections;
using HarmonyLib;
using static UnityEngine.Object;

namespace ShipWindows.Patches.ShipReset;

[HarmonyPatch(typeof(StartOfRound))]
public static class ShipResetPatch {
    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    [HarmonyPostfix]
    public static void DeleteAndRespawnWindows() => StartOfRound.Instance.StartCoroutine(DeleteAndRespawnCoroutine());

    private static IEnumerator DeleteAndRespawnCoroutine() {
        var decapitatedShip = ShipWindows.windowManager.decapitatedShip;

        if (decapitatedShip) Destroy(decapitatedShip);

        WindowUnlockData.UnlockedWindows.Clear();

        yield return null;

        ShipWindows.windowManager = new();
    }
}