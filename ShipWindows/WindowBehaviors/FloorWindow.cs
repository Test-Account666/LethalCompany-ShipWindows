// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using ShipWindows.Config;
using ShipWindows.WindowDefinition;
using UnityEngine;

namespace ShipWindows.WindowBehaviors;

public class FloorWindow : AbstractWindow {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public GameObject underlights;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public override void Initialize() => underlights.SetActive(WindowConfig.enableUnderLights.Value);
}