using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;

namespace ShipWindows.Dependencies;

internal static class LobbyCompatibilitySupport {
    internal static void Initialize() =>
        PluginHelper.RegisterPlugin(MyPluginInfo.PLUGIN_GUID, new(MyPluginInfo.PLUGIN_VERSION), CompatibilityLevel.ClientOnly,
                                    VersionStrictness.Minor);
}