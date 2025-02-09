using ShipWindows.Api;
using Unity.Netcode;

namespace ShipWindows.Networking;

public interface INetworkManager {
    public NetworkObject NetworkObject { get; }

    public void SpawnWindow(WindowInfo windowInfo);

    public void ToggleShutters(bool closeShutters, bool lockShutters = false);

    public void SyncUnlockedWindows();

    public void SyncSkyboxRotation();

    public void SyncShutter();
}