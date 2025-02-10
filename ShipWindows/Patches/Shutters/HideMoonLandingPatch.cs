using System.Collections;
using HarmonyLib;
using ShipWindows.Config;
using UnityEngine;

namespace ShipWindows.Patches.Shutters;

[HarmonyPatch(typeof(StartOfRound))]
public static class HideMoonLandingPatch {
    [HarmonyPatch(nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    public static void HideMoonLanding() {
        if (!StartOfRound.Instance.IsHost && !StartOfRound.Instance.IsServer) return;
        if (!WindowConfig.hideMoonLanding.Value) return;

        StartOfRound.Instance.StartCoroutine(ShutAndLockShuttersForTransition());
    }

    private static IEnumerator ShutAndLockShuttersForTransition() {
        var playAudio = WindowConfig.playShutterVoiceLinesOnLanding.Value;

        ShipWindows.networkManager?.ToggleShutters(true, true, playAudio);

        yield return new WaitUntil(() => StartOfRound.Instance.shipDoorsEnabled);

        ShipWindows.networkManager?.ToggleShutters(false, playAudio: playAudio);
    }
}