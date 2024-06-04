using GameNetcodeStuff;
using ShipWindows.Networking;
using Unity.Netcode;

namespace ShipWindows.Components;

public class ShipWindowShutterSwitch : NetworkBehaviour {
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        var trigger = transform.Find("WindowSwitch");

        var interactable = trigger?.GetComponent<InteractTrigger>();

        interactable?.onInteract.AddListener(PlayerUsedSwitch);
    }

    public void PlayerUsedSwitch(PlayerControllerB playerControllerB) =>
        NetworkHandler.WindowSwitchUsed(WindowState.Instance.windowsClosed);
}