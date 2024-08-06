using HarmonyLib;

namespace ShipWindows.MiscPatches;

[HarmonyPatch(typeof(ShipBuildModeManager))]
public static class ShipBuildModeManagerPatch {
    [HarmonyPatch(nameof(ShipBuildModeManager.Awake))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void ChangeLayerMasks(ShipBuildModeManager __instance) {
        __instance.placementMask |= (1 << 28);
        __instance.placementMaskAndBlockers |= (1 << 28);
        __instance.placeableShipObjectsMask |= (1 << 28);
    }
}