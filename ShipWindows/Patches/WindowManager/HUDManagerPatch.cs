using HarmonyLib;

namespace ShipWindows.Patches.WindowManager;

[HarmonyPatch(typeof(HUDManager))]
public static class HUDManagerPatch {
    [HarmonyPatch(nameof(HUDManager.Awake))]
    [HarmonyPostfix]
    public static void CreateWindowManager() => ShipWindows.windowManager = new();
}