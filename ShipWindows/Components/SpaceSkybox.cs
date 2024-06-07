using ShipWindows.Networking;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ShipWindows.Components;

[AddComponentMenu("TestAccount666/ShipWindows/SpaceSkybox")]
public class SpaceSkybox : MonoBehaviour {
    private HDRISky? _sky;

    private Transform? _starSphere;

    public static SpaceSkybox Instance { get; private set; } = null!;

    public void Awake() =>
        Instance = this;

    public void Start() {
        switch (WindowConfig.spaceOutsideSetting.Value) {
            case 0: break;
            case 1:
                var volume = GetComponent<Volume>();
                volume?.profile?.TryGet(out _sky);

                break;
            case 2:
                _starSphere = transform;
                break;
        }
    }

    public void Update() {
        if (WindowConfig.rotateSkybox.Value is false) return;

        switch (WindowConfig.spaceOutsideSetting.Value) {
            case 0: break;
            case 1:
                if (_sky is null) break;

                _sky.rotation.value += Time.deltaTime * 0.1f;
                if (_sky.rotation.value >= 360) _sky.rotation.value = 0f;
                WindowState.Instance.volumeRotation = _sky.rotation.value;
                break;
            case 2:
                if (_starSphere is null) break;

                _starSphere.Rotate(Vector3.forward * (Time.deltaTime * 0.1f));
                WindowState.Instance.volumeRotation = _starSphere.eulerAngles.y;
                break;
        }
    }

    public void SetRotation(float r) {
        switch (WindowConfig.spaceOutsideSetting.Value) {
            case 0: break;
            case 1:
                if (_sky is null) break;

                var rClamped = r % 360;
                if (rClamped < 0f) rClamped += 360f;

                _sky.rotation.value = rClamped;
                WindowState.Instance.volumeRotation = _sky.rotation.value;
                break;
            case 2:
                if (_starSphere is null) break;

                _starSphere.rotation = Quaternion.identity;
                _starSphere.Rotate(Vector3.forward * r);
                WindowState.Instance.volumeRotation = _starSphere.eulerAngles.y;
                break;
        }
    }

    public void SetSkyboxTexture(Texture2D? skybox) {
        switch (WindowConfig.spaceOutsideSetting.Value) {
            case 0: break;
            case 1:
                if (_sky is null) return;

                _sky.hdriSky.value = skybox;
                break;
        }
    }
}