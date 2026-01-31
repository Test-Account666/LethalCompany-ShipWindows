// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using System;
using ShipWindows.Config;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ShipWindows.SkyBox;

public class HdriSpaceSkybox : AbstractSkyBox {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Volume skyVolume;

    private HDRISky _sky = null!;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static HdriSpaceSkybox Instance { get; private set; } = null!;

    public override void Awake() {
        base.Awake();

        Instance = this;
        ShipWindows.skyBox = this;
    }

    private void Update() => CurrentRotation += Time.deltaTime * WindowConfig.skyboxRotateSpeed.Value;

    public override void SetSkyboxTexture(Texture? skybox) => _sky.hdriSky.value = skybox;

    public override float CurrentRotation {
        get {
            if (!_sky && !skyVolume.profile.TryGet(out _sky)) throw new NullReferenceException("Could not find the skybox!");

            return _sky.rotation.value;
        }
        set {
            if (!_sky && !skyVolume.profile.TryGet(out _sky)) throw new NullReferenceException("Could not find the skybox!");

            _sky.rotation.overrideState = true;
            _sky.rotation.value = value;
            if (_sky.rotation.value >= 360) _sky.rotation.value -= 360f;
            if (_sky.rotation.value < 0) _sky.rotation.value += 360f;
        }
    }

    public override void ToggleSkyBox(bool enable) {
        skyVolume.gameObject.SetActive(enable);
        skyVolume.enabled = enable;
    }
}