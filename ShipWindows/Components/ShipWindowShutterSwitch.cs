using System;
using System.Collections;
using GameNetcodeStuff;
using ShipWindows.Networking;
using Unity.Netcode;
using UnityEngine;

namespace ShipWindows.Components;

[AddComponentMenu("TestAccount666/ShipWindows/ShipWindowShutterSwitch")]
public class ShipWindowShutterSwitch : NetworkBehaviour {
    public InteractTrigger interactTrigger;
    public Animator animator;
    private static readonly int _OnHash = Animator.StringToHash("on");

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        interactTrigger.onInteract.AddListener(PlayerUsedSwitch);
    }

    public void PlayerUsedSwitch(PlayerControllerB playerControllerB) {
        var windowState = animator.GetBool(_OnHash);

        NetworkHandler.WindowSwitchUsed(windowState);
    }
}