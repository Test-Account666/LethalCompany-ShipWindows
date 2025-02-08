using System.Collections.Generic;
using LethalModDataLib.Attributes;
using LethalModDataLib.Enums;

namespace ShipWindows;

public static class WindowUnlockData {
    [ModData(SaveWhen.OnSave, LoadWhen.Manual, ResetWhen = ResetWhen.OnGameOver)]
    public static readonly List<string> UnlockedWindows = [
    ];
}