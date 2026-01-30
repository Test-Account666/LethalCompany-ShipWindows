// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using GameNetcodeStuff;
using HarmonyLib;
using ShipWindows.Config;
using UnityEngine;

namespace ShipWindows.Patches.EnemyFixes;

[HarmonyPatch(typeof(EnemyAICollisionDetect))]
public static class FixEnemyAttackPatch {
    [HarmonyPatch(nameof(EnemyAICollisionDetect.OnTriggerStay))]
    [HarmonyPrefix]
    // ReSharper disable InconsistentNaming
    private static bool CanCollide(EnemyAICollisionDetect __instance, EnemyAI ___mainScript, ref Collider other) {
        if (!WindowConfig.enableEnemyFix.Value) return true;

        var player = other.gameObject.GetComponent<PlayerControllerB>();
        var localPlayer = GameNetworkManager.Instance.localPlayerController;

        if (player != localPlayer) return true;

        var shipBounds = StartOfRound.Instance.shipBounds.bounds;

        var enemyInShip = shipBounds.Contains(___mainScript.transform.position);
        var playerInShip = localPlayer.playerCollider.bounds.Intersects(shipBounds);

        var canCollide = enemyInShip == playerInShip;
        if (!canCollide) other = new();

        return canCollide;
    }
}