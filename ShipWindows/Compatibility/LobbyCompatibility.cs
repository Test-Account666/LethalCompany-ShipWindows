using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;

namespace ShipWindows.Compatibility;

public class LobbyCompatibility {
    public static bool Enabled { get; private set; }

    private static bool Initialize() {
        if (Enabled) return false;
        Enabled = true;

        ShipWindows.Logger.LogInfo("Found LobbyCompatibility Mod, initializing support :)");

        PluginHelper.RegisterPlugin(MyPluginInfo.PLUGIN_GUID, new(MyPluginInfo.PLUGIN_VERSION), CompatibilityLevel.ClientOnly,
                                    VersionStrictness.Minor);

        return true;
    }
}