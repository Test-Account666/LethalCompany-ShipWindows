// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using ShipWindows.SkyBox;
using UnityEngine;

namespace ShipWindows.Api.events;

public static class EventAPI {
    #region Window Events

    public delegate void WindowEvent(WindowEventArguments windowEvent);

    internal static WindowEventArguments BeforeWindowSpawn(WindowInfo windowInfo) {
        var windowEvent = new WindowEventArguments(windowInfo);

        BeforeWindowSpawned?.Invoke(windowEvent);

        return windowEvent;
    }

    internal static WindowEventArguments AfterWindowSpawn(WindowInfo windowInfo, GameObject windowObject) {
        var windowEvent = new WindowEventArguments(windowInfo, windowObject);

        AfterWindowSpawned?.Invoke(windowEvent);

        return windowEvent;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static event WindowEvent BeforeWindowSpawned;
    public static event WindowEvent AfterWindowSpawned;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    #endregion Window Events

    #region Skybox Events

    public delegate void SkyboxEvent(SkyboxEventArguments windowEvent);

    internal static SkyboxEventArguments AfterSkyboxCreated(AbstractSkyBox skyBox) {
        var skyboxEvent = new SkyboxEventArguments(skyBox);

        SkyboxCreated?.Invoke(skyboxEvent);

        return skyboxEvent;
    }

    internal static SkyboxEventArguments AfterSkyboxLoaded(AbstractSkyBox skyBox) {
        var skyboxEvent = new SkyboxEventArguments(skyBox);

        SkyboxLoaded?.Invoke(skyboxEvent);

        return skyboxEvent;
    }

    internal static SkyboxEventArguments AfterSkyboxUnloaded(AbstractSkyBox skyBox) {
        var skyboxEvent = new SkyboxEventArguments(skyBox);

        SkyboxUnloaded?.Invoke(skyboxEvent);

        return skyboxEvent;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static event SkyboxEvent SkyboxCreated;
    public static event SkyboxEvent SkyboxLoaded;
    public static event SkyboxEvent SkyboxUnloaded;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    #endregion Skybox Events
}