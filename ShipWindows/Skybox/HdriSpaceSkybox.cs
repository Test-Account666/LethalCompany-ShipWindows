using System;
using ShipWindows.Config;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ShipWindows.SkyBox;

public class HdriSpaceSkybox : MonoBehaviour, ISkyBox {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Volume skyVolume;

    private HDRISky _sky = null!;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private SceneListener _sceneListener = null!;

    public static HdriSpaceSkybox Instance { get; private set; } = null!;

    private void Awake() {
        Instance = this;
        ShipWindows.skyBox = this;

        _sceneListener = new();
    }

    private void Update() => CurrentRotation += Time.deltaTime * WindowConfig.skyboxRotateSpeed.Value;

    public void SetSkyboxTexture(Texture? skybox) => _sky.hdriSky.value = skybox;

    public float CurrentRotation {
        get {
            if (!_sky && !skyVolume.profile.TryGet(out _sky)) throw new NullReferenceException("Could not find the skybox!");

            return _sky.rotation.value;
        }
        set {
            if (!_sky && !skyVolume.profile.TryGet(out _sky)) throw new NullReferenceException("Could not find the skybox!");

            _sky.rotation.value += value - _sky.rotation.value;
            if (_sky.rotation.value >= 360) _sky.rotation.value = 0f;
            if (_sky.rotation.value <= 0) _sky.rotation.value = 360f;
        }
    }

    public void ToggleSkyBox(bool enable) => skyVolume.enabled = enable;
}