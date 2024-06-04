using BepInEx.Configuration;

namespace ShipWindows;

// ReSharper disable once ClassNeverInstantiated.Global
public static class WindowConfig {
    public static ConfigEntry<bool> vanillaMode = null!;
    public static ConfigEntry<bool> glassRefraction = null!;
    public static ConfigEntry<bool> enableShutter = null!;
    public static ConfigEntry<bool> hideSpaceProps = null!;
    public static ConfigEntry<int> spaceOutsideSetting = null!;
    public static ConfigEntry<bool> disableUnderLights = null!;
    public static ConfigEntry<bool> dontMovePosters = null!;
    public static ConfigEntry<bool> rotateSkybox = null!;
    public static ConfigEntry<int> skyboxResolution = null!;

    public static ConfigEntry<bool> windowsUnlockable = null!;
    public static ConfigEntry<int> window1Cost = null!;
    public static ConfigEntry<int> window2Cost = null!;
    public static ConfigEntry<int> window3Cost = null!;

    public static ConfigEntry<bool> enableWindow1 = null!;
    public static ConfigEntry<bool> enableWindow2 = null!;
    public static ConfigEntry<bool> enableWindow3 = null!;

    public static ConfigEntry<bool> defaultWindow1 = null!;
    public static ConfigEntry<bool> defaultWindow2 = null!;
    public static ConfigEntry<bool> defaultWindow3 = null!;

    //public static ConfigEntry<bool> celestialTintOverrideSpace;

    public static void InitializeConfig(ConfigFile configFile) {
        vanillaMode = configFile.Bind("General", "VanillaMode", false,
                                      "Enable this to preserve vanilla network compatability. This will disable unlockables and the shutter toggle switch. (default = false)");
        glassRefraction = configFile.Bind("General", "EnableRefraction", false,
                                          "Enable refraction on the glass shader. (Note: This will cause issues with certain VFX and is performance heavy.)");
        enableShutter = configFile.Bind("General", "EnableWindowShutter", true,
                                        "Enable the window shutter to hide transitions between space and the current moon. (default = true)");
        hideSpaceProps = configFile.Bind("General", "HideSpaceProps", false,
                                         "Should the planet and moon outside the ship be hidden? (default = false)");
        spaceOutsideSetting = configFile.Bind("General", "SpaceOutside", 1,
                                              "Set this value to control how the outside space looks. (0 = Let other mods handle, 1 = Space HDRI Volume (default), 2 = Black sky with stars)");

        disableUnderLights = configFile.Bind("General", "DisableUnderLights", false,
                                             "Disable the flood lights added under the ship if you have the floor window enabled.");
        dontMovePosters = configFile.Bind("General", "DontMovePosters", false,
                                          "Don't move the poster that blocks the second window if set to true.");
        rotateSkybox = configFile.Bind("General", "RotateSpaceSkybox", true,
                                       "Enable slow rotation of the space skybox for visual effect. Requires 'SpaceOutside' to be set to 1 or 2.");
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

        enableWindow1 = configFile.Bind("General", "EnableWindow1", true,
                                        "Enable the window to the right of the switch, behind the terminal.");
        enableWindow2 = configFile.Bind("General", "EnableWindow2", true,
                                        "Enable the window to the left of the switch, across from the first window.");
        enableWindow3 = configFile.Bind("General", "EnableWindow3", true,
                                        "Enable the large floor window.");

        defaultWindow1 = configFile.Bind("General", "UnlockWindow1", true,
                                         "If set as unlockable, start the game with window to the right of the switch unlocked already.");
        defaultWindow2 = configFile.Bind("General", "UnlockWindow2", false,
                                         "If set as unlockable, start the game with window across from the terminal unlocked already.");
        defaultWindow3 = configFile.Bind("General", "UnlockWindow3", false,
                                         "If set as unlockable, start the game with the floor window unlocked already.");

        //celestialTintOverrideSpace = configFile.Bind("Other Mods", "CelestialTintOverrideSpace", false,
        //    "If Celestial Tint is installed, replace the space skybox with the red sky from Ship Windows.");
    }
}