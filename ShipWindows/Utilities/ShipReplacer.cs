using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShipWindows.Components;
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

            var windowSpawner1 = spawners.FirstOrDefault(spawner => spawner.id is 1) is not null;
            var windowSpawner2 = spawners.FirstOrDefault(spawner => spawner.id is 2) is not null;
            var windowSpawner3 = spawners.FirstOrDefault(spawner => spawner.id is 3) is not null;
            return $"ShipInsideWithWindow{(windowSpawner1? 1 : 0)}{(windowSpawner2? 1 : 0)}{(windowSpawner3? 1 : 0)}";
        }

        var window1Enabled = WindowConfig.enableWindow1.Value;
        var window2Enabled = WindowConfig.enableWindow2.Value;
        var window3Enabled = WindowConfig.enableWindow3.Value;
        return $"ShipInsideWithWindow{(window1Enabled? 1 : 0)}{(window2Enabled? 1 : 0)}{(window3Enabled? 1 : 0)}";
    }

    private static void AddWindowScripts(GameObject ship) {
        var leftDoorWindow = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorLeft (1)/WindowsLeft(Clone)");

        if (leftDoorWindow is not null) {
            ShipWindows.Logger.LogInfo("Left Door Windows not null!");

            if (leftDoorWindow.GetComponent<ShipWindow>() is null) {
                ShipWindows.Logger.LogInfo("Adding ShipWindow component to it!");

                var shipWindow = leftDoorWindow.AddComponent<ShipWindow>();
                shipWindow.id = 4;
            }
        } else {
            ShipWindows.Logger.LogInfo("Could not find Left Windows!");
        }

        var rightDoorWindow = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorRight (1)/WindowsRight(Clone)");

        if (rightDoorWindow is not null) {
            if (rightDoorWindow.GetComponent<ShipWindow>() is null) {
                var shipWindow = rightDoorWindow.AddComponent<ShipWindow>();
                shipWindow.id = 4;
            }
        }

        var container = ship.transform.Find("WindowContainer");
        if (container is null) return;

        foreach (Transform window in container) {
            if (window.gameObject.GetComponent<ShipWindow>() is not null) continue;

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

    private static void ReplaceGlassMaterial(GameObject shipPrefab) {
        if (WindowConfig.glassRefraction.Value) return;

        ShipWindows.Logger.LogInfo("Glass refraction is OFF! Replacing material...");

        var glassNoRefraction =
            ShipWindows.mainAssetBundle.LoadAsset<Material>("Assets/LethalCompany/Mods/ShipWindow/Materials/GlassNoRefraction.mat");

        if (glassNoRefraction is null) return;

        // This is bad so, so bad. Don't mind me :)
        var window1 = shipPrefab.transform.Find("WindowContainer/Window1/Glass")?.GetComponent<MeshRenderer>();
        var window2 = shipPrefab.transform.Find("WindowContainer/Window2/Glass")?.GetComponent<MeshRenderer>();
        var window3 = shipPrefab.transform.Find("WindowContainer/Window3")?.GetComponent<MeshRenderer>();

        // Don't worry, veri, this is even worse :)
        List<MeshRenderer> window4List = [
        ];

        var left = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorLeft (1)/WindowsLeft(Clone)")
                             ?.GetComponentsInChildren<MeshRenderer>();
        left ??= [
        ];

        var right = GameObject.Find("Environment/HangarShip/AnimatedShipDoor/HangarDoorRight (1)/WindowsRight(Clone)")
                              ?.GetComponentsInChildren<MeshRenderer>();
        right ??= [
        ];

        window4List.AddRange(left.Where(renderer => renderer is not null));
        window4List.AddRange(right.Where(renderer => renderer is not null));

        if (window1 is not null) window1.material = glassNoRefraction;
        if (window2 is not null) window2.material = glassNoRefraction;
        if (window3 is not null) window3.material = glassNoRefraction;

        foreach (var meshRenderer in window4List)
            meshRenderer.material = glassNoRefraction;
    }

    public static void ReplaceShip() {
        try {
            if (newShipInside is not null && vanillaShipInside is not null)
                //ShipWindows.Logger.LogInfo($"Calling ReplaceShip when ship was already replaced! Restoring original...");
                ObjectReplacer.Restore(vanillaShipInside);

            vanillaShipInside = FindOrThrow("Environment/HangarShip/ShipInside");
            var shipName = GetShipAssetName();

            //ShipWindows.Logger.LogInfo($"Replacing ship with {shipName}");

            var newShipPrefab =
                ShipWindows.mainAssetBundle.LoadAsset<GameObject>($"Assets/LethalCompany/Mods/ShipWindow/Ships/{shipName}.prefab");

            if (newShipPrefab is null) {
                if (!shipName.Equals("ShipInsideWithWindow000"))
                    throw new($"Could not load requested ship replacement! {shipName}");

                newShipPrefab = vanillaShipInside;
            }

            var spawners = Object.FindObjectsByType<ShipWindowSpawner>(FindObjectsSortMode.None);

            var windowSpawner4 = spawners.FirstOrDefault(spawner => spawner.id is 4) is not null;

            var window4Enabled = (!WindowConfig.windowsUnlockable.Value || WindowConfig.vanillaMode.Value)
                              && WindowConfig.enableWindow4.Value;

            if (windowSpawner4 || window4Enabled)
                SpawnDoorWindows();

            AddWindowScripts(newShipPrefab);
            ReplaceGlassMaterial(newShipPrefab);

            newShipInside = ObjectReplacer.Replace(vanillaShipInside, newShipPrefab);

            StartOfRound.Instance.StartCoroutine(WaitAndCheckSwitch());
        } catch (Exception e) {
            ShipWindows.Logger.LogError($"Failed to replace ShipInside! \n{e}");
        }
    }

    private static void SpawnDoorWindows() {
        var shipDoors = GameObject.Find("Environment/HangarShip/AnimatedShipDoor");

        var boxColliders = shipDoors.GetComponentsInChildren<BoxCollider>();

        foreach (var boxCollider in boxColliders) {
            if (boxColliders is null)
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

        var newDoorMesh =
            ShipWindows.mainAssetBundle.LoadAsset<Mesh>($"Assets/LethalCompany/Mods/ShipWindow/ShipDoor/ShipDoor{side}.mesh");

        ShipWindows.Logger.LogInfo("Got new mesh? " + (newDoorMesh != null));

        ObjectReplacer.ReplacedMeshes.Add(new() {
            meshFilter = doorMeshFilter,
            original = doorMeshFilter.mesh,
            replacement = newDoorMesh!,
        });

        doorMeshFilter.mesh = newDoorMesh;
        doorMeshFilter.sharedMesh = newDoorMesh;

        var doorCollider = door.gameObject.AddComponent<MeshCollider>();

        doorCollider.sharedMesh = doorMeshFilter.sharedMesh;


        var shipWindows =
            ShipWindows.mainAssetBundle.LoadAsset<GameObject>($"Assets/LethalCompany/Mods/ShipWindow/ShipDoor/Windows{side}.prefab");

        Object.Instantiate(shipWindows, door);
    }

    public static void SpawnSwitch() {
        var windowSwitch = Object.FindFirstObjectByType<ShipWindowShutterSwitch>();
        if (windowSwitch is not null) {
            switchInstance = windowSwitch.gameObject;
            return;
        }

        ShipWindows.Logger.LogInfo("Spawning shutter switch...");
        if (ShipWindows.windowSwitchPrefab is null)
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

        //ShipWindows.Logger.LogInfo("Checking window switch redundancy...");
        var windows = Object.FindObjectsByType<ShipWindow>(FindObjectsSortMode.None);

        if (windows.Length > 0) {
            if (switchInstance is null)
                SpawnSwitch();
            else
                switchInstance.SetActive(true);
        } else {
            if (switchInstance is null)
                return;

            Object.Destroy(switchInstance);
            switchInstance = null;
        }
    }

    public static IEnumerator WaitAndCheckSwitch() {
        yield return null;

        CheckShutterSwitch();
    }

    public static void RestoreShip() {
        if (newShipInside is null) return;

        if (vanillaShipInside is null)
            throw new NullReferenceException(nameof(vanillaShipInside) + " is null?!");

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