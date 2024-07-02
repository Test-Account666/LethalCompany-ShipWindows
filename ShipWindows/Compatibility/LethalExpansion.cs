using HarmonyLib;
using UnityEngine;

namespace ShipWindows.Compatibility;

internal static class LethalExpansion {
    public static bool Enabled { get; private set; }

    private static bool Initialize() {
        if (Enabled) return false;
        Enabled = true;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Start))]
    private static void Patch_RoundStart() {
        if (!Enabled) return;

        // https://github.com/jverif/lc-shipwindow/issues/8
        // Lethal Expansion "terrainfixer" is positioned at 0, -500, 0 and becomes
        // visible when a mod that increases view distance is installed.
        var terrainFixer = GameObject.Find("terrainfixer");
        if (terrainFixer is null) return;

        terrainFixer.transform.position = new(0, -5000, 0);
    }
}