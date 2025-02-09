using UnityEngine;

namespace ShipWindows.SkyBox;

public interface ISkyBox {
    public float CurrentRotation { get; }

    public void ToggleSkyBox(bool enabled);

    public void SetSkyboxTexture(Texture? skybox);
}