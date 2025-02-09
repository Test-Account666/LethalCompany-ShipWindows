using ShipWindows.Api;
using ShipWindows.Config;
using UnityEngine;

namespace ShipWindows.WindowDefinition;

public abstract class AbstractWindow : MonoBehaviour {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WindowInfo windowInfo;
    public MeshRenderer[] meshRenderers;
    public Collider[] colliders;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private void Awake() {
        UpdateMaterial();
        WindowConfig.glassMaterial.SettingChanged += (_, _) => UpdateMaterial();

        UpdateLayer();
        WindowConfig.allowEnemyTriggerThroughWindows.SettingChanged += (_, _) => UpdateLayer();
    }

    public virtual void UpdateLayer() {
        var allowEnemyTriggerThroughWindows = WindowConfig.allowEnemyTriggerThroughWindows.Value;

        foreach (var collider in colliders) collider.gameObject.layer = LayerMask.NameToLayer(allowEnemyTriggerThroughWindows? "Railing" : "Room");
    }

    public virtual void UpdateMaterial() {
        foreach (var meshRenderer in meshRenderers) meshRenderer.material = WindowConfig.glassMaterial.Value.GetMaterial();
    }

    public abstract void Initialize();
}