using System;
using ShipWindows.Networking;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ShipWindows.Components;

[AddComponentMenu("TestAccount666/ShipWindows/ SpaceSkybox")]
public class SpaceSkybox : MonoBehaviour {
    private HDRISky? _sky;

    private Transform? _starSphere;

    public static SpaceSkybox Instance { get; private set; } = null!;

    public void Awake() =>
        Instance = this;

    public void Start() {
        switch (WindowConfig.spaceOutsideSetting.Value) {
            case SpaceOutside.OTHER_MODS: break;
            case SpaceOutside.SPACE_HDRI:
                var volume = GetComponent<Volume>();
                volume?.profile?.TryGet(out _sky);

                break;
            case SpaceOutside.BLACK_AND_STARS:
                _starSphere = transform;
                break;
            default:
                throw new ArgumentOutOfRangeException(WindowConfig.spaceOutsideSetting.Value + " is not a valid option!");
        }
    }

    public void Update() {
        switch (WindowConfig.spaceOutsideSetting.Value) {
            case SpaceOutside.OTHER_MODS: break;
            case SpaceOutside.SPACE_HDRI:
                if (_sky is null) break;

                _sky.rotation.value += Time.deltaTime * WindowConfig.skyboxRotateSpeed.Value;
                if (_sky.rotation.value >= 360) _sky.rotation.value = 0f;
                WindowState.Instance.volumeRotation = _sky.rotation.value;
                break;
            case SpaceOutside.BLACK_AND_STARS:
                if (_starSphere is null) break;

                _starSphere.Rotate(Vector3.forward * (Time.deltaTime * WindowConfig.skyboxRotateSpeed.Value));
                WindowState.Instance.volumeRotation = _starSphere.eulerAngles.y;
                break;
            default:
                throw new ArgumentOutOfRangeException(WindowConfig.spaceOutsideSetting.Value + " is not a valid option!");
        }
    }

    public void SetRotation(float r) {
        switch (WindowConfig.spaceOutsideSetting.Value) {
            case SpaceOutside.OTHER_MODS: break;
            case SpaceOutside.SPACE_HDRI:
                if (_sky is null) break;

                var rClamped = r % 360;
                if (rClamped < 0f) rClamped += 360f;

                _sky.rotation.value = rClamped;
                WindowState.Instance.volumeRotation = _sky.rotation.value;
                break;
            case SpaceOutside.BLACK_AND_STARS:
                if (_starSphere is null) break;

                _starSphere.rotation = Quaternion.identity;
                _starSphere.Rotate(Vector3.forward * r);
                WindowState.Instance.volumeRotation = _starSphere.eulerAngles.y;
                break;
            default:
                throw new ArgumentOutOfRangeException(WindowConfig.spaceOutsideSetting.Value + " is not a valid option!");
        }
    }

    public void SetSkyboxTexture(Texture? skybox) {
        switch (WindowConfig.spaceOutsideSetting.Value) {
            case SpaceOutside.OTHER_MODS: break;
            case SpaceOutside.SPACE_HDRI:
                if (_sky is null) return;

                _sky.hdriSky.value = skybox;
                break;
            case SpaceOutside.BLACK_AND_STARS:
                break;
            default:
                throw new ArgumentOutOfRangeException(WindowConfig.spaceOutsideSetting.Value + " is not a valid option!");
        }
    }
}