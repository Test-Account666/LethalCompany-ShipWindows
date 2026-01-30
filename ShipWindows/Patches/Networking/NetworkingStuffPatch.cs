// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using HarmonyLib;
using Unity.Netcode;
using static UnityEngine.Object;

namespace ShipWindows.Patches.Networking;

[HarmonyPatch(typeof(GameNetworkManager))]
public static class NetworkingStuffPatch {
    [HarmonyPatch(nameof(GameNetworkManager.Start))]
    [HarmonyPostfix]
    public static void RegisterNetworkPrefab() {
        var networkManagerPrefab = ShipWindows.Instance.GetNetworkManagerPrefab();

        if (NetworkManager.Singleton.NetworkConfig.Prefabs.Contains(networkManagerPrefab)) return;

        NetworkManager.Singleton.AddNetworkPrefab(networkManagerPrefab);
    }

    [HarmonyPatch(nameof(GameNetworkManager.Disconnect))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static void DestroyNetworkManager(GameNetworkManager __instance) {
        if (!__instance.isHostingGame) {
            ShipWindows.networkManager = null!;
            return;
        }

        ShipWindows.networkManager?.NetworkObject.Despawn();
    }

    [HarmonyPatch(nameof(GameNetworkManager.SetLobbyJoinable))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    public static void SpawnNetworkManager(GameNetworkManager __instance) {
        if (!__instance.isHostingGame) return;

        if (ShipWindows.networkManager?.NetworkObject) {
            ShipWindows.Logger.LogDebug("Network manager already exists! Destroying...");
            ShipWindows.networkManager.NetworkObject.Despawn();
        }

        var networkManagerObject = Instantiate(ShipWindows.Instance.GetNetworkManagerPrefab());

        var networkObject = networkManagerObject.GetComponent<NetworkObject>();
        networkObject.name = "ShipWindowsNetworkManager";
        networkObject.Spawn();
        DontDestroyOnLoad(networkManagerObject);
    }
}