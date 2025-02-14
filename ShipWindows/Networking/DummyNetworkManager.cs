using System.Collections;
using ShipWindows.Api;
using ShipWindows.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace ShipWindows.Networking;

public class DummyNetworkManager : INetworkManager {
    public NetworkObject NetworkObject => null!;

    public void SpawnWindow(WindowInfo windowInfo) {
    }

    public void ToggleShutters(bool closeShutters, bool lockShutters = false, bool playAudio = false) {
        if (playAudio) PlayWesleyVoice(closeShutters? 1 : 0);
        ToggleShutterOnLocalClient(closeShutters, lockShutters);
    }

    private static void ToggleShutterOnLocalClient(bool closeShutters, bool lockShutters) {
        var windows = ShipWindows.windowManager.spawnedWindows;

        foreach (var window in windows) window.ToggleWindowShutter(closeShutters, lockShutters);
    }

    public void SyncUnlockedWindows() {
    }

    public void SyncSkyboxRotation() {
    }

    public void SyncShutter() {
    }

    public void PlayWesleyVoice(int index) => StartOfRound.Instance.StartCoroutine(PlayWesleyVoiceCoroutine(index));

    private static IEnumerator PlayWesleyVoiceCoroutine(int index) {
        var speakerAudio = StartOfRound.Instance.speakerAudioSource;

        if (speakerAudio.isPlaying) {
            StartOfRound.Instance.speakerAudioSource.Stop();
            StartOfRound.Instance.speakerAudioSource.PlayOneShot(StartOfRound.Instance.disableSpeakerSFX);
        }

        yield return new WaitUntil(() => !speakerAudio.isPlaying);

        speakerAudio.PlayOneShot(SoundLoader.VoiceLines[index]);
    }
}