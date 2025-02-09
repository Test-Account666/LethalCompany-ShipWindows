using System.Linq;
using BepInEx.Bootstrap;

namespace ShipWindows.SoftDependencies;

public static class DependencyChecker {
    public static bool IsCelestialTintInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata => metadata.Metadata.GUID.Contains("CelestialTint"));
}