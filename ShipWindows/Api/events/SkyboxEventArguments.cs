// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using ShipWindows.SkyBox;

namespace ShipWindows.Api.events;

public struct SkyboxEventArguments(AbstractSkyBox skyBox) {
    public AbstractSkyBox skyBox = skyBox;
}