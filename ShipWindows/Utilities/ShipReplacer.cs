﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShipWindows.Components;
using ShipWindows.Networking;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShipWindows.Utilities;

internal static class ShipReplacer {
    public static bool debounceReplace;

    public static GameObject? vanillaShipInside;
    public static GameObject? newShipInside;

    // Only set on the server
    public static GameObject? switchInstance;

    private static GameObject? FindOrThrow(string name) {
        var gameObject = GameObject.Find(name);
        if (!gameObject) throw new($"Could not find {name}! Wrong scene?");

        return gameObject;
    }

    private static string GetShipAssetName() {
        if (WindowConfig.windowsUnlockable.Value && WindowConfig.vanillaMode.Value is false) {
            var spawners = Object.FindObjectsByType<ShipWindowSpawner>(FindObjectsSortMode.None);

            var windowSpawner1 = spawners.FirstOrDefault(spawner => spawner.id is 1) != null;
            var windowSpawner2 = spawners.FirstOrDefault(spawner => spawner.id is 2) != null;
            var windowSpawner3 = spawners.FirstOrDefault(spawner => spawner.id is 3) != null;
            return $"ShipInsideWithWindow{(windowSpawner1? 1 : 0)}{(windowSpawner2? 1 : 0)}{(windowSpawner3? 1 : 0)}";
        }

        var window1Enabled = WindowConfig.enableWindow1.Value;
        var window2Enabled = WindowConfig.enableWindow2.Value;
        var window3Enabled = WindowConfig.enableWindow3.Value;
        return $"ShipInsideWithWindow{(window1Enabled? 1 : 0)}{(window2Enabled? 1 : 0)}{(window3Enabled? 1 : 0)}";
    }

    private static void AddWindowScripts(GameObject ship) {
        var leftDoorWindow = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorLeft (1)/WindowsLeft(Clone)");

        if (leftDoorWindow != null) {
            if (leftDoorWindow.GetComponent<ShipWindow>() == null) {
                var shipWindow = leftDoorWindow.AddComponent<ShipWindow>();
                shipWindow.id = 4;
            }
        }

        var rightDoorWindow = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorRight (1)/WindowsRight(Clone)");

        if (rightDoorWindow != null) {
            if (rightDoorWindow.GetComponent<ShipWindow>() == null) {
                var shipWindow = rightDoorWindow.AddComponent<ShipWindow>();
                shipWindow.id = 4;
            }
        }

        var container = ship.transform.Find("WindowContainer");
        if (container == null) return;

        foreach (Transform window in container) {
            if (window.gameObject.GetComponent<ShipWindow>() != null) continue;

            if (int.TryParse(window.gameObject.name[^1].ToString(), out var id))
                window.gameObject.AddComponent<ShipWindow>().id = id;
        }
    }

    public static void ReplaceDebounced(bool replace) {
        //ShipWindows.Logger.LogInfo($"Debounce replace call. Replace? {replace} Is multiple call: {debounceReplace}");
        if (WindowConfig.windowsUnlockable.Value is false || WindowConfig.vanillaMode.Value) return;
        if (debounceReplace) return;

        debounceReplace = true;
        StartOfRound.Instance.StartCoroutine(ReplacementCoroutine(replace));
    }

    private static IEnumerator ReplacementCoroutine(bool replace) {
        yield return null; // Wait 1 frame.

        //ShipWindows.Logger.LogInfo("Performing ship replacement/restore.");
        debounceReplace = false;

        if (replace)
            ReplaceShip();
        else
            RestoreShip();
    }

    private static string GetMaterialName(this WindowMaterial separator) =>
        separator switch {
            WindowMaterial.NO_REFRACTION => "GlassNoRefraction",
            WindowMaterial.NO_REFRACTION_IRIDESCENCE => "GlassNoRefractionIridescence",
            WindowMaterial.REFRACTION => "GlassWithRefraction",
            WindowMaterial.REFRACTION_IRIDESCENCE => "GlassWithRefractionIridescence",
            var _ => throw new ArgumentOutOfRangeException(nameof(separator), separator, null),
        };

    internal static void ReplaceGlassMaterial() {
        if (newShipInside == null) return;

        ReplaceGlassMaterial(newShipInside);
    }

