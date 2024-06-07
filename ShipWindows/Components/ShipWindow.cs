﻿using ShipWindows.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace ShipWindows.Components;

[AddComponentMenu("TestAccount666/ShipWindows/ShipWindow")]
public class ShipWindow : MonoBehaviour {
    // Window 3
    public static string[] window3DisabledList = [
        "UnderbellyMachineParts", "NurbsPath.001",
    ];

    [FormerlySerializedAs("ID")]
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

        if (id is not 3)
            return;

        gameObject.tag = closed? "Aluminum" : "Wood";
    }

    public void OnStart() {
        switch (id) {
            case 1:
                break;

            case 2:
                if (WindowConfig.dontMovePosters.Value is false) {
                    var movedPostersPrefab =
                        ShipWindows.mainAssetBundle.LoadAsset<GameObject>("Assets/LethalCompany/Mods/ShipWindow/ShipPosters.prefab");
                    if (movedPostersPrefab is not null) {
                        var oldPosters = ShipReplacer.newShipInside?.transform.parent.Find("Plane.001");
                        if (oldPosters is not null) {
                            _oldPostersObject = oldPosters.gameObject;
                            var newPosters = ObjectReplacer.Replace(_oldPostersObject, movedPostersPrefab);

                            // Support for custom posters.
                            ObjectReplacer.ReplaceMaterial(_oldPostersObject, newPosters);
                        }
                    }
                }

                break;

            case 3:
                foreach (var go in window3DisabledList) {
                    var obj = GameObject.Find($"Environment/HangarShip/{go}");

                    obj?.gameObject.SetActive(false);
                }

                if (WindowConfig.disableUnderLights.Value) {
                    var floodLights = ShipReplacer.newShipInside?.transform.Find("WindowContainer/Window3/Lights");
                    floodLights?.gameObject.SetActive(false);
                }

                break;
        }
    }
}