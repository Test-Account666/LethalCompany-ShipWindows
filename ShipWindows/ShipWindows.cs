using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using InteractiveTerminalAPI.UI;
using ShipWindows.Api;
using ShipWindows.Config;
using ShipWindows.Networking;
using ShipWindows.Patches.BuildManagerFix;
using ShipWindows.Patches.EnemyFixes;
using ShipWindows.Patches.Networking;
using ShipWindows.Patches.SellAudios;
using ShipWindows.Patches.ShipReset;
using ShipWindows.Patches.Shutters;
using ShipWindows.Patches.Skybox;
using ShipWindows.Patches.WindowManager;
using ShipWindows.SkyBox;
using ShipWindows.Utilities;
using UnityEngine;
using static TestAccountCore.AssetLoader;
using static TestAccountCore.Netcode;
using Debug = System.Diagnostics.Debug;

namespace ShipWindows;

[BepInIncompatibility("veri.lc.shipwindow")]
[BepInDependency("WhiteSpike.InteractiveTerminalAPI", "1.1.4")]
[BepInDependency("MaxWasUnavailable.LethalModDataLib")]
[BepInDependency("TestAccount666.TestAccountCore", "1.14.0")]
[BepInDependency("evaisa.lethallib", "0.16.2")]
[BepInDependency("CelestialTint", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ShipWindows : BaseUnityPlugin {
    public const string ASSET_BUNDLE_PATH_PREFIX = "Assets/LethalCompany/Mods/plugins/ShipWindows/Beta";

    public static AssetBundle mainAssetBundle = null!;

    public static WindowRegistry windowRegistry = null!;
    public static WindowManager windowManager = null!;

    public static INetworkManager? networkManager;

    public static AbstractSkyBox? skyBox;

    public static ShipWindows Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; private set; }

    private void Awake() {
        Logger = base.Logger;
        Instance = this;

        Harmony ??= new(MyPluginInfo.PLUGIN_GUID);

        WindowConfig.InitializeConfig(Config);

        if (!LoadAssetBundle()) {
            Logger.LogError("Failed to load asset bundle! Abort mission!");
            return;
        }

        var assembly = Assembly.GetExecutingAssembly();

        try {
            ExecuteNetcodePatcher(assembly);
        } catch (Exception e) {
            Logger.LogError("Something went wrong with the netcode patcher!");
            Logger.LogError(e);
            return;
        }

        if (!WindowConfig.vanillaMode.Value) {
            LoadBundle(assembly, "ship_windows_shutter");
            LoadUnlockables(Config);
        }

        windowRegistry = new();

        WindowLoader.LoadWindows();

        if (!WindowConfig.vanillaMode.Value) {
            InteractiveTerminalManager.RegisterApplication<ShipWindowApplication>([
                "windows", "window",
            ], false);
            Harmony.PatchAll(typeof(NetworkingStuffPatch));
        } else {
            networkManager = new DummyNetworkManager();
        }

        Harmony.PatchAll(typeof(WindowManagerCreatePatch));
        Harmony.PatchAll(typeof(ShipResetPatch));
        Harmony.PatchAll(typeof(EnemyMeshPatch));
        Harmony.PatchAll(typeof(SkyboxCreatePatch));
        Harmony.PatchAll(typeof(HideMoonTransitionPatch));
        Harmony.PatchAll(typeof(HideMoonLandingPatch));
        Harmony.PatchAll(typeof(AddSellAudiosPatch));
        Harmony.PatchAll(typeof(FixEnemyAttackPatch));
        Harmony.PatchAll(typeof(BuildManagerFixPatch));

        StartCoroutine(SoundLoader.LoadAudioClips());

        Logger.LogInfo("Loaded successfully!");
    }

    private static bool LoadAssetBundle() {
        Logger.LogInfo("Loading ShipWindow AssetBundle...");
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Debug.Assert(assemblyLocation != null, nameof(assemblyLocation) + " != null");
        mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation, "ship_window"));

        return mainAssetBundle != null;
    }

    private GameObject _decapitatedShipPrefab = null!;

    public GameObject GetDecapitatedShipPrefab() {
        if (_decapitatedShipPrefab) return _decapitatedShipPrefab;


        _decapitatedShipPrefab = mainAssetBundle.LoadAsset<GameObject>($"{ASSET_BUNDLE_PATH_PREFIX}/PrefabDecapitatedShip.prefab");
        return _decapitatedShipPrefab;
    }

    private GameObject _networkManagerPrefab = null!;

    public GameObject GetNetworkManagerPrefab() {
        if (_networkManagerPrefab) return _networkManagerPrefab;

        _networkManagerPrefab = mainAssetBundle.LoadAsset<GameObject>($"{ASSET_BUNDLE_PATH_PREFIX}/PrefabShipWindowsNetworkManager.prefab");
        return _networkManagerPrefab;
    }

    private GameObject _hdriSpacePrefab = null!;

    public GameObject GetSpaceHdriPrefab() {
        if (_hdriSpacePrefab) return _hdriSpacePrefab;

        _hdriSpacePrefab = mainAssetBundle.LoadAsset<GameObject>($"{ASSET_BUNDLE_PATH_PREFIX}/SkyBox/HDRI/UniverseVolume.prefab");
        return _hdriSpacePrefab;
    }

    private GameObject _celestialTintOverlayPrefab = null!;

    public GameObject GetCelestialTintOverlayPrefab() {
        if (_celestialTintOverlayPrefab) return _celestialTintOverlayPrefab;

        _celestialTintOverlayPrefab = mainAssetBundle.LoadAsset<GameObject>($"{ASSET_BUNDLE_PATH_PREFIX}/SkyBox/HDRI/CelestialTintOverride.prefab");
        return _celestialTintOverlayPrefab;
    }

    private GameObject _starsPrefab = null!;

    public GameObject GetStarsPrefab() {
        if (_starsPrefab) return _starsPrefab;

        _starsPrefab = mainAssetBundle.LoadAsset<GameObject>($"{ASSET_BUNDLE_PATH_PREFIX}/SkyBox/StarsSphereLarge.prefab");
        return _starsPrefab;
    }
}