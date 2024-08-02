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
        var rareVoiceLines = __instance.rareMicrophoneAudios.ToList();

        rareVoiceLines.AddRange(SoundLoader.RareSellCounterLines);

        rareVoiceLines.RemoveAll(clip => clip == null);

        __instance.rareMicrophoneAudios = rareVoiceLines.ToArray();


        var commonVoiceLines = __instance.microphoneAudios.ToList();

        commonVoiceLines.AddRange(SoundLoader.CommonSellCounterLines);

        commonVoiceLines.RemoveAll(clip => clip == null);

        __instance.microphoneAudios = commonVoiceLines.ToArray();
    }
}