    private static void ReplaceGlassMaterial(GameObject shipPrefab) {
        var glassMaterial = WindowConfig.glassMaterial.Value;

        ShipWindows.Logger.LogInfo($"Replacing material glass material with: {glassMaterial}");

        var material = ShipWindows.mainAssetBundle.LoadAsset<Material>($"Assets/LethalCompany/Mods/plugins/ShipWindows/Materials/{
            glassMaterial.GetMaterialName()}.mat");

        if (material == null)
            throw new NullReferenceException($"Couldn't find glass material {glassMaterial} ({glassMaterial.GetMaterialName()})!");

        // This is bad so, so bad. Don't mind me :)
        var window1 = shipPrefab.transform.Find("WindowContainer/Window1/Glass")?.GetComponent<MeshRenderer>();
        var window2 = shipPrefab.transform.Find("WindowContainer/Window2/Glass")?.GetComponent<MeshRenderer>();
        var window3 = shipPrefab.transform.Find("WindowContainer/Window3")?.GetComponent<MeshRenderer>();

        // Don't worry, veri, this is even worse :)
        List<MeshRenderer?> window4List = [
        ];

        var leftFront = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorLeft (1)/WindowsLeft(Clone)/WindowFront");
        var leftBack = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorLeft (1)/WindowsLeft(Clone)/WindowBack");

        var rightFront = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorRight (1)/WindowsRight(Clone)/WindowFront");
        var rightBack = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorRight (1)/WindowsRight(Clone)/WindowBack");


        window4List.Add(leftFront?.GetComponent<MeshRenderer>());
        window4List.Add(leftBack?.GetComponent<MeshRenderer>());

        window4List.Add(rightFront?.GetComponent<MeshRenderer>());
        window4List.Add(rightBack?.GetComponent<MeshRenderer>());

        if (window1 != null) window1.material = material;
        if (window2 != null) window2.material = material;
        if (window3 != null) window3.material = material;

        foreach (var meshRenderer in window4List.OfType<MeshRenderer>()) meshRenderer.material = material;
    }

    public static void ReplaceShip() {
        try {
            if (newShipInside != null && vanillaShipInside != null)
                //ShipWindows.Logger.LogInfo($"Calling ReplaceShip when ship was already replaced! Restoring original...");
                ObjectReplacer.Restore(vanillaShipInside);

            vanillaShipInside = FindOrThrow("Environment/HangarShip/ShipInside");
            var shipName = GetShipAssetName();

            //ShipWindows.Logger.LogInfo($"Replacing ship with {shipName}");

            var newShipPrefab = ShipWindows.mainAssetBundle.LoadAsset<GameObject>($"Assets/LethalCompany/Mods/plugins/ShipWindows/Ships/{shipName}.prefab");

            if (newShipPrefab == null) {
                if (!shipName.Equals("ShipInsideWithWindow000"))
                    throw new($"Could not load requested ship replacement! {shipName}");

                newShipPrefab = vanillaShipInside;
            }

            var spawners = Object.FindObjectsByType<ShipWindowSpawner>(FindObjectsSortMode.None);

            var windowSpawner4 = spawners.FirstOrDefault(spawner => spawner.id is 4) != null;

            var window4Enabled = (!WindowConfig.windowsUnlockable.Value || WindowConfig.vanillaMode.Value) && WindowConfig.enableWindow4.Value;

            if (windowSpawner4 || window4Enabled)
                SpawnDoorWindows();

            AddWindowScripts(newShipPrefab);
            ReplaceGlassMaterial(newShipPrefab);

            newShipInside = ObjectReplacer.Replace(vanillaShipInside, newShipPrefab);

            if (Compatibility.ShipColors.Enabled) Compatibility.ShipColors.RefreshColors();

            StartOfRound.Instance.StartCoroutine(WaitAndCheckSwitch());

            WindowState.Instance.SetWindowState(WindowState.Instance.windowsClosed, WindowState.Instance.windowsLocked, false);
        } catch (Exception e) {
            ShipWindows.Logger.LogError($"Failed to replace ShipInside! \n{e}");
        }
    }

    private static void SpawnDoorWindows() {
        var shipDoors = GameObject.Find("Environment/HangarShip/AnimatedShipDoor");

        var boxColliders = shipDoors.GetComponentsInChildren<BoxCollider>();

        foreach (var boxCollider in boxColliders) {
            if (boxColliders == null)
                continue;

            boxCollider.isTrigger = true;
        }

        ReplaceDoor(shipDoors, "Left");

        ReplaceDoor(shipDoors, "Right");
    }

