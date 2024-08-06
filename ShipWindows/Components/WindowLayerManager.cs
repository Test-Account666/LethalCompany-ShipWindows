using System;
using UnityEngine;

namespace ShipWindows.Components;

public class WindowLayerManager : MonoBehaviour {
    private void Start() {
        if (WindowConfig.vanillaMode.Value) {
            SetLayer(false);
            return;
        }

        WindowConfig.allowEnemyTriggerThroughWindows.SettingChanged += UpdateLayer;
    }

    private void OnDestroy() => WindowConfig.allowEnemyTriggerThroughWindows.SettingChanged -= UpdateLayer;
    private void UpdateLayer(object o, EventArgs eventArgs) => SetLayer(WindowConfig.allowEnemyTriggerThroughWindows.Value);
    private void SetLayer(bool enemySeeThrough) => gameObject.layer = enemySeeThrough? 28 : 8;
}