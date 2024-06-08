using System;
using System.Collections;
using GameNetcodeStuff;
using ShipWindows.Networking;
using Unity.Netcode;
using UnityEngine;

namespace ShipWindows.Components;

[AddComponentMenu("TestAccount666/ShipWindows/ShipWindowShutterSwitch")]
public class ShipWindowShutterSwitch : NetworkBehaviour {
    public InteractTrigger? interactTrigger;
    public Animator animator;
    private static readonly int _OnHash = Animator.StringToHash("on");
    private bool _destroy;

    public override void OnNetworkSpawn() {
        if (_destroy) return;

        base.OnNetworkSpawn();

        interactTrigger ??= GetComponent<InteractTrigger>();

        if (interactTrigger is null)
            throw new("Could not find InteractTrigger!");

        if (interactTrigger?.onInteract is null)
            throw new("Could not find onInteract in InteractTrigger???");

        interactTrigger.onInteract.AddListener(PlayerUsedSwitch);

        StartCoroutine(SyncInteractable());
    }

    public override void OnNetworkDespawn() {
        _destroy = true;

        base.OnNetworkDespawn();
    }

    public override void OnDestroy() {
        _destroy = true;

        base.OnDestroy();
    }

    private IEnumerator SyncInteractable() {
        var currentlyLocked = WindowState.Instance.windowsLocked;

        yield return new WaitUntil(() => _destroy || WindowState.Instance.windowsLocked != currentlyLocked);

        if (_destroy) yield break;

        interactTrigger ??= GetComponent<InteractTrigger>();

        if (interactTrigger is null)
            throw new("Could not find InteractTrigger!");

        if (interactTrigger?.onInteract is null)
            throw new("Could not find onInteract in InteractTrigger???");

        interactTrigger.interactable = !WindowState.Instance.windowsLocked;

        StartCoroutine(SyncInteractable());
    }

    public void PlayerUsedSwitch(PlayerControllerB playerControllerB) {
        var windowState = animator.GetBool(_OnHash);

        NetworkHandler.WindowSwitchUsed(windowState);
    }
}