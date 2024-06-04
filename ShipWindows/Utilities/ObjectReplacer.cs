using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShipWindows.Utilities;

internal static class ObjectReplacer {
    private static readonly Dictionary<GameObject?, ReplaceInfo> _ReplacedObjects = [
    ];

    public static void ReplaceMaterial(GameObject fromObj, GameObject toObj) {
        try {
            var mesh1 = fromObj.GetComponent<MeshRenderer>();
            var mesh2 = toObj.GetComponent<MeshRenderer>();

            if (!mesh1 || !mesh2) return;

            mesh2.material = mesh1.material;
        } catch (Exception e) {
            ShipWindows.Logger.LogError($"Could not replace object material:\n{e}");
        }
    }

    public static GameObject Replace(GameObject original, GameObject prefab) {
        ShipWindows.Logger.LogInfo($"Replacing object {original.name} with {prefab.name}...");
        var newObj = Object.Instantiate(prefab, original.transform.parent);
        newObj.transform.position = original.transform.position;
        newObj.transform.rotation = original.transform.rotation;

        var originalName = original.name;
        original.name = $"{originalName} (Old)";
        newObj.name = originalName;

        newObj.SetActive(true);
        original.SetActive(false);

        ReplaceInfo info;
        info.name = originalName;
        info.original = original;
        info.replacement = newObj;

        _ReplacedObjects[original] = info;

        return newObj;
    }

    public static void Restore(GameObject original) {
        if (!_ReplacedObjects.ContainsKey(original)) return;

        try {
            _ReplacedObjects.TryGetValue(original, out var info);

            ShipWindows.Logger.LogInfo($"Restoring object {info.name}...");

            info.original?.SetActive(true);

            if (info.original is not null)
                info.original.name = info.name;

            Object.DestroyImmediate(info.replacement);

            _ReplacedObjects.Remove(original);
        } catch (Exception) {
            ShipWindows.Logger.LogWarning($"GameObject replacement info not found for: " +
                                          $"{(original != null? original.name : "Invalid GameObject")}! Not replaced?");
        }
    }
}

internal struct ReplaceInfo {
    public string name;
    public GameObject? original;
    public GameObject replacement;
}