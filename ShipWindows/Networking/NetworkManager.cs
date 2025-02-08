using System.Linq;
using ShipWindows.Api;
using Unity.Netcode;

namespace ShipWindows.Networking;

public class NetworkManager : NetworkBehaviour, INetworkManager {
    public override void OnNetworkSpawn() {
        ShipWindows.networkManager = this;

        NetworkObject = ((NetworkBehaviour) this).NetworkObject;

        SyncUnlockedWindows();
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
            ShipWindows.Logger.LogFatal("Requesting sync!");
            SyncUnlockedWindowsServerRpc();
            return;
        }

        foreach (var unlockedWindow in WindowUnlockData.UnlockedWindows) SpawnWindowClientRpc(unlockedWindow);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncUnlockedWindowsServerRpc() {
        ShipWindows.Logger.LogFatal("Sync requested!");
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

        ShipWindows.Logger.LogFatal($"Trying to unlock {windowName}!");

        var window = ShipWindows.windowRegistry.windows.FirstOrDefault(info => info.windowName.Equals(windowName));

        ShipWindows.Logger.LogFatal($"Found window? {window && true}");

        if (!window) return;

        SpawnWindowOnLocalClient(window!);
    }
}