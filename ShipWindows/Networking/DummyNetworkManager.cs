using ShipWindows.Api;
using Unity.Netcode;

namespace ShipWindows.Networking;

public class DummyNetworkManager : INetworkManager {
    public NetworkObject NetworkObject => null!;

    public void SpawnWindow(WindowInfo windowInfo) {
    }

    public void ToggleShutters(bool closeShutters, bool lockShutters = false) {
        ToggleShutterOnLocalClient(closeShutters, lockShutters);
    }

    private static void ToggleShutterOnLocalClient(bool closeShutters, bool lockShutters) {
        var windows = ShipWindows.windowManager.spawnedWindows;

        foreach (var window in windows) window.ToggleWindowShutter(closeShutters, lockShutters);
    }

    public void SyncUnlockedWindows() {
    }

    public void SyncSkyboxRotation() {
    }

    public void SyncShutter() {
    }
}