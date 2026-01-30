// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using System.Collections;
using System.Linq;
using ShipWindows.Api;
using ShipWindows.ShutterSwitch;
using ShipWindows.Utilities;
using Unity.Netcode;
using UnityEngine;

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

    public void ToggleShutters(bool closeShutters, bool lockShutters = false, bool playAudio = false) {
        if (!IsHost && !IsServer) {
            ToggleShuttersServerRpc(closeShutters);
            return;
        }

        if (playAudio) PlayWesleyVoice(closeShutters? 1 : 0);
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
        if (!ShipWindows.skyBox) return;

        if (!IsHost && !IsServer) {
            SyncSkyboxRotationServerRpc();
            return;
        }

        SyncSkyboxRotationClientRpc(ShipWindows.skyBox!.CurrentRotation);
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

    public void PlayWesleyVoice(int index) {
        if (!IsHost && !IsServer) {
            PlayWesleyVoiceServerRpc(index);
            return;
        }

        PlayWesleyVoiceClientRpc(index);
    }

    private static IEnumerator PlayWesleyVoiceCoroutine(int index) {
        var speakerAudio = StartOfRound.Instance.speakerAudioSource;

        if (speakerAudio.isPlaying) {
            StartOfRound.Instance.speakerAudioSource.Stop();
            StartOfRound.Instance.speakerAudioSource.PlayOneShot(StartOfRound.Instance.disableSpeakerSFX);
        }

        yield return new WaitUntil(() => !speakerAudio.isPlaying);

        speakerAudio.PlayOneShot(SoundLoader.VoiceLines[index]);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayWesleyVoiceServerRpc(int index) {
        PlayWesleyVoiceClientRpc(index);
    }

    [ClientRpc]
    public void PlayWesleyVoiceClientRpc(int index) {
        StopAllCoroutines();
        StartCoroutine(PlayWesleyVoiceCoroutine(index));
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
        if (!ShipWindows.skyBox) return;

        ShipWindows.skyBox!.CurrentRotation = rotation;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncUnlockedWindowsServerRpc() {
        SyncUnlockedWindows();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnWindowServerRpc(string windowName) {
        var window = ShipWindows.windowRegistry.Windows.FirstOrDefault(info => info.windowName.Equals(windowName));
        if (!window) return;

        SpawnWindow(window!);
    }

    [ClientRpc]
    public void SpawnWindowClientRpc(string windowName) {
        if (IsHost || IsServer) return;

        var window = ShipWindows.windowRegistry.Windows.FirstOrDefault(info => info.windowName.Equals(windowName));

        if (!window) return;

        SpawnWindowOnLocalClient(window!);
    }
}