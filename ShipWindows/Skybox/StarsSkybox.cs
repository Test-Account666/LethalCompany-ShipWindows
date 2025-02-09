using ShipWindows.Config;
using UnityEngine;

namespace ShipWindows.SkyBox;

public class StarsSkybox : MonoBehaviour, ISkyBox {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Transform stars;
    public GameObject starsObject;
    public MeshRenderer starsRenderer;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private SceneListener _sceneListener = null!;

    public static StarsSkybox Instance { get; private set; } = null!;

    private void Awake() {
        Instance = this;
        ShipWindows.skyBox = this;

        _sceneListener = new();
    }

    private void Update() {
        stars.Rotate(Vector3.forward * (Time.deltaTime * WindowConfig.skyboxRotateSpeed.Value));

        var rotation = stars.rotation.eulerAngles;

        if (rotation.y >= 360) rotation.y -= 360;
        if (rotation.y <= 0) rotation.y += 360;
        stars.rotation = stars.rotation = Quaternion.Euler(rotation);

        CurrentRotation = rotation.y;
    }

    public float CurrentRotation { get; private set; }

    public void ToggleSkyBox(bool enable) => starsObject.SetActive(enable);

    public void SetSkyboxTexture(Texture? skybox) => starsRenderer.material.mainTexture = skybox;
}