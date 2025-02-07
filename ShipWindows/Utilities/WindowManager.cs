using System;
using System.Collections.Generic;
using System.Linq;
using ShipWindows.Api;
using ShipWindows.WindowDefinition;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShipWindows.Utilities;

public class WindowManager {
    public GameObject decapitatedShip = null!;

    public readonly List<string> unlockedWindows = [
    ];

    public WindowManager() {
        foreach (var windowInfo in ShipWindows.windowRegistry.windows.Where(windowInfo => windowInfo.alwaysUnlocked)) {
            unlockedWindows.Add(windowInfo.windowName);
            CreateWindow(windowInfo);
        }
    }

    //TODO: Figure out if destroying windows is worth it

    public void CreateDecapitatedShip() {
        var shipInside = GameObject.Find("Environment/HangarShip/ShipInside");

        if (!shipInside) throw new NullReferenceException("Could not find ShipInside!");

        var decapitatedShipPrefab = ShipWindows.Instance.GetDecapitatedShipPrefab();

        decapitatedShip = Object.Instantiate(decapitatedShipPrefab, shipInside.transform);
        decapitatedShip.name = "DecapitatedShip";

        shipInside.GetComponent<MeshRenderer>().enabled = false;
        shipInside.GetComponent<MeshCollider>().enabled = false;
    }

    public void CreateWindow(WindowInfo windowInfo) {
        if (unlockedWindows.Contains(windowInfo.windowName.ToLower())) return;

        if (!decapitatedShip) CreateDecapitatedShip();

        var windowObject = Object.Instantiate(windowInfo.windowPrefab, decapitatedShip.transform);
        var window = windowObject.GetComponentInChildren<Window>();

        window.Initialize();

        foreach (var objectToDisable in windowInfo.objectsToDisable) {
            var foundObject = GameObject.Find(objectToDisable);

            if (!foundObject) {
                ShipWindows.Logger.LogError($"Couldn't find object '{objectToDisable}'!");
                continue;
            }

            foundObject.SetActive(false);
        }

        unlockedWindows.Add(windowInfo.windowName.ToLower());
    }
}