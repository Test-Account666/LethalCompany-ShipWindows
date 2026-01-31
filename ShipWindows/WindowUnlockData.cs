// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using System.Collections.Generic;
using LethalModDataLib.Attributes;
using LethalModDataLib.Enums;

namespace ShipWindows;

public static class WindowUnlockData {
    [ModData(SaveWhen.OnSave, LoadWhen.Manual, ResetWhen = ResetWhen.OnGameOver)]
    public static readonly List<string> UnlockedWindows = [
    ];
}