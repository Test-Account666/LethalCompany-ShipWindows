using System;
using System.Collections.Generic;
using System.Linq;
using LethalModDataLib.Features;
using LethalModDataLib.Helpers;
using ShipWindows.Api;
using ShipWindows.Api.events;
using ShipWindows.WindowDefinition;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShipWindows.Utilities;

public class WindowManager {
    public GameObject decapitatedShip = null!;

    public List<AbstractWindow> spawnedWindows = [
    ];

    //TODO: Call ShipColors

    public WindowManager() {
        SaveLoadHandler.LoadData(ModDataHelper.GetModDataKey(typeof(WindowUnlockData), nameof(WindowUnlockData.UnlockedWindows))!);

        CreateDecapitatedShip();

        foreach (var windowInfo in ShipWindows.windowRegistry.windows.Where(windowInfo => windowInfo.alwaysUnlocked)) CreateWindow(windowInfo, check: false);

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var windowName in WindowUnlockData.UnlockedWindows) {
            var windowInfo = ShipWindows.windowRegistry.windows.FirstOrDefault(info => info.windowName.Equals(windowName));
            if (!windowInfo) continue;

            CreateWindow(windowInfo!, addToList: false, check: true);
        }
    }

    private void CreateDecapitatedShip() {
        var shipInside = GameObject.Find("Environment/HangarShip/ShipInside");

        if (!shipInside) throw new NullReferenceException("Could not find ShipInside!");

        var decapitatedShipPrefab = ShipWindows.Instance.GetDecapitatedShipPrefab();

        decapitatedShip = Object.Instantiate(decapitatedShipPrefab, shipInside.transform);
        decapitatedShip.name = "DecapitatedShip";

        shipInside.GetComponent<MeshRenderer>().enabled = false;
        shipInside.GetComponent<MeshCollider>().enabled = false;
    }

    public void CreateWindow(WindowInfo windowInfo, bool addToList = true, bool check = true) {
        var contains = WindowUnlockData.UnlockedWindows.Contains(windowInfo.windowName);
        if (check && contains) return;

        EventAPI.BeforeWindowSpawn(windowInfo);

        if (!decapitatedShip) CreateDecapitatedShip();

        var windowObject = Object.Instantiate(windowInfo.windowPrefab, decapitatedShip.transform);
        if (windowObject.name.ToLower().StartsWith("prefab")) windowObject.name = windowObject.name["prefab".Length..];

        var window = windowObject.GetComponentInChildren<AbstractWindow>();

        window.Initialize();

        foreach (var objectToDisable in windowInfo.objectsToDisable) {
            var foundObject = GameObject.Find(objectToDisable);

            if (!foundObject) {
                ShipWindows.Logger.LogError($"Couldn't find object '{objectToDisable}'!");
                continue;
            }

            foundObject.SetActive(false);
        }

        if (addToList && !contains) WindowUnlockData.UnlockedWindows.Add(windowInfo.windowName);

        EventAPI.AfterWindowSpawn(windowInfo, windowObject);
    }
}