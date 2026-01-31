// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using UnityEngine;

namespace ShipWindows.Api.events;

public struct WindowEventArguments(WindowInfo windowInfo, GameObject? windowObject = null) {
    public WindowInfo windowInfo = windowInfo;
    public GameObject? windowObject = windowObject;
}