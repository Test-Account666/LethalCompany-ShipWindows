using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace ShipWindows.Utilities;

internal class Unlockables {
    public static Dictionary<int, string> windowNames = new() {
        [1] = "Right Window",
        [2] = "Left Window",
        [3] = "Floor Window",
        [4] = "Door Window",
    };

    public static Dictionary<int, string> windowInfo = new() {
        [1] = "\nAdds a window to the right of the ship's control panel, behind the terminal.\n\n",
        [2] = "\nAdds a window to the left of the ship's control panel.\n\n",
        [3] = "\nAdds a window to the floor of the ship.\n\n",
        [4] = "\nAdds windows to the door of the ship.\n\n",
    };

    private static readonly Dictionary<int, WindowUnlockable> _WindowUnlockables = [
    ];

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Terminal), "TextPostProcess")]
    private static void Patch_TextPostProcess(ref string modifiedDisplayText, TerminalNode node) {
        try {
            if (!modifiedDisplayText.Contains("[buyableItemsList]") || !modifiedDisplayText.Contains("[unlockablesSelectionList]"))
                return;

            var index = modifiedDisplayText.IndexOf(":");

            modifiedDisplayText =
                (from unlock in _WindowUnlockables.Reverse()
                 where !ShipWindows.IsWindowDefaultUnlocked(unlock.Key)
                 select $"\n* {unlock.Value.name}    //    Price: ${unlock.Value.price}")
                .Aggregate(modifiedDisplayText, (current, upgradeLine) => current.Insert(index + 1, upgradeLine));
        } catch (Exception e) {
            ShipWindows.Logger.LogError(e);
        }
    }

    private static TerminalKeyword CreateKeyword(string word, TerminalKeyword defaultVerb) {
        var keyword = ScriptableObject.CreateInstance<TerminalKeyword>();
        keyword.name = word;
        keyword.word = word;
        keyword.isVerb = false;
        keyword.accessTerminalObjects = false;
        keyword.defaultVerb = defaultVerb;

        return keyword;
    }

    public static int AddWindowToUnlockables(Terminal terminal, ShipWindowDef def) {
        if (!windowNames.TryGetValue(def.id, out var name))
            name = $"Window {def.id}";

        ShipWindows.Logger.LogInfo($"Adding {name} to unlockables...");

        int windowUnlockableID;

        var unlockablesList = StartOfRound.Instance.unlockablesList;

        var index = unlockablesList.unlockables.FindIndex(unlockable => unlockable.unlockableName == name);

        if (!_WindowUnlockables.ContainsKey(def.id))
            _WindowUnlockables.Add(def.id, new() {
                name = name,
                price = def.baseCost,
            });

        if (index is not -1) {
            windowUnlockableID = index;
        } else {
            var buyKeyword = terminal.terminalNodes.allKeywords.First(keyword => keyword.word == "buy");
            var cancelPurchaseNode = buyKeyword.compatibleNouns[0].result.terminalOptions[1].result;
            var infoKeyword = terminal.terminalNodes.allKeywords.First(keyword => keyword.word == "info");

            var keyword = CreateKeyword($"{name.ToLowerInvariant().Replace(" ", "")}", buyKeyword);

            var windowUnlockableItem = new UnlockableItem {
                unlockableName = name,
                spawnPrefab = true,
                alwaysInStock = true,
                prefabObject = def.prefab,
                unlockableType = 1,
                IsPlaceable = false,
                maxNumber = 1,
                canBeStored = false,
                alreadyUnlocked = ShipWindows.IsWindowDefaultUnlocked(def.id),
            };

            unlockablesList.unlockables.Capacity++;
            unlockablesList.unlockables.Add(windowUnlockableItem);
            windowUnlockableID = unlockablesList.unlockables.FindIndex(unlockable => unlockable.unlockableName == name);

            ShipWindows.Logger.LogInfo($"{name} added to unlockable list at index {windowUnlockableID}");

            var buyNode2 = ScriptableObject.CreateInstance<TerminalNode>();
            buyNode2.name = $"{name.Replace(" ", "-")}BuyNode2";
            buyNode2.displayText = $"Ordered {name}! Your new balance is [playerCredits].\n\nPlease clean the windows at the end of your contract.\n\n";
            buyNode2.clearPreviousText = true;
            buyNode2.maxCharactersToType = 15;
            buyNode2.buyItemIndex = -1;
            buyNode2.shipUnlockableID = windowUnlockableID;
            buyNode2.buyUnlockable = true;
            buyNode2.creatureName = name;
            buyNode2.isConfirmationNode = false;
            buyNode2.itemCost = def.baseCost;

            var buyNode1 = ScriptableObject.CreateInstance<TerminalNode>();
            buyNode1.name = $"{name.Replace(" ", "-")}BuyNode1";
            buyNode1.displayText = $"You have requested to order {name}.\nTotal cost of item: [totalCost].\n\nPlease CONFIRM or DENY.\n\n";
            buyNode1.clearPreviousText = true;
            buyNode1.maxCharactersToType = 15;
            buyNode1.shipUnlockableID = windowUnlockableID;
            buyNode1.itemCost = def.baseCost;
            buyNode1.creatureName = name;
            buyNode1.overrideOptions = true;
            buyNode1.terminalOptions = [
                new() {
                    noun = terminal.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "confirm"),
                    result = buyNode2,
                },
                new() {
                    noun = terminal.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "deny"),
                    result = cancelPurchaseNode,
                },
            ];

            var infoText = windowInfo.GetValueOrDefault(def.id, "[No information about this object was found.]\n");

            var itemInfo = ScriptableObject.CreateInstance<TerminalNode>();
            itemInfo.name = $"{name.Replace(" ", "-")}InfoNode";
            itemInfo.displayText = infoText;
            itemInfo.clearPreviousText = true;
            itemInfo.maxCharactersToType = 25;

            var allKeywords = terminal.terminalNodes.allKeywords.ToList();
            allKeywords.Add(keyword);
            terminal.terminalNodes.allKeywords = allKeywords.ToArray();

            var nouns = buyKeyword.compatibleNouns.ToList();
            nouns.Add(new() {
                noun = keyword,
                result = buyNode1,
            });
            buyKeyword.compatibleNouns = nouns.ToArray();

            var itemInfoNouns = infoKeyword.compatibleNouns.ToList();
            itemInfoNouns.Add(new() {
                noun = keyword,
                result = itemInfo,
            });
            infoKeyword.compatibleNouns = itemInfoNouns.ToArray();

            ShipWindows.Logger.LogInfo($"Registered terminal nodes for {name}");
        }

        return windowUnlockableID;
    }

    public static int AddSwitchToUnlockables() {
        int switchUnlockableID;

        const string name = "Shutter Switch";
        var unlockablesList = StartOfRound.Instance.unlockablesList;

        // When running in unity editor this function permanently edits the unlockables list.
        // To keep from duplicating a ton, check if the unlockable is already there and use its ID instead.

        var index = unlockablesList.unlockables.FindIndex(unlockable => unlockable.unlockableName == name);

        if (index is not -1) {
            switchUnlockableID = index;
        } else {
            var windowUnlockableItem = new UnlockableItem {
                unlockableName = name,
                spawnPrefab = false,
                unlockableType = 1,
                IsPlaceable = true,
                maxNumber = 1,
                canBeStored = false,
                alreadyUnlocked = true,
            };

            unlockablesList.unlockables.Capacity++;
            unlockablesList.unlockables.Add(windowUnlockableItem);
            switchUnlockableID = unlockablesList.unlockables.FindIndex(unlockable => unlockable.unlockableName == name);
        }

        if (ShipWindows.windowSwitchPrefab) {
            var shipObject = ShipWindows.windowSwitchPrefab.GetComponentInChildren<PlaceableShipObject>();
            shipObject.unlockableID = switchUnlockableID;
        }

        ShipWindows.Logger.LogInfo($"Added shutter switch to unlockables list at ID {switchUnlockableID}");

        return switchUnlockableID;
    }

    private class WindowUnlockable {
        public string name = null!;
        public int price;
    }
}