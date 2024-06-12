using System.Collections;
using GameNetcodeStuff;
using ShipWindows.Networking;
using Unity.Netcode;
using UnityEngine;

namespace ShipWindows.Components;

[AddComponentMenu("TestAccount666/ShipWindows/ShipWindowShutterSwitch")]
public class ShipWindowShutterSwitch : NetworkBehaviour {
    public InteractTrigger? interactTrigger;
    public Animator? animator;
    private static readonly int _OnHash = Animator.StringToHash("on");
    private bool _destroy;

    [SerializeField]
    private GameObject? scanNodeObject;

    public override void OnNetworkSpawn() {
        if (_destroy) return;

        base.OnNetworkSpawn();

        StartCoroutine(AddSwitchListener());

        StartCoroutine(UpdateScanNodeOnce());

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

    private IEnumerator AddSwitchListener() {
        yield return new WaitUntil(() => interactTrigger is not null);

        interactTrigger?.onInteract.AddListener(PlayerUsedSwitch);

        ShipWindows.Logger.LogDebug("Added listener! :)");
    }

    private IEnumerator SyncInteractable() {
        var currentlyLocked = WindowState.Instance.windowsLocked;

        yield return new WaitUntil(() => _destroy || WindowState.Instance.windowsLocked != currentlyLocked);

        if (_destroy) yield break;

        yield return new WaitUntil(() => _destroy || interactTrigger is not null);

        if (_destroy) yield break;

        if (interactTrigger is null)
            throw new("Could not find InteractTrigger!");

        interactTrigger.interactable = !WindowState.Instance.windowsLocked;

        StartCoroutine(SyncInteractable());
    }

    public void PlayerUsedSwitch(PlayerControllerB playerControllerB) {
        if (animator is null) return;

        var windowState = animator.GetBool(_OnHash);

        NetworkHandler.WindowSwitchUsed(windowState);

        UpdateScanNode();
    }

    private IEnumerator UpdateScanNodeOnce() {
        yield return new WaitUntil(() => scanNodeObject is not null);

        ShipWindows.Logger.LogDebug("Updating Scan Node! :>");

        UpdateScanNode();
    }

    private void UpdateScanNode() {
        if (scanNodeObject is null) {
            ShipWindows.Logger.LogError("Couldn't find ScanNode object for ShutterSwitch???");
            return;
        }

        if (scanNodeObject.activeSelf == WindowConfig.enableShutterSwitchScanNode.Value) return;

        scanNodeObject.SetActive(WindowConfig.enableShutterSwitchScanNode.Value);
    }
}