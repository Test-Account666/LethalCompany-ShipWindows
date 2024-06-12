using BepInEx.Configuration;

namespace ShipWindows;

public static class WindowConfig {
    public static ConfigEntry<bool> vanillaMode = null!;

    public static ConfigEntry<WindowMaterial> glassMaterial = null!;

    public static ConfigEntry<bool> enableShutter = null!;
    public static ConfigEntry<bool> shuttersHideMoonTransitions = null!;
    public static ConfigEntry<bool> hideSpaceProps = null!;
    public static ConfigEntry<SpaceOutside> spaceOutsideSetting = null!;
    public static ConfigEntry<bool> disableUnderLights = null!;
    public static ConfigEntry<bool> dontMovePosters = null!;
    public static ConfigEntry<float> skyboxRotateSpeed = null!;
    public static ConfigEntry<int> skyboxResolution = null!;

    public static ConfigEntry<bool> windowsUnlockable = null!;
    public static ConfigEntry<int> window1Cost = null!;
    public static ConfigEntry<int> window2Cost = null!;
    public static ConfigEntry<int> window3Cost = null!;
    public static ConfigEntry<int> window4Cost = null!;

    public static ConfigEntry<bool> enableWindow1 = null!;
    public static ConfigEntry<bool> enableWindow2 = null!;
    public static ConfigEntry<bool> enableWindow3 = null!;
    public static ConfigEntry<bool> enableWindow4 = null!;

    public static ConfigEntry<bool> defaultWindow1 = null!;
    public static ConfigEntry<bool> defaultWindow2 = null!;
    public static ConfigEntry<bool> defaultWindow3 = null!;
    public static ConfigEntry<bool> defaultWindow4 = null!;

    public static ConfigEntry<bool> changeLightSwitchTip = null!;

    public static ConfigEntry<bool> enableShutterVoiceLines = null!;
    public static ConfigEntry<bool> playShutterVoiceLinesOnTransitions = null!;

    public static ConfigEntry<bool> enableShutterSwitchScanNode = null!;

    public static ConfigEntry<bool> celestialTintOverrideSpace = null!;

    public static ConfigEntry<bool> enableEnemyFix = null!;

    public static void InitializeConfig(ConfigFile configFile) {
        vanillaMode = configFile.Bind("General", "VanillaMode", false,
                                      "Enable this to preserve vanilla network compatability. This will disable unlockables and the shutter toggle switch. (default = false)");

        glassMaterial = configFile.Bind("General", "WindowMaterial", WindowMaterial.NO_REFRACTION_IRIDESCENCE,
                                        "Defines what material will be used for the glass. Iridescence will give you some nice rainbow colors."
                                      + " They are more visible with Refraction, but Refraction breaks some VFX.");

        enableShutter = configFile.Bind("General", "EnableWindowShutter", true,
                                        "Enable the window shutter to hide transitions between space and the current moon. (default = true)");
        shuttersHideMoonTransitions = configFile.Bind("Misc", "Shutters hide moon transitions", true,
                                                      "If set to true, will close the window shutters when routing to a new moon."
                                                    + "Disabling this will look weird, if CelestialTint isn't installed.");

        hideSpaceProps = configFile.Bind("General", "HideSpaceProps", false,
                                         "Should the planet and moon outside the ship be hidden?");


        spaceOutsideSetting = configFile.Bind("General", "SpaceOutside", SpaceOutside.SPACE_HDRI,
                                              "Set this value to control how the outside space looks.");

        disableUnderLights = configFile.Bind("General", "DisableUnderLights", false,
                                             "Disable the flood lights added under the ship if you have the floor window enabled.");
        dontMovePosters = configFile.Bind("General", "DontMovePosters", false,
                                          "Don't move the poster that blocks the second window if set to true.");
        skyboxRotateSpeed = configFile.Bind("General", "RotateSpaceSkybox", 0.1f,
                                            new ConfigDescription(
                                                "Sets the rotation speed of the space skybox for visual effect. Requires 'SpaceOutside' to be set to 1 or 2.",
                                                new AcceptableValueRange<float>(-1F, 1F)));
        skyboxResolution = configFile.Bind("General", "SkyboxResolution", 0,
                                           "OBSOLETE: Download [Ship Windows 4K Skybox] from the Thunderstore to enable!");

        windowsUnlockable = configFile.Bind("General", "WindowsUnlockable", true,
                                            "Adds the windows to the terminal as ship upgrades. Set this to false and use below settings to have them enabled by default.");
        window1Cost = configFile.Bind("General", "Window1Cost", 60,
                                      "The base cost of the window behind the terminal / right of the switch.");
        window2Cost = configFile.Bind("General", "Window2Cost", 60,
                                      "The base cost of the window across from the terminal / left of the switch.");
        window3Cost = configFile.Bind("General", "Window3Cost", 100,
                                      "The base cost of the large floor window.");
        window4Cost = configFile.Bind("General", "Window4Cost", 75,
                                      "The base cost of the door windows.");

        enableWindow1 = configFile.Bind("General", "EnableWindow1", true,
                                        "Enable the window to the right of the switch, behind the terminal.");
        enableWindow2 = configFile.Bind("General", "EnableWindow2", true,
                                        "Enable the window to the left of the switch, across from the first window.");
        enableWindow3 = configFile.Bind("General", "EnableWindow3", true,
                                        "Enable the large floor window.");
        enableWindow4 = configFile.Bind("General", "EnableWindow4", true,
                                        "Enable the door windows.");

        defaultWindow1 = configFile.Bind("General", "UnlockWindow1", true,
                                         "If set as unlockable, start the game with window to the right of the switch unlocked already.");
        defaultWindow2 = configFile.Bind("General", "UnlockWindow2", false,
                                         "If set as unlockable, start the game with window across from the terminal unlocked already.");
        defaultWindow3 = configFile.Bind("General", "UnlockWindow3", false,
                                         "If set as unlockable, start the game with the floor window unlocked already.");
        defaultWindow4 = configFile.Bind("General", "UnlockWindow4", false,
                                         "If set as unlockable, start the game with the door windows unlocked already.");

        changeLightSwitchTip = configFile.Bind("Misc", "Change light switch tool tip", true,
                                               "If set to true, will change the tool tip for the light switch to match the shutter's tool tip.");

        enableShutterVoiceLines = configFile.Bind("Misc", "Enable Wesley shutter voice lines", true,
                                                  "If set to true, will load and use Wesley's voice lines for opening/closing the window shutters.");
        playShutterVoiceLinesOnTransitions = configFile.Bind("Misc", "Play Wesley shutter voice lines on transitions", true,
                                                             "If set to true, will play the voice lines, if opening/closing the window shutters is caused by a transition.");

        enableShutterSwitchScanNode = configFile.Bind("Misc", "Enable Shutter Switch scan node", true,
                                                      "If set to true, will enable the scan node for the shutter switch.");

        celestialTintOverrideSpace = configFile.Bind("Other Mods", "CelestialTintOverrideSpace", false,
                                                     "If Celestial Tint is installed, override the skybox. "
                                                   + "Only effective if skybox is set to Space HDRRI Volume.");


        enableEnemyFix = configFile.Bind("Fixes", "Enable Enemy Fix", true,
                                         "If set to true, will add a check to enemy's ai to prevent them from killing you through the windows. "
                                       + "Enabling this might cause some issues though.");
    }
}