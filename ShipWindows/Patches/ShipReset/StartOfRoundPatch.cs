using System.Collections;
using HarmonyLib;
using LethalModDataLib.Features;
using LethalModDataLib.Helpers;
using UnityEngine;
using static UnityEngine.Object;

namespace ShipWindows.Patches.ShipReset;

[HarmonyPatch(typeof(StartOfRound))]
public static class StartOfRoundPatch {
    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    [HarmonyPostfix]
    public static void DeleteAndRespawnWindows() => StartOfRound.Instance.StartCoroutine(DeleteAndRespawnCoroutine());

    private static IEnumerator DeleteAndRespawnCoroutine() {
        var decapitatedShip = ShipWindows.windowManager.decapitatedShip;

        if (decapitatedShip) Destroy(decapitatedShip);

        WindowUnlockData.UnlockedWindows.Clear();
        SaveLoadHandler.SaveData(ModDataHelper.GetModDataKey(typeof(WindowUnlockData), nameof(WindowUnlockData.UnlockedWindows))!);

        yield return null;

        ShipWindows.windowManager = new();
    }
}