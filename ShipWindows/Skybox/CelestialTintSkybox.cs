using System;
using ShipWindows.Config;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ShipWindows.SkyBox;

public class CelestialTintSkybox : MonoBehaviour, ISkyBox {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Volume skyVolume;

    private PhysicallyBasedSky _sky = null!;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private SceneListener _sceneListener = null!;

    public static CelestialTintSkybox Instance { get; private set; } = null!;

    private void Awake() {
        Instance = this;
        ShipWindows.skyBox = this;

        _sceneListener = new();
    }

    private void Update() {
        if (!_sky && !skyVolume.profile.TryGet(out _sky)) throw new NullReferenceException("Could not find the skybox!");

        if (_sky.spaceRotation.value.y > 360) _sky.spaceRotation.value -= new Vector3(0, 360, 0);
        if (_sky.spaceRotation.value.y < 0) _sky.spaceRotation.value += new Vector3(0, 360, 0);

        _sky.spaceRotation.value += new Vector3(0, Time.deltaTime * WindowConfig.skyboxRotateSpeed.Value, 0);

        CurrentRotation = _sky.spaceRotation.value.y;
    }

    public void SetSkyboxTexture(Texture? skybox) => _sky.spaceEmissionTexture.value = skybox;
    public float CurrentRotation { get; private set; }

    public void ToggleSkyBox(bool enable) => skyVolume.enabled = enable;
}