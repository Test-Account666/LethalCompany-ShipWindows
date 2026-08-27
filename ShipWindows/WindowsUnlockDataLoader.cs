// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dawn;
using TestAccountCore.KotlinSharp.Extensions;

namespace ShipWindows;

public static class WindowsUnlockDataLoader {
    private static NamespacedKey? _ShipWindowsKey;

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void Load() {
        ShipWindows.Logger.LogDebug("Loading windows unlock data...");
        _ShipWindowsKey ??= NamespacedKey.From("me.testaccount666.shipwindows", "windows");
        var currentSave = DawnLib.GetCurrentContract();
        if (currentSave == null) {
            WindowUnlockData.UnlockedWindows = new SavingList<string>();
            ShipWindows.Logger.LogDebug("No current save!");
            return;
        }

        if (!currentSave.TryGet<string[]>(_ShipWindowsKey, out var windowsUnlockData)) {
            WindowUnlockData.UnlockedWindows = new SavingList<string>();
            ShipWindows.Logger.LogDebug("No unlock data!");
            return;
        }

        WindowUnlockData.UnlockedWindows = new SavingList<string>(windowsUnlockData);
        ShipWindows.Logger.LogDebug($"Got: {WindowUnlockData.UnlockedWindows.Count} windows!");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void Save() {
        ShipWindows.Logger.LogDebug("Saving windows unlock data...");
        _ShipWindowsKey ??= NamespacedKey.From("me.testaccount666.shipwindows", "windows");
        var currentSave = DawnLib.GetCurrentContract();
        if (currentSave == null) {
            ShipWindows.Logger.LogDebug("No current save!");
            return;
        }

        currentSave.Set(_ShipWindowsKey, WindowUnlockData.UnlockedWindows.ToArray());
        ShipWindows.Logger.LogDebug($"Saved: {WindowUnlockData.UnlockedWindows.Count} windows!");
    }
}

internal class SavingList<T> : IList<T> {
    private readonly List<T> _list = [];
    public SavingList(params T[] items) => _list.AddRange(items);

    internal void AddUnsafely(T item) => _list.Add(item);
    internal void RemoveUnsafely(T item) => _list.Remove(item);
    internal void ClearUnsafely() => _list.Clear();
    internal void AddRangeUnsafely(params T[] items) => _list.AddRange(items);


    #region Implementation of IEnumerable

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Implementation of ICollection<T>

    public void Add(T item) {
        _list.Add(item);
        WindowsUnlockDataLoader.Save();
    }

    public void Clear() {
        _list.Clear();
        WindowsUnlockDataLoader.Save();
    }

    public bool Contains(T item) => _list.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public bool Remove(T item) => _list.Remove(item).Also(_ => WindowsUnlockDataLoader.Save());

    public int Count => _list.Count;
    public bool IsReadOnly => false;

    #endregion

    #region Implementation of IList<T>

    public int IndexOf(T item) => _list.IndexOf(item);

    public void Insert(int index, T item) {
        _list.Insert(index, item);
        WindowsUnlockDataLoader.Save();
    }

    public void RemoveAt(int index) {
        _list.RemoveAt(index);
        WindowsUnlockDataLoader.Save();
    }

    public T this[int index] {
        get => _list[index];
        set {
            _list[index] = value;
            WindowsUnlockDataLoader.Save();
        }
    }

    #endregion
}