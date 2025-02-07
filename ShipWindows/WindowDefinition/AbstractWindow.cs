using ShipWindows.Api;
using ShipWindows.Config;
using UnityEngine;

namespace ShipWindows.WindowDefinition;

public abstract class AbstractWindow : MonoBehaviour {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WindowInfo windowInfo;
    public MeshRenderer meshRenderer;
    public Collider collider;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private void Awake() {
        UpdateMaterial();
        WindowConfig.glassMaterial.SettingChanged += (_, _) => UpdateMaterial();

        UpdateLayer();
        WindowConfig.allowEnemyTriggerThroughWindows.SettingChanged += (_, _) => UpdateLayer();
    }

    public virtual void UpdateLayer() => collider.gameObject.layer = LayerMask.NameToLayer(WindowConfig.allowEnemyTriggerThroughWindows.Value? "Railing" : "Room");

    public virtual void UpdateMaterial() => meshRenderer.material = WindowConfig.glassMaterial.Value.GetMaterial();

    public abstract void Initialize();
}