using System;
using ShipWindows.Components;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace ShipWindows.Networking;

[Serializable]
internal class WindowState {
    [FormerlySerializedAs("WindowsClosed")]
    public bool windowsClosed;

    [FormerlySerializedAs("WindowsLocked")]
    public bool windowsLocked;

    [FormerlySerializedAs("VolumeActive")]
    public bool volumeActive = true;

    [FormerlySerializedAs("VolumeRotation")]
    public float volumeRotation;

    public WindowState() => Instance = this;

    public static WindowState Instance { get; set; } = null!;

    public void SetWindowState(bool closed, bool locked) {
        if (!WindowConfig.enableShutter.Value)
            return;

        var windows = Object.FindObjectsByType<ShipWindow>(FindObjectsSortMode.None);

        foreach (var w in windows)
            w.SetClosed(closed);

        windowsClosed = closed;
        windowsLocked = locked;
    }

    public void SetVolumeState(bool active) {
        var outsideSkybox = ShipWindows.outsideSkybox;
        outsideSkybox?.SetActive(active);

        volumeActive = active;
    }

    public void SetVolumeRotation(float rotation) {
        SpaceSkybox.Instance?.SetRotation(rotation);
        volumeRotation = rotation;
    }

    public void ReceiveSync() {
        // By this point the Instance has already been replaced, so we can just update the actual objects
        // with what the values should be.

        ShipWindows.Logger.LogInfo("Receiving window sync message...");

        SetWindowState(windowsClosed, windowsLocked);
        SetVolumeState(volumeActive);
        SetVolumeRotation(volumeRotation);
    }
}