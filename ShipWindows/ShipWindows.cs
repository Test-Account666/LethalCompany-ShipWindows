using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using ShipWindows.Compatibility;
using ShipWindows.Components;
using ShipWindows.EnemyPatches;
using ShipWindows.MiscPatches;
using ShipWindows.Networking;
using ShipWindows.Utilities;
using Unity.Netcode;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace ShipWindows;

[BepInIncompatibility("veri.lc.shipwindow")]
[CompatibleDependency("CelestialTint", "1.0.1", typeof(Compatibility.CelestialTint))]
[CompatibleDependency("LethalExpansion", typeof(LethalExpansion))]
[CompatibleDependency("com.github.lethalmods.lethalexpansioncore", typeof(LethalExpansion))]
[CompatibleDependency("BMX.LobbyCompatibility", typeof(Compatibility.LobbyCompatibility))]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ShipWindows : BaseUnityPlugin {
    public static AssetBundle mainAssetBundle = null!;

    // Prefabs
    public static GameObject? windowSwitchPrefab;

    public static readonly Dictionary<int, ShipWindowDef> WindowRegistry = [
    ];

    // Vanilla object references
    public static GameObject? spaceProps = null!;

    // Spawned objects
    public static GameObject? outsideSkybox;

    public static readonly Material?[] DoorMaterials = new Material[2];

    // Various
    private static Coroutine? _windowCoroutine;

    public static ShipWindows Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; private set; }

    private void Awake() {
        Logger = base.Logger;
        Instance = this;

        Harmony ??= new(MyPluginInfo.PLUGIN_GUID);

        WindowConfig.InitializeConfig(Config);

        if (WindowConfig.enableWindow1.Value is false && WindowConfig.enableWindow2.Value is false
                                                      && WindowConfig.enableWindow3.Value is false
                                                      && WindowConfig.enableWindow4.Value is false) {
            Logger.LogWarning("All windows are disabled. Please enable any window in your settings for this mod to have any effect.");
            return;
        }

        Logger.LogInfo($"\nCurrent settings:\n"
                     + $"    Vanilla Mode:       {WindowConfig.vanillaMode.Value}\n"
                     + $"    Shutters:           {WindowConfig.enableShutter.Value}\n"
                     + $"    Hide Space Props:   {WindowConfig.hideSpaceProps.Value}\n"
                     + $"    Space Sky:          {WindowConfig.spaceOutsideSetting.Value}\n"
                     + $"    Bottom Lights:      {WindowConfig.disableUnderLights.Value}\n"
                     + $"    Posters:            {WindowConfig.dontMovePosters.Value}\n"
                     + $"    Sky Rotation:       {WindowConfig.skyboxRotateSpeed.Value}\n"
                     + $"    Sky Resolution:     {WindowConfig.skyboxResolution.Value}\n"
                     + $"    Windows Unlockable: {WindowConfig.windowsUnlockable.Value}\n"
                     + $"    Window 1 Enabled:   {WindowConfig.enableWindow1.Value}\n"
                     + $"    Window 2 Enabled:   {WindowConfig.enableWindow2.Value}\n"
                     + $"    Window 3 Enabled:   {WindowConfig.enableWindow3.Value}\n"
                     + $"    Window 4 Enabled:   {WindowConfig.enableWindow4.Value}\n");


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

        // I hate this, veri, why???
        _ = new WindowState();

        Harmony.PatchAll(typeof(ShipWindows));
        Harmony.PatchAll(typeof(Unlockables));

        #region EnemyPatches

        Harmony.PatchAll(typeof(EnemyAICollisionDetectPatch));

        Harmony.PatchAll(typeof(EnemyMeshPatch));

        #endregion EnemyPatches

        if (WindowConfig.changeLightSwitchTip.Value) Harmony.PatchAll(typeof(LightSwitchPatch));

        CompatibleDependencyAttribute.Init(this);

        ShipWindow4K.TryToLoad();

        StartCoroutine(SoundLoader.LoadAudioClips());

        WindowConfig.glassMaterial.SettingChanged += (_, _) => ShipReplacer.ReplaceGlassMaterial();

        Logger.LogInfo("Loaded successfully!");
    }

    private static bool LoadAssetBundle() {
        Logger.LogInfo("Loading ShipWindow AssetBundle...");
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Debug.Assert(assemblyLocation != null, nameof(assemblyLocation) + " != null");
        mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation, "ship_window"));

        return mainAssetBundle != null;
    }

    private static GameObject FindOrThrow(string name) {
        var gameObject = GameObject.Find(name);
        if (!gameObject) throw new($"Could not find {name}! Wrong scene?");

        return gameObject;
    }

    private static int GetWindowBaseCost(int id) {
        return id switch {
            1 => WindowConfig.window1Cost.Value,
            2 => WindowConfig.window2Cost.Value,
            3 => WindowConfig.window3Cost.Value,
            4 => WindowConfig.window4Cost.Value,
            var _ => 60, // Shouldn't happen, but just in case.
        };
    }

    public static bool IsWindowEnabled(int id) {
        return id switch {
            1 => WindowConfig.enableWindow1.Value,
            2 => WindowConfig.enableWindow2.Value,
            3 => WindowConfig.enableWindow3.Value,
            4 => WindowConfig.enableWindow4.Value,
            var _ => false,
        };
    }

    public static bool IsWindowDefaultUnlocked(int id) {
        return id switch {
            1 => WindowConfig.defaultWindow1.Value,
            2 => WindowConfig.defaultWindow2.Value,
            3 => WindowConfig.defaultWindow3.Value,
            4 => WindowConfig.defaultWindow4.Value,
            var _ => false,
        };
    }

    private static void RegisterWindows() {
        for (var id = 1; id <= 4; id++) {
            if (!IsWindowEnabled(id)) continue;

            var def = ShipWindowDef.Register(id, GetWindowBaseCost(id));
            WindowRegistry.Add(id, def);
        }
    }

    private static void AddStars() {
        if (Compatibility.CelestialTint.Enabled) return;

        var renderingObject = GameObject.Find("Systems/Rendering");
        var vanillaStarSphere = GameObject.Find("Systems/Rendering/StarsSphere");

        switch (WindowConfig.spaceOutsideSetting.Value) {
            // do nothing
            case SpaceOutside.OTHER_MODS:
                break;

            // spawn Volume sphere
            case SpaceOutside.SPACE_HDRI:
                if (renderingObject == null) throw new("Could not find Systems/Rendering. Wrong scene?");

                var universePrefab = mainAssetBundle.LoadAsset<GameObject>("Assets/LethalCompany/Mods/ShipWindow/UniverseVolume.prefab");

                outsideSkybox = Instantiate(universePrefab, renderingObject.transform);
                vanillaStarSphere.GetComponent<MeshRenderer>().enabled = false;

                outsideSkybox.AddComponent<SpaceSkybox>();

                // Load 4k texture
                if (ShipWindow4K.Skybox4K != null) outsideSkybox.GetComponent<SpaceSkybox>()?.SetSkyboxTexture(ShipWindow4K.Skybox4K);
                break;

            // spawn large star sphere
            case SpaceOutside.BLACK_AND_STARS:
                if (vanillaStarSphere == null) throw new("Could not find vanilla Stars Sphere. Wrong scene?");
                if (renderingObject == null) throw new("Could not find Systems/Rendering. Wrong scene?");

                var starSpherePrefab =
                    mainAssetBundle.LoadAsset<GameObject>("Assets/LethalCompany/Mods/ShipWindow/StarsSphereLarge.prefab");
                if (starSpherePrefab == null) throw new("Could not load star sphere large prefab!");

                outsideSkybox = Instantiate(starSpherePrefab, renderingObject.transform);
                vanillaStarSphere.GetComponent<MeshRenderer>().enabled = false;

                outsideSkybox.AddComponent<SpaceSkybox>();

                break;
            default:
                throw new ArgumentOutOfRangeException(WindowConfig.spaceOutsideSetting.Value + " is not a valid option!");
        }
    }

    private static void HideSpaceProps() {
        if (Compatibility.CelestialTint.Enabled) return;

        if (!WindowConfig.hideSpaceProps.Value) return;

        var props = GameObject.Find("Environment/SpaceProps");
        props?.SetActive(false);
    }

    private static void HideMiscMeshes() {
        /*
         * Thanks to sf Desat for finding these.
         */
        var notSpawnedPlatform1 = GameObject.Find("notSpawnedPlatform");
        var notSpawnedPlatform2 = GameObject.Find("notSpawnedPlatform (1)");

        var renderer1 = notSpawnedPlatform1?.GetComponent<MeshRenderer>();
        var renderer2 = notSpawnedPlatform2?.GetComponent<MeshRenderer>();
        if (renderer1 != null) renderer1.enabled = false;
        if (renderer2 != null) renderer2.enabled = false;
    }

    public static void OpenWindowDelayed(float delay) {
        if (_windowCoroutine != null) StartOfRound.Instance.StopCoroutine(_windowCoroutine);
        _windowCoroutine = StartOfRound.Instance.StartCoroutine(OpenWindowCoroutine(delay));
    }

    public static void OpenWindowOnCondition(Func<bool> conditionPredicate) {
        if (_windowCoroutine != null) StartOfRound.Instance.StopCoroutine(_windowCoroutine);
        _windowCoroutine = StartOfRound.Instance.StartCoroutine(OpenWindowOnConditionCoroutine(conditionPredicate));
    }

    private static IEnumerator OpenWindowCoroutine(float delay) {
        Logger.LogInfo("Opening window in " + delay + " seconds...");
        yield return new WaitForSeconds(delay);
        WindowState.Instance.SetWindowState(false, false, WindowConfig.playShutterVoiceLinesOnTransitions.Value);
        _windowCoroutine = null;
    }

    private static IEnumerator OpenWindowOnConditionCoroutine(Func<bool> conditionPredicate) {
        Logger.LogInfo("Opening window when " + conditionPredicate + " is true");
        yield return new WaitUntil(conditionPredicate);
        WindowState.Instance.SetWindowState(false, false, WindowConfig.playShutterVoiceLinesOnTransitions.Value);
        _windowCoroutine = null;
    }

    private static void HandleWindowSync() => WindowState.Instance.ReceiveSync();

    //TODO: Move patches

    // ==============================================================================
    // Patches
    // ==============================================================================

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
    private static void AddPrefabsToNetwork() {
        if (WindowConfig.vanillaMode.Value) return;

        var shutterSwitchAsset = mainAssetBundle.LoadAsset<GameObject>("Assets/LethalCompany/Mods/ShipWindow/WindowShutterSwitch.prefab");
        shutterSwitchAsset.AddComponent<ShipWindowShutterSwitch>();
        NetworkManager.Singleton.AddNetworkPrefab(shutterSwitchAsset);

        windowSwitchPrefab = shutterSwitchAsset;

        RegisterWindows();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ChangeLevelServerRpc))]
    private static void LockWindowsWhileRouting(int levelID) {
        if (!WindowConfig.shuttersHideMoonTransitions.Value) return;

        var moons = StartOfRound.Instance.levels;
        moons ??= [
        ];

        var selectedLevel =
            moons.Where(level => level != null).FirstOrDefault(selectableLevel => selectableLevel.levelID == levelID);

        if (selectedLevel == null) return;

        WindowState.Instance.SetWindowState(true, true, WindowConfig.playShutterVoiceLinesOnTransitions.Value);

        OpenWindowDelayed(selectedLevel.timeToArrive + 2.5F);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Terminal), nameof(Terminal.Awake))]
    // ReSharper disable once InconsistentNaming
    private static void AddWindowsToUnlockables(Terminal __instance) {
        try {
            if (WindowConfig.windowsUnlockable.Value is false || WindowConfig.vanillaMode.Value) return;

            foreach (var entry in WindowRegistry) {
                var id = Unlockables.AddWindowToUnlockables(__instance, entry.Value);
                entry.Value.unlockableID = id;
            }
        } catch (Exception e) {
            Logger.LogError($"Error occurred registering window unlockables...\n{e}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Awake))]
    private static void SpawnShutterSwitch() {
        try {
            if (!WindowConfig.vanillaMode.Value) {
                // The switch will be removed by a later function if it is not needed
                // Spawning here will let the (potential) saved position be restored.
                Unlockables.AddSwitchToUnlockables();
                //ShipReplacer.SpawnSwitch();
                StartOfRound.Instance.StartCoroutine(ShipReplacer.WaitAndCheckSwitch());
            }


            // The debounce coroutine is cancelled when quitting the game because StartOfRound is destroyed.
            // This means the flag doesn't get reset. So, we have to manually reset it at the start.
            ShipReplacer.debounceReplace = false;
        } catch (Exception e) {
            Logger.LogError(e);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Start))]
    private static void InitializeWindows() {
        try {
            if (WindowConfig.windowsUnlockable.Value == false || WindowConfig.vanillaMode.Value) ShipReplacer.ReplaceShip();

            AddStars();
            HideSpaceProps();
            HideMiscMeshes();
        } catch (Exception e) {
            Logger.LogError(e);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    private static void OnPlayerConnect() {
        NetworkHandler.RegisterMessages();
        NetworkHandler.WindowSyncReceivedEvent += HandleWindowSync;

        if (NetworkHandler.IsHost) return;

        NetworkHandler.RequestWindowSync();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.StartDisconnect))]
    private static void OnPlayerDisconnect() {
        NetworkHandler.UnregisterMessages();
        NetworkHandler.WindowSyncReceivedEvent -= HandleWindowSync;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.LateUpdate))]
    private static void FollowPlayer() {
        if (Compatibility.CelestialTint.Enabled) return;
        // Make the stars follow the player when they get sucked out of the ship.
        if (outsideSkybox == null) return;

        if (StartOfRound.Instance.suckingPlayersOutOfShip)
            outsideSkybox.transform.position = GameNetworkManager.Instance.localPlayerController.transform.position;
        else outsideSkybox.transform.localPosition = Vector3.zero;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartMatchLever), nameof(StartMatchLever.PullLeverAnim))]
    private static void CloseAndLockWindows(bool leverPulled) {
        //Logger.LogInfo($"StartMatchLever.StartGame -> Is Host:{NetworkHandler.IsHost} / Is Client:{NetworkHandler.IsClient} ");
        if (!leverPulled) return;

        WindowState.Instance.SetWindowState(true, true, WindowConfig.playShutterVoiceLinesOnTransitions.Value);
    }

    // TODO: This does not need to be networked anymore.
    [HarmonyPostfix]
    [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.FinishGeneratingNewLevelClientRpc))]
    private static void OpenWindowAfterLevelGeneration() {
        //Logger.LogInfo($"RoundManager.FinishGeneratingNewLevelClientRpc -> Is Host:{NetworkHandler.IsHost} / Is Client:{NetworkHandler.IsClient} ");
        // Increased the delay to 3 seconds, in hopes to combat windows being open before ship is ready to land
        //OpenWindowDelayed(3f);

        WindowState.Instance.SetVolumeState(false);

        OpenWindowOnCondition(() => StartOfRound.Instance.shipDoorsEnabled && !StartOfRound.Instance.inShipPhase);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ShipHasLeft))]
    private static void OpenWindowAfterShipLeave() {
        //Logger.LogInfo($"StartOfRound.ShipHasLeft -> Is Host:{NetworkHandler.IsHost} / Is Client:{NetworkHandler.IsClient} ");
        WindowState.Instance.SetWindowState(true, true, WindowConfig.playShutterVoiceLinesOnTransitions.Value);
        //OpenWindowDelayed(5f);

        OpenWindowOnCondition(() => StartOfRound.Instance.inShipPhase);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ResetShip))]
    private static void CheckForKeptSpawners() => StartOfRound.Instance.StartCoroutine(ShipReplacer.CheckForKeptSpawners());

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DespawnPropsAtEndOfRound))]
    private static void Patch_DespawnProps() {
        //Logger.LogInfo($"RoundManager.DespawnPropsAtEndOfRound -> Is Host:{NetworkHandler.IsHost} / Is Client:{NetworkHandler.IsClient} ");

        if (Compatibility.CelestialTint.Enabled) return;

        try {
            switch (WindowConfig.spaceOutsideSetting.Value) {
                case SpaceOutside.OTHER_MODS: break;

                case SpaceOutside.SPACE_HDRI:
                case SpaceOutside.BLACK_AND_STARS:
                    // If for whatever reason this code errors, the game breaks.
                    var daysSpent = StartOfRound.Instance.gameStats?.daysSpent;
                    var rotation = (daysSpent ?? 1) * 80f;
                    WindowState.Instance.SetVolumeRotation(rotation);
                    WindowState.Instance.SetVolumeState(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(WindowConfig.spaceOutsideSetting.Value + " is not a valid option!");
            }

            var props = GameObject.Find("Environment/SpaceProps");
            if (props == null || !WindowConfig.hideSpaceProps.Value) return;

            props.SetActive(false);
        } catch (Exception e) {
            Logger.LogError(e);
        }
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
}