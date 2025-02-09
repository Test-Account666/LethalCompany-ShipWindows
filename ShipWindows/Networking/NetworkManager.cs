using System.Linq;
using ShipWindows.Api;
using ShipWindows.ShutterSwitch;
using Unity.Netcode;

namespace ShipWindows.Networking;

public class NetworkManager : NetworkBehaviour, INetworkManager {
    public override void OnNetworkSpawn() {
        ShipWindows.networkManager = this;

        NetworkObject = ((NetworkBehaviour) this).NetworkObject;

        SyncUnlockedWindows();
        SyncSkyboxRotation();
        SyncShutter();
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
        if (!IsHost && !IsServer) {
            ToggleShuttersServerRpc(closeShutters);
            return;
        }

        ToggleShuttersClientRpc(closeShutters, lockShutters);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ToggleShuttersServerRpc(bool closeShutters) {
        ToggleShutters(closeShutters);
    }

    [ClientRpc]
    public void ToggleShuttersClientRpc(bool closeShutters, bool lockShutters = false) {
        ToggleShutterOnLocalClient(closeShutters, lockShutters);
    }

    private static void ToggleShutterOnLocalClient(bool closeShutters, bool lockShutters) {
        var windows = ShipWindows.windowManager.spawnedWindows;

        foreach (var window in windows) window.ToggleWindowShutter(closeShutters, lockShutters);

        if (!ShutterSwitchBehavior.Instance) return;

        ShutterSwitchBehavior.Instance.ToggleSwitch(closeShutters, lockShutters);
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

    public void SyncShutter() {
        if (!IsHost && !IsServer) {
            SyncShutterServerRpc();
            return;
        }

        var shutterSwitch = ShutterSwitchBehavior.Instance;

        if (!shutterSwitch) return;

        var closeShutters = shutterSwitch.animator.GetBool(ShutterSwitchBehavior.EnabledAnimatorHash);
        var lockShutters = !shutterSwitch.interactTrigger.interactable;

        SyncShutterClientRpc(closeShutters, lockShutters);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncShutterServerRpc() {
        SyncShutter();
    }

    [ClientRpc]
    public void SyncShutterClientRpc(bool closeShutters, bool lockShutters = false) {
        ToggleShutterOnLocalClient(closeShutters, lockShutters);
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