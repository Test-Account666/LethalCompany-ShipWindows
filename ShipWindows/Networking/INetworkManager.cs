// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using ShipWindows.Api;
using Unity.Netcode;

namespace ShipWindows.Networking;

public interface INetworkManager {
    public NetworkObject NetworkObject { get; }

    public void SpawnWindow(WindowInfo windowInfo);

    public void ToggleShutters(bool closeShutters, bool lockShutters = false, bool playAudio = false);

    public void SyncUnlockedWindows();

    public void SyncSkyboxRotation();

    public void SyncShutter();

    public void PlayWesleyVoice(int index);
}