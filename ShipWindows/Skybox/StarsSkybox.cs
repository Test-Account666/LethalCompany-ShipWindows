// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using ShipWindows.Config;
using UnityEngine;

namespace ShipWindows.SkyBox;

public class StarsSkybox : AbstractSkyBox {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Transform stars;
    public GameObject starsObject;
    public MeshRenderer starsRenderer;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static StarsSkybox Instance { get; private set; } = null!;

    public override void Awake() {
        base.Awake();

        Instance = this;
        ShipWindows.skyBox = this;
    }

    private void Update() => CurrentRotation += Time.deltaTime * WindowConfig.skyboxRotateSpeed.Value;

    public override float CurrentRotation {
        get => stars.rotation.eulerAngles.y;
        set {
            var rotation = stars.rotation.eulerAngles;

            rotation.y += value - rotation.y;

            if (rotation.y >= 360) rotation.y -= 360;
            if (rotation.y <= 0) rotation.y += 360;
            stars.rotation = Quaternion.Euler(rotation);
        }
    }

    public override void ToggleSkyBox(bool enable) => starsObject.SetActive(enable);

    public override void SetSkyboxTexture(Texture? skybox) => starsRenderer.material.mainTexture = skybox;
}