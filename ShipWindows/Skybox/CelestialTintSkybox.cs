// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using System;
using ShipWindows.Config;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ShipWindows.SkyBox;

public class CelestialTintSkybox : AbstractSkyBox {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Volume skyVolume;

    private PhysicallyBasedSky _sky = null!;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static CelestialTintSkybox Instance { get; private set; } = null!;

    public override void Awake() {
        base.Awake();

        Instance = this;
        ShipWindows.skyBox = this;
    }

    private void Update() => CurrentRotation += Time.deltaTime * WindowConfig.skyboxRotateSpeed.Value;

    public override void SetSkyboxTexture(Texture? skybox) => _sky.spaceEmissionTexture.value = skybox;

    public override float CurrentRotation {
        get {
            if (!_sky && !skyVolume.profile.TryGet(out _sky)) throw new NullReferenceException("Could not find the skybox!");

            return _sky.spaceRotation.value.y;
        }
        set {
            if (!_sky && !skyVolume.profile.TryGet(out _sky)) throw new NullReferenceException("Could not find the skybox!");

            if (_sky.spaceRotation.value.y > 360) _sky.spaceRotation.value -= new Vector3(0, 360, 0);
            if (_sky.spaceRotation.value.y < 0) _sky.spaceRotation.value += new Vector3(0, 360, 0);

            _sky.spaceRotation.value += new Vector3(0, value - _sky.spaceRotation.value.y, 0);
        }
    }

    public override void ToggleSkyBox(bool enable) {
        skyVolume.gameObject.SetActive(enable);
        skyVolume.enabled = enable;
    }
}