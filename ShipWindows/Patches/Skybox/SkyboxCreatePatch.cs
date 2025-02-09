using System;
using HarmonyLib;
using ShipWindows.Config;
using ShipWindows.SoftDependencies;
using static UnityEngine.Object;

namespace ShipWindows.Patches.Skybox;

[HarmonyPatch(typeof(HUDManager))]
public static class SkyboxCreatePatch {
    [HarmonyPatch(nameof(HUDManager.Awake))]
    [HarmonyPostfix]
    public static void CreateSkybox() {
        var celestialTint = DependencyChecker.IsCelestialTintInstalled();

        switch (WindowConfig.spaceOutsideSetting.Value) {
            case SpaceOutside.SPACE_HDRI:
                var renderSystem = StartOfRound.Instance.blackSkyVolume.transform.parent;

                if (celestialTint) {
                    if (!WindowConfig.celestialTintOverrideSpace.Value) {
                        ShipWindows.Logger.LogWarning("Skybox set to HDRI, but Celestial Tint Override is set to false!");
                        break;
                    }

                    Instantiate(ShipWindows.Instance.GetCelestialTintOverlayPrefab(), renderSystem);
                    break;
                }

                Instantiate(ShipWindows.Instance.GetSpaceHdriPrefab(), renderSystem);
                break;
            case SpaceOutside.BLACK_AND_STARS:
                if (celestialTint) break;

                Instantiate(ShipWindows.Instance.GetStarsPrefab());
                break;
            case SpaceOutside.OTHER_MODS:
                break;
            default:
                throw new NotImplementedException();
        }
    }
}