    private static void ReplaceDoor(GameObject shipDoors, string side) {
        ShipWindows.Logger.LogInfo("Replacing ship door: " + side);

        var door = shipDoors.transform.Find($"HangarDoor{side} (1)");

        if (door.transform.Find($"Windows{side}"))
            return;

        var doorMeshFilter = door.gameObject.GetComponent<MeshFilter>();

        var newDoorMesh = ShipWindows.mainAssetBundle.LoadAsset<Mesh>($"Assets/LethalCompany/Mods/plugins/ShipWindows/ShipDoor/ShipDoor{side}.asset");

        ShipWindows.Logger.LogInfo("Got new mesh? " + (newDoorMesh != null));


        var doorMeshRenderer = door.gameObject.GetComponent<MeshRenderer>();

        for (var index = 0; index < ShipWindows.DoorMaterials.Length; index++) {
            var material = ShipWindows.DoorMaterials[index];

            if (material != null) continue;

            material = ShipWindows.mainAssetBundle.LoadAsset<Material>($"Assets/LethalCompany/Mods/plugins/ShipWindows/Materials/HangarShipDoor{index + 1
            }.mat");

            if (material == null) {
                ShipWindows.Logger.LogError($"Couldn't find ship door material '{index}'!");
                return;
            }

            ShipWindows.DoorMaterials[index] = material;
        }

        var materials = ShipWindows.DoorMaterials;

        ObjectReplacer.ReplacedMaterials.Add(new() {
            meshRenderer = doorMeshRenderer,
            original = doorMeshRenderer.material,
            replacement = materials[0]!,

            originals = doorMeshRenderer.materials,
            replacements = materials!,
        });

        ObjectReplacer.ReplacedMeshes.Add(new() {
            meshFilter = doorMeshFilter,
            original = doorMeshFilter.mesh,
            replacement = newDoorMesh!,
        });

        doorMeshFilter.mesh = newDoorMesh;
        doorMeshFilter.sharedMesh = newDoorMesh;

        var doorCollider = door.gameObject.AddComponent<MeshCollider>();

        doorCollider.sharedMesh = doorMeshFilter.sharedMesh;


        doorMeshRenderer.material = materials[0];
        doorMeshRenderer.materials = materials;

        doorMeshRenderer.sharedMaterial = materials[0];
        doorMeshRenderer.sharedMaterials = materials;

        var shipWindows = ShipWindows.mainAssetBundle.LoadAsset<GameObject>($"Assets/LethalCompany/Mods/plugins/ShipWindows/ShipDoor/Windows{side}.prefab");

        Object.Instantiate(shipWindows, door);
    }

    public static void SpawnSwitch() {
        var windowSwitch = Object.FindFirstObjectByType<ShipWindowShutterSwitch>();
        if (windowSwitch != null) {
            switchInstance = windowSwitch.gameObject;
            return;
        }

        ShipWindows.Logger.LogInfo("Spawning shutter switch...");
        if (ShipWindows.windowSwitchPrefab == null)
            return;

        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
            return;

        switchInstance = Object.Instantiate(ShipWindows.windowSwitchPrefab);
        switchInstance.GetComponent<NetworkObject>().Spawn();
    }

    public static void CheckShutterSwitch() {
        if (NetworkManager.Singleton is {
                IsHost: false, IsServer: false,
            }) return;

        //ShipWindows.Logger.LogFatal("Checking window switch redundancy...");
        var windows = Object.FindObjectsByType<ShipWindow>(FindObjectsSortMode.None);

        windows ??= [
        ];

        //ShipWindows.Logger.LogFatal($"Found {windows.Length} windows!");

        if (windows.Length > 0) {
            if (switchInstance == null) SpawnSwitch();
            else switchInstance.SetActive(true);
        } else {
            if (switchInstance == null) return;

            var networkObject = switchInstance.GetComponent<NetworkObject>();

            if (networkObject != null) networkObject.Despawn();

            Object.Destroy(switchInstance);
            switchInstance = null;
        }
    }

    public static IEnumerator WaitAndCheckSwitch() {
        yield return null;

        CheckShutterSwitch();
    }

    public static void RestoreShip() {
        if (newShipInside == null) return;

        if (vanillaShipInside == null)
            throw new NullReferenceException(nameof(vanillaShipInside) + " == null?!");

        ObjectReplacer.RestoreMaterials();
        ObjectReplacer.RestoreMeshes();
        ObjectReplacer.Restore(vanillaShipInside);
        StartOfRound.Instance.StartCoroutine(WaitAndCheckSwitch());
        newShipInside = null;
    }

    // If any of the window spawners still exist without windows, spawn those windows.
    public static IEnumerator CheckForKeptSpawners() {
        yield return new WaitForSeconds(2f);

        var windows = Object.FindObjectsByType<ShipWindowSpawner>(FindObjectsSortMode.None);
        if (windows.Length <= 0)
            yield break;

        ReplaceDebounced(true);
    }
}