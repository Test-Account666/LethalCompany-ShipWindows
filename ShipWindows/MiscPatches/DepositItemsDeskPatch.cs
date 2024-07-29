using System.Linq;
using HarmonyLib;
using ShipWindows.Utilities;

namespace ShipWindows.MiscPatches;

[HarmonyPatch(typeof(DepositItemsDesk))]
public static class DepositItemsDeskPatch {
    [HarmonyPatch(nameof(DepositItemsDesk.Start))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void AddVoiceLines(DepositItemsDesk __instance) {
        var voiceLines = __instance.rareMicrophoneAudios.ToList();

        voiceLines.AddRange(SoundLoader.RareSellCounterLines);

        voiceLines.RemoveAll(clip => clip == null);

        __instance.rareMicrophoneAudios = voiceLines.ToArray();
    }
}