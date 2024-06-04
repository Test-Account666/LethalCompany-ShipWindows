using ShipWindows.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace ShipWindows.Components;

public class ShipWindowSpawner : MonoBehaviour {
    [FormerlySerializedAs("ID")]
    public int id;

    public void Start() {
        OnStart();
    }

    public void OnDestroy() =>
        // If the ship was already replaced, calling again will revert it.
        ShipReplacer.ReplaceDebounced(false);

    // Flag to the mod that we have spawned. It will wait for a moment and then
    // find all ShipWindowSpawners to replace the ship once instead of n times.
    public void OnStart() =>
        ShipReplacer.ReplaceDebounced(true);
    //ShipWindows.Logger.LogInfo($"We should spawn window {ID}");
}