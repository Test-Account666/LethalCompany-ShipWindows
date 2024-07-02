using System.Collections;
using HarmonyLib;
using ShipWindows.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using static UnityEngine.Object;

namespace ShipWindows.Compatibility;

internal static class CelestialTint {
    private static GameObject? _skyGameObject;
    public static bool Enabled { get; private set; }
    private static bool _checkFor4K = true;

    private static bool Initialize() {
        if (Enabled) return false;
        Enabled = true;

        if (WindowConfig.celestialTintOverrideSpace.Value) {
            if (WindowConfig.spaceOutsideSetting.Value is not SpaceOutside.SPACE_HDRI) {
                ShipWindows.Logger.LogFatal(
                    "You have 'CelestialTintOverrideSpace' activated, but 'SpaceOutside' is not set to 'SPACE_HDRI'. This will do nothing!");
                return true;
            }
        }

        WindowConfig.celestialTintOverrideSpace.SettingChanged += (_, _) => CheckSceneState();

        SceneManager.sceneLoaded += (_, _) => {
            if (StartOfRound.Instance is null) return;

            StartOfRound.Instance.StartCoroutine(CheckSceneStateDelayed());
        };
        SceneManager.sceneUnloaded += _ => {
            if (StartOfRound.Instance is null) return;

            StartOfRound.Instance.StartCoroutine(CheckSceneStateDelayed());
        };
        return true;
    }

    [HarmonyPatch(typeof(global::CelestialTint), nameof(global::CelestialTint.CheckSceneState))]
    [HarmonyPostfix]
    private static void CelestialTintCheckSceneState() => CheckSceneState();

    private static IEnumerator CheckSceneStateDelayed() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        CheckSceneState();
    }

    private static void CheckSceneState() {
        if (!WindowConfig.celestialTintOverrideSpace.Value) {
            DestroySkyOverride();
            return;
        }

        if (SceneManager.sceneCount is not 1 || SceneManager.GetActiveScene() is not {
                name: "SampleSceneRelay",
            }) {
            DestroySkyOverride();
            return;
        }

        LoadSkyOverride();
    }

    private static void DestroySkyOverride() {
        if (_skyGameObject is null) return;

        Destroy(_skyGameObject);

        _skyGameObject = null;
    }

    private static void LoadSkyOverride() {
        if (_skyGameObject is not null) DestroySkyOverride();

        var skyPrefab =
            ShipWindows.mainAssetBundle.LoadAsset<GameObject>(
                "Assets/LethalCompany/Mods/ShipWindow/CelestialTint/CelestialTintSkyOverridePrefab.prefab");

        _skyGameObject = Instantiate(skyPrefab);

        CheckFor4KTexture();

        if (ShipWindow4K.Skybox4K is null) {
            ShipWindows.Logger.LogFatal("Did not find Skybox4k!");
            return;
        }

        OverrideSpaceEmissionTexture();
    }

    private static void OverrideSpaceEmissionTexture() {
        ShipWindows.Logger.LogFatal("Overriding texture now!");

        if (_skyGameObject is null) {
            ShipWindows.Logger.LogFatal("Nevermind!");
            return;
        }

        var skyBoxVolume = _skyGameObject.GetComponent<Volume>();

        skyBoxVolume.profile.TryGet<PhysicallyBasedSky>(out var physicallyBasedSky);

        if (physicallyBasedSky is null) {
            ShipWindows.Logger.LogFatal("Couldn't find PhysicallyBasedSky in celestial tint sky override!");
            return;
        }

        physicallyBasedSky.spaceEmissionTexture.value = ShipWindow4K.Skybox4K;

        ShipWindows.Logger.LogFatal("Loaded 4k Skybox!");
    }

    private static void CheckFor4KTexture() {
        if (!_checkFor4K) return;

        _checkFor4K = false;

        ShipWindow4K.TryToLoad();
    }
}