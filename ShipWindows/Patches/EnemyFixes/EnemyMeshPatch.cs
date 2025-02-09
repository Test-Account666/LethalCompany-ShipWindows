using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace ShipWindows.Patches.EnemyFixes;

[HarmonyPatch]
public static class EnemyMeshPatch {
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.SetPlayerSafeInShip))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> EnableAllEnemyMeshes(IEnumerable<CodeInstruction> instructions) {
        const string patternStart = "ldfld bool StartOfRound::hangarDoorsClosed";
        const string patternEnd = "ret NULL";

        ShipWindows.Logger.LogDebug("Searching for start: " + patternStart);
        ShipWindows.Logger.LogDebug("Searching for end: " + patternEnd);

        var codeInstructions = instructions.ToList();

        var start = -1;
        var end = -1;

        for (var index = 0; index < codeInstructions.Count; index++) {
            var codeInstruction = codeInstructions[index];

            if (start == -1 && codeInstruction.ToString().Equals(patternStart)) {
                start = index - 1;
                continue;
            }

            if (!codeInstruction.ToString().Equals(patternEnd)) continue;

            end = index;
            break;
        }

        if (start is -1 || end is -1) {
            ShipWindows.Logger.LogError("Couldn't find instructions to remove!");
            ShipWindows.Logger.LogError("Start: " + start);
            ShipWindows.Logger.LogError("End: " + end);
            ShipWindows.Logger.LogError("Please report this error!");
            return codeInstructions;
        }

        ShipWindows.Logger.LogDebug("Found start at: " + start);
        ShipWindows.Logger.LogDebug("Found end at: " + end);

        codeInstructions.RemoveRange(start, end - start);

        return codeInstructions;
    }

    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void EnableEnemyMesh(EnemyAI __instance) => EnableEnemyMeshGeneric(__instance);

    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.Start))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void EnableEnemyMesh(MaskedPlayerEnemy __instance) => EnableEnemyMeshGeneric(__instance);

    private static void EnableEnemyMeshGeneric(EnemyAI enemyAI) => enemyAI.EnableEnemyMesh(true);
}