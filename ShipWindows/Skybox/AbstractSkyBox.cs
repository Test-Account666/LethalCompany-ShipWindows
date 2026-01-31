// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using ShipWindows.Api.events;
using UnityEngine;

namespace ShipWindows.SkyBox;

public abstract class AbstractSkyBox : MonoBehaviour {
    public abstract float CurrentRotation { get; set; }

    private SceneListener _sceneListener = null!;

    public virtual void Awake() {
        EventAPI.AfterSkyboxCreated(this);

        if (SceneListener.Instance != null) {
            _sceneListener = SceneListener.Instance;
            return;
        }

        _sceneListener = new(this);
    }

    public abstract void ToggleSkyBox(bool enabled);

    public abstract void SetSkyboxTexture(Texture? skybox);
}