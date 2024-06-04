using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;
using Unity.Netcode;

namespace ShipWindows.Networking;

[Serializable]
internal class NetworkHandler {
    public delegate void OnWindowSwitchToggled();

    public delegate void OnWindowSyncReceive();

    internal static CustomMessagingManager MessageManager => NetworkManager.Singleton.CustomMessagingManager;
    internal static bool IsClient => NetworkManager.Singleton.IsClient;
    internal static bool IsHost => NetworkManager.Singleton.IsHost;
    public static event OnWindowSyncReceive WindowSyncReceivedEvent;
    public static event OnWindowSwitchToggled WindowSwitchToggledEvent;

    public static void RegisterMessages() {
        ShipWindows.Logger.LogInfo("Registering network message handlers...");
        MessageManager.RegisterNamedMessageHandler("ShipWindow_WindowSyncResponse", ReceiveWindowSync);

        MessageManager.RegisterNamedMessageHandler("ShipWindow_WindowSwitchUsed", ReceiveWindowSwitchUsed_Server);
        MessageManager.RegisterNamedMessageHandler("ShipWindow_WindowSwitchUsedBroadcast", ReceiveWindowSwitchUsed_Client);

        if (IsHost)
            MessageManager.RegisterNamedMessageHandler("ShipWindow_WindowSyncRequest", ReceiveWindowSyncRequest);
    }

    public static void UnregisterMessages() {
        ShipWindows.Logger.LogInfo("Unregistering network message handlers...");
        MessageManager.UnregisterNamedMessageHandler("ShipWindow_WindowSyncResponse");
        MessageManager.UnregisterNamedMessageHandler("ShipWindow_WindowSyncRequest");

        MessageManager.UnregisterNamedMessageHandler("ShipWindow_WindowSwitchUsed");
        MessageManager.UnregisterNamedMessageHandler("ShipWindow_WindowSwitchUsedBroadcast");
    }

    public static void WindowSwitchUsed(bool currentState) {
        using FastBufferWriter stream = new(1, Allocator.Temp);
        stream.WriteValueSafe(currentState);

        //ShipWindows.Logger.LogInfo("Sending window switch toggle message...");

        MessageManager.SendNamedMessage("ShipWindow_WindowSwitchUsed", 0ul, stream);
    }

    public static void ReceiveWindowSwitchUsed_Server(ulong clientId, FastBufferReader reader) {
        reader.ReadValueSafe(out bool currentState);

        using FastBufferWriter stream = new(1, Allocator.Temp);
        stream.WriteValueSafe(currentState);

        //ShipWindows.Logger.LogInfo($"Received window switch toggle message from client {clientId}");

        MessageManager.SendNamedMessageToAll("ShipWindow_WindowSwitchUsedBroadcast", stream);
    }

    public static void ReceiveWindowSwitchUsed_Client(ulong _, FastBufferReader reader) {
        reader.ReadValueSafe(out bool currentState);

        //ShipWindows.Logger.LogInfo("Received window switch toggle message from server...");

        WindowState.Instance.SetWindowState(!currentState, WindowState.Instance.windowsLocked);
    }

    private static void ReceiveWindowSyncRequest(ulong clientId, FastBufferReader reader) {
        if (!IsHost) return;

        var windowStateBytes = SerializeToBytes(WindowState.Instance);
        var length = windowStateBytes.Length;
        const int intSize = sizeof(int);

        using FastBufferWriter stream = new(length + intSize, Allocator.Temp);

        try {
            stream.WriteValueSafe(in length);
            stream.WriteBytesSafe(windowStateBytes);

            MessageManager.SendNamedMessage("ShipWindow_WindowSyncResponse", clientId, stream);
        } catch (Exception e) {
            ShipWindows.Logger.LogError($"Error occurred sending window sync message:\n{e}");
        }
    }

    public static void RequestWindowSync() {
        if (!IsClient) return;

        ShipWindows.Logger.LogInfo("Requesting WindowState sync...");

        using FastBufferWriter stream = new(1, Allocator.Temp);
        MessageManager.SendNamedMessage("ShipWindow_WindowSyncRequest", 0ul, stream);
    }

    public static void ReceiveWindowSync(ulong _, FastBufferReader reader) {
        const int intSize = sizeof(int);

        if (!reader.TryBeginRead(intSize)) {
            ShipWindows.Logger.LogError("Failed to read window sync message");
            return;
        }

        reader.ReadValueSafe(out int len);
        if (!reader.TryBeginRead(len)) {
            ShipWindows.Logger.LogError("Window sync failed.");
            return;
        }

        ShipWindows.Logger.LogInfo("Receiving WindowState sync message...");

        var data = new byte[len];
        reader.ReadBytesSafe(ref data, len);

        var state = DeserializeFromBytes<WindowState>(data);
        WindowState.Instance = state;

        //ShipWindows.Logger.LogInfo($"{state.WindowsClosed}, {state.WindowsLocked}, {state.VolumeActive}, {state.VolumeRotation}");

        WindowSyncReceivedEvent?.Invoke();
    }

    public static byte[] SerializeToBytes(object val) {
        BinaryFormatter bf = new();
        using MemoryStream stream = new();

        try {
            bf.Serialize(stream, val);
            return stream.ToArray();
        } catch (Exception e) {
            ShipWindows.Logger.LogError($"Error serializing object: \n{e}");
            return null!;
        }
    }

    public static T DeserializeFromBytes<T>(byte[] data) {
        BinaryFormatter bf = new();
        using MemoryStream stream = new(data);

        try {
            return (T) bf.Deserialize(stream);
        } catch (Exception e) {
            ShipWindows.Logger.LogError($"Error deserializing object: \n{e}");
            return default!;
        }
    }
}