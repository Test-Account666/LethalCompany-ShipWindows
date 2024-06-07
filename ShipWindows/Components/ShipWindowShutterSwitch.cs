using GameNetcodeStuff;
using ShipWindows.Networking;
using Unity.Netcode;
using UnityEngine;

namespace ShipWindows.Components;

[AddComponentMenu("TestAccount666/ShipWindows/ShipWindowShutterSwitch")]
public class ShipWindowShutterSwitch : NetworkBehaviour {
    public InteractTrigger interactTrigger;
    
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        interactTrigger.onInteract.AddListener(PlayerUsedSwitch);
    }

    public void PlayerUsedSwitch(PlayerControllerB playerControllerB) =>
        NetworkHandler.WindowSwitchUsed(WindowState.Instance.windowsClosed);
}