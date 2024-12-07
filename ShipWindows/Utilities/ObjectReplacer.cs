﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShipWindows.Utilities;

internal static class ObjectReplacer {
    private static readonly Dictionary<GameObject?, ReplaceInfo> _ReplacedObjects = [
    ];

    internal static readonly List<ReplacedMeshInfo> ReplacedMeshes = [
    ];

    internal static readonly List<ReplacedMaterialInfo> ReplacedMaterials = [
    ];

    public static void ReplaceMaterial(GameObject fromObj, GameObject toObj) {
        try {
            var mesh1 = fromObj.GetComponent<MeshRenderer>();
            var mesh2 = toObj.GetComponent<MeshRenderer>();

            if (!mesh1 || !mesh2) return;

            mesh2.material = mesh1.material;
            mesh2.materials = mesh1.materials;
            mesh2.sharedMaterial = mesh1.sharedMaterial;
            mesh2.sharedMaterials = mesh1.sharedMaterials;
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

        var info = new ReplaceInfo {
            name = originalName,
            original = original,
            replacement = newObj,
        };

        _ReplacedObjects[original] = info;

        return newObj;
    }

    public static void RestoreMeshes() {
        foreach (var replacedMesh in ReplacedMeshes.ToList()) {
            var meshFilter = replacedMesh.meshFilter;

            meshFilter.mesh = replacedMesh.original;
            meshFilter.sharedMesh = replacedMesh.original;

            ReplacedMeshes.Remove(replacedMesh);
        }
    }

    public static void RestoreMaterials() {
        foreach (var replacedMaterial in ReplacedMaterials.ToList()) {
            var meshFilter = replacedMaterial.meshRenderer;

            meshFilter.material = replacedMaterial.original;
            meshFilter.sharedMaterial = replacedMaterial.original;

            meshFilter.materials = replacedMaterial.originals;
            meshFilter.sharedMaterials = replacedMaterial.originals;

            ReplacedMaterials.Remove(replacedMaterial);
        }
    }

    public static void Restore(GameObject original) {
        try {
            var containsObject = _ReplacedObjects.TryGetValue(original, out var info);

            if (!containsObject) return;

            ShipWindows.Logger.LogInfo($"Restoring object {info.name}...");

            info.original?.SetActive(true);

            if (info.original != null) info.original.name = info.name;

            Object.DestroyImmediate(info.replacement);

            _ReplacedObjects.Remove(original);
        } catch (Exception) {
            ShipWindows.Logger.LogWarning($"GameObject replacement info not found for: {(original != null? original.name : "Invalid GameObject")
            }! Not replaced?");
        }
    }
}

internal record struct ReplacedMeshInfo {
    public MeshFilter meshFilter;
    public Mesh original;
    public Mesh replacement;
}

internal record struct ReplacedMaterialInfo {
    public MeshRenderer meshRenderer;
    public Material original;
    public Material replacement;

    public Material[] originals;
    public Material[] replacements;

    public bool Equals(ReplacedMaterialInfo other) =>
        meshRenderer == other.meshRenderer && original == other.original && replacement == other.replacement && originals.SequenceEqual(other.originals)
     && replacements.SequenceEqual(other.replacements);

    public override int GetHashCode() => HashCode.Combine(meshRenderer, original, replacement, originals, replacements);
}

internal struct ReplaceInfo {
    public string name;
    public GameObject? original;
    public GameObject replacement;
}