using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ShipWindows.Components;

[AddComponentMenu("TestAccount666/ShipWindows/CelestialTintSkyboxRotator")]
public class CelestialTintSkyboxRotator : MonoBehaviour {
    [SerializeField]
    private Volume? skyVolume;

    private PhysicallyBasedSky? _sky;

    private void Update() {
        if (skyVolume == null) return;

        if (_sky == null) {
            skyVolume.profile.TryGet(out _sky);
            return;
        }

        if (_sky.spaceRotation.value.y > 360)
            _sky.spaceRotation.value -= new Vector3(0, 360, 0);

        if (_sky.spaceRotation.value.y < 0)
            _sky.spaceRotation.value += new Vector3(0, 360, 0);

        _sky.spaceRotation.value += new Vector3(0, Time.deltaTime * WindowConfig.skyboxRotateSpeed.Value, 0);
    }
}