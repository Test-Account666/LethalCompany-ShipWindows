using UnityEngine;

namespace ShipWindows.SkyBox;

public abstract class AbstractSkyBox : MonoBehaviour {
    public abstract float CurrentRotation { get; set; }

    public abstract void ToggleSkyBox(bool enabled);

    public abstract void SetSkyboxTexture(Texture? skybox);
}