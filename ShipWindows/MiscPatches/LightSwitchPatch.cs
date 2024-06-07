using HarmonyLib;

namespace ShipWindows.MiscPatches;

[HarmonyPatch(typeof(AutoParentToShip))]
public class LightSwitchPatch {
    [HarmonyPatch(nameof(AutoParentToShip.Awake))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    private static void ConstructorPostfix(AutoParentToShip __instance) {
        var objectName = __instance?.name ?? "";

        if (!objectName.Equals("LightSwitchContainer")) return;

        var interactTrigger = __instance?.GetComponentInChildren<InteractTrigger>();

        if (interactTrigger is null) return;

        if (!interactTrigger.hoverTip.Equals("Switch lights : [LMB]")) return;

        interactTrigger.hoverTip = "Toggle lights : [LMB]";
    }
}