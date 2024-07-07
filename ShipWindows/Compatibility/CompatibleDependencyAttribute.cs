using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;

namespace ShipWindows.Compatibility;

// https://discord.com/channels/1168655651455639582/1216761387343151134
// Automatic Soft Dependency Initializer
// by Kittenji
internal class CompatibleDependencyAttribute : BepInDependency {
    public Type handler;
    public Version? versionRequired;

    /// <summary>
    /// Marks this BepInEx.BaseUnityPlugin as soft depenant on another plugin.
    /// The handler type must have an Initialize() method that will automatically be invoked if the compatible dependency is present.
    /// </summary>
    /// <param name="guid">The GUID of the referenced plugin.</param>
    /// <param name="handlerType">The class type that will handle this compatibility. Must contain a private method called Initialize()</param>
    public CompatibleDependencyAttribute(string guid, Type handlerType) : base(guid, DependencyFlags.SoftDependency) =>
        handler = handlerType;

    public CompatibleDependencyAttribute(string guid, string versionRequired, Type handlerType) :
        base(guid, DependencyFlags.SoftDependency) {
        handler = handlerType;
        this.versionRequired = new(versionRequired);
    }

    /// <summary>
    /// Global initializer for this class.
    /// You must call this method from your base plugin Awake method and pass the plugin instance to the source parameter.
    /// </summary>
    /// <param name="source">The source plugin instance with the BepInPlugin attribute.</param>
    public static void Init(BaseUnityPlugin source) {
        const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

        var attributes = source.GetType().GetCustomAttributes<CompatibleDependencyAttribute>();
        foreach (var attribute in attributes) {
            if (!Chainloader.PluginInfos.TryGetValue(attribute.DependencyGUID, out var info))
                continue;

            if (attribute.versionRequired != null && attribute.versionRequired.CompareTo(info.Metadata.Version) > 0) {
                ShipWindows.Logger.LogInfo($"Found compatible mod, but it does not meet version requirements:  {attribute.DependencyGUID
                } {info.Metadata.Version}");
                continue;
            }

            ShipWindows.Logger.LogInfo($"Found compatible mod:  {attribute.DependencyGUID} {info.Metadata.Version}");
            var initialized = (bool) attribute.handler.GetMethod("Initialize", bindingFlags)?.Invoke(null, null)!;

            if (!initialized) {
                ShipWindows.Logger.LogInfo($"Found compatible mod, but patches have already been applied:  {attribute.DependencyGUID
                } {info.Metadata.Version}");
                continue;
            }

            // we do a little hehe
            ShipWindows.Harmony?.PatchAll(attribute.handler);

            attribute.handler = null!;
        }
        // Logger.Info("Compatibility not found: " + attr.DependencyGUID);
    }
}