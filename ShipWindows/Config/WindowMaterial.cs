// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using System.Collections.Generic;
using BepInEx;
using UnityEngine;

namespace ShipWindows.Config;

public enum WindowMaterial {
    NO_REFRACTION,
    NO_REFRACTION_IRIDESCENCE,
    REFRACTION,
    REFRACTION_IRIDESCENCE,
}

public static class WindowMaterialConverter {
    private static readonly Dictionary<WindowMaterial, Material> _MaterialDictionary = new();

    public static Material? GetMaterial(this WindowMaterial windowMaterial) {
        LoadMaterial(windowMaterial);

        return _MaterialDictionary[windowMaterial];
    }

    private static void LoadMaterial(WindowMaterial windowMaterial) {
        if (_MaterialDictionary.ContainsKey(windowMaterial)) return;

        var materialPath = windowMaterial switch {
            WindowMaterial.NO_REFRACTION => $"{ShipWindows.ASSET_BUNDLE_PATH_PREFIX}/Windows/Shared/WindowMaterials/GlassNoRefraction.mat",
            WindowMaterial.NO_REFRACTION_IRIDESCENCE =>
                $"{ShipWindows.ASSET_BUNDLE_PATH_PREFIX}/Windows/Shared/WindowMaterials/GlassNoRefractionIridescence.mat",
            WindowMaterial.REFRACTION => $"{ShipWindows.ASSET_BUNDLE_PATH_PREFIX}/Windows/Shared/WindowMaterials/GlassWithRefraction.mat",
            WindowMaterial.REFRACTION_IRIDESCENCE =>
                $"{ShipWindows.ASSET_BUNDLE_PATH_PREFIX}/Windows/Shared/WindowMaterials/GlassWithRefractionIridescence.mat",
            var _ => null,
        };

        if (materialPath.IsNullOrWhiteSpace()) return;

        var material = ShipWindows.mainAssetBundle.LoadAsset<Material>(materialPath);
        _MaterialDictionary.Add(windowMaterial, material);
    }
}