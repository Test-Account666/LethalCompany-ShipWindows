using ShipWindows.Config;
using ShipWindows.WindowDefinition;
using UnityEngine;

namespace ShipWindows.WindowBehaviors;

public class RightWindow : AbstractWindow {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Mesh postersMesh;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Mesh _originalPostersMesh = null!;

    public override void Initialize() {
        if (!WindowConfig.movePosters.Value) return;

        UpdatePostersMesh(postersMesh);
    }

    private void OnDestroy() {
        if (!_originalPostersMesh) return;

        UpdatePostersMesh(_originalPostersMesh);
    }

    public void UpdatePostersMesh(Mesh newMesh) {
        var posters = StartOfRound.Instance.shipAnimator.transform.Find("Plane.001");

        if (!posters) {
            ShipWindows.Logger.LogWarning("Could not find posters!");
            return;
        }

        var meshFilter = posters.GetComponent<MeshFilter>();

        if (!_originalPostersMesh) _originalPostersMesh = meshFilter.mesh;

        meshFilter.mesh = newMesh;
    }
}