using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace ShipWindows.Utilities;

internal static class ShipWindow4K {
    private static readonly DirectoryInfo _BaseDirectory = new(Assembly.GetExecutingAssembly().Location);

    public static AssetBundle? TextureBundle { get; private set; }
    public static Texture2D? Skybox4K { get; private set; }

    public static bool TryToLoad() {
        try {
            var pluginsFolder = _BaseDirectory.Parent?.Parent?.FullName;

            Debug.Assert(pluginsFolder is not null, nameof(pluginsFolder) + " != null");
            foreach (var file in Directory.GetFiles(pluginsFolder, "ship_window_4k", SearchOption.AllDirectories)) {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Extension.Equals(".old")) break;

                TextureBundle = AssetBundle.LoadFromFile(fileInfo.FullName);
                Skybox4K = TextureBundle.LoadAsset<Texture2D>("Assets/LethalCompany/Mods/ShipWindow/Textures/Space4KCube.png");

                ShipWindows.Logger.LogInfo("Found 4K skybox texture!");
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