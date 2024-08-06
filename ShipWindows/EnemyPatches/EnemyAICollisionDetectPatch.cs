using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace ShipWindows.EnemyPatches;

[HarmonyPatch(typeof(EnemyAICollisionDetect))]
public static class EnemyAICollisionDetectPatch {
    [HarmonyPatch(nameof(EnemyAICollisionDetect.OnTriggerStay))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static bool CanCollide(EnemyAI ___mainScript, ref Collider other) {
        if (!WindowConfig.enableEnemyFix.Value) return true;

        var player = other.gameObject.GetComponent<PlayerControllerB>();
        var localPlayer = GameNetworkManager.Instance.localPlayerController;

        if (player != localPlayer) return true;

        var canCollide = ___mainScript.isInsidePlayerShip == localPlayer.isInHangarShipRoom;

        if (!canCollide) other = new();

        return canCollide;
    }
}