using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace ShipWindows.Utilities;

internal static class ShipWindow4K {
    private static readonly DirectoryInfo _BaseDirectory = new(Assembly.GetExecutingAssembly().Location);

    public static AssetBundle? TextureBundle { get; private set; }
    public static Texture? Skybox4K { get; private set; }

    public static bool TryToLoad() {
        if (Skybox4K is not null) return true;

        try {
            var pluginsFolder = _BaseDirectory.Parent?.Parent?.FullName;

            Debug.Assert(pluginsFolder is not null, nameof(pluginsFolder) + " != null");
            foreach (var file in Directory.GetFiles(pluginsFolder, "ship_window_4k", SearchOption.AllDirectories)) {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Extension.Equals(".old")) break;

                TextureBundle ??= AssetBundle.LoadFromFile(fileInfo.FullName);

                var allTextures = TextureBundle.LoadAllAssets();

                allTextures ??= [
                ];

                Skybox4K = allTextures.Length > 0? allTextures[0] as Texture : null;

                if (Skybox4K is null) throw new NullReferenceException("Texture not present");

                ShipWindows.Logger.LogInfo("Found 4K skybox texture! " + (Skybox4K != null));
                return true;
            }
        } catch (Exception exception) {
            ShipWindows.Logger.LogError($"Failed to find and load 4K skybox AssetBundle!\n{exception}");
            return false;
        }

        ShipWindows.Logger.LogInfo("Did not locate 4K skybox bundle.");
        return false;
    }
}