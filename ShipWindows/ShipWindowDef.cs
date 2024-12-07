using ShipWindows.Components;
using Unity.Netcode;
using UnityEngine;

namespace ShipWindows;

public class ShipWindowDef {
    public int baseCost;
    public int id;
    public GameObject prefab;
    public int unlockableID;

    private ShipWindowDef(int id, GameObject prefab, int baseCost) {
        this.id = id;
        this.prefab = prefab;
        this.baseCost = baseCost;
    }

    public static ShipWindowDef Register(int id, int baseCost) {
        ShipWindows.Logger.LogInfo($"Registering window prefab: Window {id}");
        var windowSpawner = ShipWindows.mainAssetBundle.LoadAsset<GameObject>($"Assets/LethalCompany/Mods/plugins/ShipWindows/SpawnWindow{id}.prefab");
        windowSpawner.AddComponent<ShipWindowSpawner>().id = id;

        NetworkManager.Singleton.AddNetworkPrefab(windowSpawner);

        ShipWindowDef def = new(id, windowSpawner, baseCost);
        //def.UnlockableID = Unlockables.AddWindowToUnlockables(def);

        return def;
    }
}