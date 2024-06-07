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
    private bool destroy;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        interactTrigger.onInteract.AddListener(PlayerUsedSwitch);

        StartCoroutine(SyncInteractable());
    }

    public override void OnNetworkDespawn() {
        destroy = true;

        base.OnNetworkDespawn();
    }

    public override void OnDestroy() {
        destroy = true;

        base.OnDestroy();
    }

    private IEnumerator SyncInteractable() {
        var currentlyLocked = WindowState.Instance.windowsLocked;

        yield return new WaitUntil(() => destroy || WindowState.Instance.windowsLocked != currentlyLocked);

        if (destroy) yield break;

        interactTrigger.interactable = !WindowState.Instance.windowsLocked;

        StartCoroutine(SyncInteractable());
    }

    public void PlayerUsedSwitch(PlayerControllerB playerControllerB) {
        var windowState = animator.GetBool(_OnHash);

        NetworkHandler.WindowSwitchUsed(windowState);
    }
}