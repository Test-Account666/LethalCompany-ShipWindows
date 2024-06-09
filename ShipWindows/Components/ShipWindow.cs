using ShipWindows.Utilities;
using UnityEngine;

namespace ShipWindows.Components;

[AddComponentMenu("TestAccount666/ShipWindows/ShipWindow")]
public class ShipWindow : MonoBehaviour {
    // Window 3
    public static string[] window3DisabledList = [
        "UnderbellyMachineParts", "NurbsPath.001",
    ];


    public int id;

    // Misc variables held by the windows. This is kind of nasty.

    // Window 2
    private GameObject? _oldPostersObject;
    private static readonly int _ClosedId = Animator.StringToHash("Closed");

    public void Start() =>
        OnStart();

    public void OnDestroy() {
        switch (id) {
            case 1:
                break;

            case 2:
                ObjectReplacer.Restore(_oldPostersObject!);
                break;

            case 3:
                foreach (var go in window3DisabledList) {
                    var obj = GameObject.Find($"Environment/HangarShip/{go}");

                    obj?.gameObject.SetActive(true);
                }

                break;
        }
    }

    public void SetClosed(bool closed) {
        var animator = GetComponent<Animator>();

        if (animator is null)
            return;

        animator.SetBool(_ClosedId, closed);
    }

    public void OnStart() {
        switch (id) {
            case 1:
                break;

            case 2:
                if (WindowConfig.dontMovePosters.Value)
                    break;

                var movedPostersPrefab =
                    ShipWindows.mainAssetBundle.LoadAsset<GameObject>("Assets/LethalCompany/Mods/ShipWindow/ShipPosters.prefab");
                if (movedPostersPrefab is null) break;

                var oldPosters = ShipReplacer.newShipInside?.transform.parent.Find("Plane.001");

                if (oldPosters is null) break;

                _oldPostersObject = oldPosters.gameObject;
                var newPosters = ObjectReplacer.Replace(_oldPostersObject, movedPostersPrefab);

                // Support for custom posters.
                ObjectReplacer.ReplaceMaterial(_oldPostersObject, newPosters);

                break;

            case 3:
                foreach (var go in window3DisabledList) {
                    var obj = GameObject.Find($"Environment/HangarShip/{go}");

                    obj?.gameObject.SetActive(false);
                }

                if (!WindowConfig.disableUnderLights.Value) break;

                var floodLights = ShipReplacer.newShipInside?.transform.Find("WindowContainer/Window3/Lights");
                floodLights?.gameObject.SetActive(false);

                break;

            case 4:
                break;
        }
    }
}