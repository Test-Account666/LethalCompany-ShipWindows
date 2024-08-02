using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Debug = System.Diagnostics.Debug;

namespace ShipWindows.Utilities;

public static class SoundLoader {
    public static readonly AudioClip[] CommonSellCounterLines = new AudioClip[1];
    public static readonly AudioClip[] RareSellCounterLines = new AudioClip[1];
    public static readonly AudioClip[] VoiceLines = new AudioClip[2];

    public static IEnumerator LoadAudioClips() {
        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Debug.Assert(assemblyDirectory != null, nameof(assemblyDirectory) + " != null");
        var audioPath = Path.Combine(assemblyDirectory, "sounds");

        audioPath = Directory.Exists(audioPath)? audioPath : Path.Combine(assemblyDirectory);

        ShipWindows.Logger.LogInfo("Loading Wesley voice lines...");

        var voiceLinesAudioPath = Path.Combine(audioPath, "voicelines");

        voiceLinesAudioPath = Directory.Exists(voiceLinesAudioPath)? voiceLinesAudioPath : Path.Combine(audioPath);

        LoadShutterCloseClip(voiceLinesAudioPath);

        LoadShutterOpenClip(voiceLinesAudioPath);

        LoadSellCounterClips(voiceLinesAudioPath);
        yield break;
    }

    private static void LoadShutterOpenClip(string voiceLinesAudioPath) {
        var shutterOpenFile = Path.Combine(voiceLinesAudioPath, "ShutterOpen.wav");

        var shutterOpenFileName = Path.GetFileName(shutterOpenFile);

        var shutterOpenVoiceLineAudioClip = LoadAudioClipFromFile(new(shutterOpenFile), shutterOpenFileName[..^4]);

        if (shutterOpenVoiceLineAudioClip == null) {
            ShipWindows.Logger.LogError("Failed to load voice line 'ShutterOpen'!");
            ShipWindows.Logger.LogError($"Path: {voiceLinesAudioPath}");
            return;
        }

        VoiceLines[0] = shutterOpenVoiceLineAudioClip;
        ShipWindows.Logger.LogInfo($"Loaded line '{shutterOpenVoiceLineAudioClip.name}'!");
    }

    private static void LoadShutterCloseClip(string voiceLinesAudioPath) {
        var shutterCloseFile = Path.Combine(voiceLinesAudioPath, "ShutterClose.wav");

        var shutterCloseFileName = Path.GetFileName(shutterCloseFile);

        var shutterCloseVoiceLineAudioClip = LoadAudioClipFromFile(new(shutterCloseFile), shutterCloseFileName[..^4]);

        if (shutterCloseVoiceLineAudioClip == null) {
            ShipWindows.Logger.LogError("Failed to load voice line 'ShutterClose'!");
            ShipWindows.Logger.LogError($"Path: {voiceLinesAudioPath}");
            return;
        }

        VoiceLines[1] = shutterCloseVoiceLineAudioClip;
        ShipWindows.Logger.LogInfo($"Loaded line '{shutterCloseVoiceLineAudioClip.name}'!");
    }

    private static void LoadSellCounterClips(string voiceLinesAudioPath) {
        var sellCounterFile = Path.Combine(voiceLinesAudioPath, "SellCounter1.wav");

        var sellCounterFileName = Path.GetFileName(sellCounterFile);

        var sellCounterAudioClip = LoadAudioClipFromFile(new(sellCounterFile), sellCounterFileName[..^4]);

        if (sellCounterAudioClip == null) {
            ShipWindows.Logger.LogError("Failed to load voice line 'SellCounter1'!");
            ShipWindows.Logger.LogError($"Path: {voiceLinesAudioPath}");
            return;
        }

        if (WindowConfig.makeWesleySellAudioRare.Value) RareSellCounterLines[0] = sellCounterAudioClip;
        else CommonSellCounterLines[0] = sellCounterAudioClip;
        ShipWindows.Logger.LogInfo($"Loaded line '{sellCounterAudioClip.name}'!");
    }

    private static AudioClip? LoadAudioClipFromFile(Uri filePath, string name) {
        using var unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);

        var asyncOperation = unityWebRequest.SendWebRequest();

        while (!asyncOperation.isDone)
            Thread.Sleep(100);

        if (unityWebRequest.result != UnityWebRequest.Result.Success) {
            ShipWindows.Logger.LogError("Failed to load AudioClip: " + unityWebRequest.error);
            return null;
        }

        var clip = DownloadHandlerAudioClip.GetContent(unityWebRequest);

        clip.name = name;

        return clip;
    }
}