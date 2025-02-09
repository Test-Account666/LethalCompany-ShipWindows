using System.Collections;
using ShipWindows.Api;
using ShipWindows.Config;
using UnityEngine;

namespace ShipWindows.WindowDefinition;

[RequireComponent(typeof(Animator))]
public abstract class AbstractWindow : MonoBehaviour {
    public static readonly int CloseShutterAnimatorHash = Animator.StringToHash("CloseShutter");
    public static readonly int LockShutterAnimatorHash = Animator.StringToHash("LockShutter");

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WindowInfo windowInfo;
    public MeshRenderer[] meshRenderers;
    public Collider[] colliders;
    public Animator shutterAnimator;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private void Awake() {
        StartCoroutine(WaitAndRegister());

        UpdateMaterial();
        WindowConfig.glassMaterial.SettingChanged += (_, _) => UpdateMaterial();

        UpdateLayer();
        WindowConfig.allowEnemyTriggerThroughWindows.SettingChanged += (_, _) => UpdateLayer();
    }

    private IEnumerator WaitAndRegister() {
        yield return new WaitUntil(() => ShipWindows.windowManager != null!);

        ShipWindows.windowManager.spawnedWindows.Add(this);
    }

    private void OnDestroy() => ShipWindows.windowManager.spawnedWindows.Remove(this);

    public virtual void UpdateLayer() {
        var allowEnemyTriggerThroughWindows = WindowConfig.allowEnemyTriggerThroughWindows.Value;

        foreach (var collider in colliders) collider.gameObject.layer = LayerMask.NameToLayer(allowEnemyTriggerThroughWindows? "Railing" : "Room");
    }

    public virtual void UpdateMaterial() {
        foreach (var meshRenderer in meshRenderers) meshRenderer.material = WindowConfig.glassMaterial.Value.GetMaterial();
    }

    public virtual void ToggleWindowShutter(bool closeShutter, bool lockShutter = false) {
        shutterAnimator.SetBool(CloseShutterAnimatorHash, closeShutter);
        shutterAnimator.SetBool(LockShutterAnimatorHash, lockShutter);
    }

    public abstract void Initialize();
}