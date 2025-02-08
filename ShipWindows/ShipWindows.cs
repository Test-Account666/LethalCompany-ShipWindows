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
using ShipWindows.Patches.Networking;
using ShipWindows.Patches.ShipReset;
using ShipWindows.Patches.WindowManager;
using ShipWindows.Utilities;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace ShipWindows;

[BepInIncompatibility("veri.lc.shipwindow")]
[BepInDependency("WhiteSpike.InteractiveTerminalAPI", "1.1.4")]
[BepInDependency("MaxWasUnavailable.LethalModDataLib")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ShipWindows : BaseUnityPlugin {
    public const string ASSET_BUNDLE_PATH_PREFIX = "Assets/LethalCompany/Mods/plugins/ShipWindows/Beta";

    public static AssetBundle mainAssetBundle = null!;

    public static WindowRegistry windowRegistry = null!;
    public static WindowManager windowManager = null!;

    public static INetworkManager? networkManager;

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

        try {
            InitializeNetcode();
        } catch (Exception e) {
            Logger.LogError("Something went wrong with the netcode patcher!");
            Logger.LogError(e);
            return;
        }

        windowRegistry = new();

        WindowLoader.LoadWindows();

        if (!WindowConfig.vanillaMode.Value) {
            InteractiveTerminalManager.RegisterApplication<ShipWindowApplication>("windows", false);
            Harmony.PatchAll(typeof(GameNetworkManagerPatch));
        } else {
            networkManager = new DummyNetworkManager();
        }

        Harmony.PatchAll(typeof(HUDManagerPatch));
        Harmony.PatchAll(typeof(StartOfRoundPatch));

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

    private static void InitializeNetcode() {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types) {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods) {
                var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.Length <= 0) continue;
                method.Invoke(null, null);
            }
        }
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
}