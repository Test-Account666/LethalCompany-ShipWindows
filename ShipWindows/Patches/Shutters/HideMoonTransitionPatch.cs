using System.Collections;
using HarmonyLib;
using ShipWindows.Config;
using UnityEngine;

namespace ShipWindows.Patches.Shutters;

[HarmonyPatch(typeof(StartOfRound))]
public static class HideMoonTransitionPatch {
    [HarmonyPatch(nameof(StartOfRound.ChangeLevel))]
    [HarmonyPostfix]
    public static void HideMoonTransition() {
        if (!StartOfRound.Instance.IsHost && !StartOfRound.Instance.IsServer) return;
        if (!WindowConfig.shuttersHideMoonTransitions.Value) return;

        var currentLevel = StartOfRound.Instance.currentLevel;
        StartOfRound.Instance.StartCoroutine(ShutAndLockShuttersForTransition(currentLevel.timeToArrive));
    }

    private static IEnumerator ShutAndLockShuttersForTransition(float transitionTime) {
        ShipWindows.networkManager?.ToggleShutters(true, true);

        yield return new WaitForSeconds(transitionTime + 2.5F);

        ShipWindows.networkManager?.ToggleShutters(false);
    }
}