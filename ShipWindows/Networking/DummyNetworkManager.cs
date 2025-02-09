using ShipWindows.Api;
using Unity.Netcode;

namespace ShipWindows.Networking;

public class DummyNetworkManager : INetworkManager {
    public NetworkObject NetworkObject => null!;

    public void SpawnWindow(WindowInfo windowInfo) {
    }

    public void ToggleShutters(bool closeShutters, bool lockShutters = false) {
    }

    public void SyncUnlockedWindows() {
    }

    public void SyncSkyboxRotation() {
    }
}