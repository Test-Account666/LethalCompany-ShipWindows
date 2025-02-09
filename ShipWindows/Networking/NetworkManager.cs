using System.Linq;
using ShipWindows.Api;
using Unity.Netcode;

namespace ShipWindows.Networking;

public class NetworkManager : NetworkBehaviour, INetworkManager {
    public override void OnNetworkSpawn() {
        ShipWindows.networkManager = this;

        NetworkObject = ((NetworkBehaviour) this).NetworkObject;

        SyncUnlockedWindows();
        SyncSkyboxRotation();
    }

    public override void OnNetworkDespawn() => ShipWindows.networkManager = null;

    public new NetworkObject NetworkObject { get; private set; } = null!;

    public void SpawnWindow(WindowInfo windowInfo) {
        if (!IsHost && !IsServer) {
            SpawnWindowServerRpc(windowInfo.windowName);
            return;
        }

        var terminal = HUDManager.Instance.terminalScript;

        if (terminal.groupCredits < windowInfo.cost) return;

        terminal.SyncGroupCreditsClientRpc(terminal.groupCredits - windowInfo.cost, terminal.numberOfItemsInDropship);

        SpawnWindowOnLocalClient(windowInfo);
        SpawnWindowClientRpc(windowInfo.windowName);
    }

    private static void SpawnWindowOnLocalClient(WindowInfo windowInfo) => ShipWindows.windowManager.CreateWindow(windowInfo);

    public void ToggleShutters(bool closeShutters, bool lockShutters = false) {
        //TODO: Network Shutters
    }

    public void SyncUnlockedWindows() {
        if (!IsHost && !IsServer) {
            SyncUnlockedWindowsServerRpc();
            return;
        }

        foreach (var unlockedWindow in WindowUnlockData.UnlockedWindows) SpawnWindowClientRpc(unlockedWindow);
    }

    public void SyncSkyboxRotation() {
        if (ShipWindows.skyBox == null) return;

        if (!IsHost && !IsServer) {
            SyncSkyboxRotationServerRpc();
            return;
        }

        SyncSkyboxRotationClientRpc(ShipWindows.skyBox.CurrentRotation);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncSkyboxRotationServerRpc() {
        SyncSkyboxRotation();
    }

    [ClientRpc]
    public void SyncSkyboxRotationClientRpc(float rotation) {
        if (ShipWindows.skyBox == null) return;

        ShipWindows.skyBox.CurrentRotation = rotation;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncUnlockedWindowsServerRpc() {
        SyncUnlockedWindows();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnWindowServerRpc(string windowName) {
        var window = ShipWindows.windowRegistry.windows.FirstOrDefault(info => info.windowName.Equals(windowName));
        if (!window) return;

        SpawnWindow(window!);
    }

    [ClientRpc]
    public void SpawnWindowClientRpc(string windowName) {
        if (IsHost || IsServer) return;

        var window = ShipWindows.windowRegistry.windows.FirstOrDefault(info => info.windowName.Equals(windowName));

        if (!window) return;

        SpawnWindowOnLocalClient(window!);
    }
}