using BepInEx.Configuration;

namespace ShipWindows.Config;

public static class WindowConfig {
    public static ConfigEntry<bool> vanillaMode = null!;

    public static ConfigEntry<WindowMaterial> glassMaterial = null!;

    public static ConfigEntry<bool> enableShutter = null!;
    public static ConfigEntry<bool> shuttersHideMoonTransitions = null!;
    public static ConfigEntry<bool> hideSpaceProps = null!;
    public static ConfigEntry<SpaceOutside> spaceOutsideSetting = null!;
    public static ConfigEntry<bool> enableUnderLights = null!;
    public static ConfigEntry<bool> dontMovePosters = null!;
    public static ConfigEntry<float> skyboxRotateSpeed = null!;

    public static ConfigEntry<bool> changeLightSwitchTip = null!;

    public static ConfigEntry<bool> enableShutterVoiceLines = null!;
    public static ConfigEntry<bool> playShutterVoiceLinesOnTransitions = null!;

    public static ConfigEntry<bool> enableShutterSwitchScanNode = null!;

    public static ConfigEntry<bool> makeWesleySellAudioRare = null!;
    public static ConfigEntry<bool> enableWesleySellAudio = null!;

    public static ConfigEntry<bool> celestialTintOverrideSpace = null!;

    public static ConfigEntry<bool> enableEnemyFix = null!;

    public static ConfigEntry<bool> allowEnemyTriggerThroughWindows = null!;

    public static void InitializeConfig(ConfigFile configFile) {
        vanillaMode = configFile.Bind("General", "VanillaMode", false,
                                      "Enable this to preserve vanilla network compatability. This will disable unlockables and the shutter toggle switch. (default = false)");


        glassMaterial = configFile.Bind("Windows", "Window Material", WindowMaterial.NO_REFRACTION_IRIDESCENCE,
                                        "Defines what material will be used for the glass. Iridescence will give you some nice rainbow colors."
                                      + " They are more visible with Refraction, but Refraction breaks some VFX.");

        allowEnemyTriggerThroughWindows = configFile.Bind("Windows", "Allow Enemy Trigger Through Windows", true,
                                                          "If set to true, will allow you to trigger enemies through windows. "
                                                        + "Will also allow enemies to see you through windows. Does not have any effect on vanilla mode.");


        enableShutter = configFile.Bind("General", "EnableWindowShutter", true,
                                        "Enable the window shutter to hide transitions between space and the current moon.");

        shuttersHideMoonTransitions = configFile.Bind("Misc", "Shutters hide moon transitions", true,
                                                      "If set to true, will close the window shutters when routing to a new moon."
                                                    + "Disabling this will look weird, if CelestialTint isn't installed.");


        spaceOutsideSetting = configFile.Bind("Skybox", "Skybox Type", SpaceOutside.SPACE_HDRI, "Set this value to control how the outside space looks.");

        celestialTintOverrideSpace = configFile.Bind("Skybox", "Override Celestial Tint Skybox", true,
                                                     "If Celestial Tint is installed, override the skybox. "
                                                   + "Only effective if skybox is set to Space HDRRI Volume.");

        skyboxRotateSpeed = configFile.Bind("Skybox", "Skybox Rotation Speed", 0.1f,
                                            new ConfigDescription("Sets the rotation speed of the space skybox for visual effect.",
                                                                  new AcceptableValueRange<float>(-1F, 1F)));

        hideSpaceProps = configFile.Bind("Skybox", "HideSpaceProps", false, "Should the planet and moon outside the ship be hidden?");


        dontMovePosters = configFile.Bind("General", "DontMovePosters", false, "Don't move the poster that blocks the second window if set to true.");

        changeLightSwitchTip = configFile.Bind("Misc", "Change light switch tool tip", true,
                                               "If set to true, will change the tool tip for the light switch to match the shutter's tool tip.");

        enableShutterVoiceLines = configFile.Bind("Misc", "Enable Wesley shutter voice lines", true,
                                                  "If set to true, will load and use Wesley's voice lines for opening/closing the window shutters.");
        playShutterVoiceLinesOnTransitions = configFile.Bind("Misc", "Play Wesley shutter voice lines on transitions", true,
                                                             "If set to true, will play the voice lines, if opening/closing the window shutters is caused by a transition.");

        enableShutterSwitchScanNode = configFile.Bind("Misc", "Enable Shutter Switch scan node", true,
                                                      "If set to true, will enable the scan node for the shutter switch.");

        makeWesleySellAudioRare = configFile.Bind("Misc", "Make Wesley Sell Audio Rare", false,
                                                  "If set to true, will add the wesley sell audio to the rare audio list.");

        enableWesleySellAudio = configFile.Bind("Misc", "Enable Wesley Sell Audio", true, "If set to true, will add the wesley sell audio to the audio list.");


        enableEnemyFix = configFile.Bind("Fixes", "Enable Enemy Fix", true,
                                         "If set to true, will add a check to enemy's ai to prevent them from killing you through the windows. "
                                       + "Enabling this might cause some issues though.");
    }
}