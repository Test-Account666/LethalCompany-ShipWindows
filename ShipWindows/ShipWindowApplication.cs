// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;
using ShipWindows.Api;
using Object = UnityEngine.Object;

namespace ShipWindows;

public class ShipWindowApplication : InteractiveTerminalApplication<CursorElement> {
    public override void Initialization() {
        List<CursorElement> cursorElements = [
        ];
        cursorElements.AddRange(from windowInfo in ShipWindows.windowRegistry.Windows
                                where !windowInfo.alwaysUnlocked
                                let isUnlocked = WindowUnlockData.UnlockedWindows.Contains(windowInfo.windowName)
                                let elementAction = !isUnlocked? WindowBuyAction(windowInfo) : WindowAlreadyUnlockedAction(windowInfo)
                                let description = $"- Price: {windowInfo.cost}$"
                                select CursorElement.Create(windowInfo.windowName, description, elementAction, _ => !isUnlocked));

        if (cursorElements.Count <= 0) {
            ErrorMessage("Ship Windows", ExitApplication, "No unlockable windows found");
            return;
        }

        var cursorMenu = CursorMenu<CursorElement>.Create(elements: cursorElements.ToArray());

        SwitchScreen(BoxedScreen.Create("Ship Windows", [cursorMenu,]), cursorMenu, false);
    }

    public Action WindowAlreadyUnlockedAction(WindowInfo window) {
        return () => ErrorMessage(window.windowName, window.windowDescription, Initialization, $"{window.windowName} already unlocked!");
    }

    public Action WindowBuyAction(WindowInfo window) {
        return () => {
            Confirm(window.windowName, window.windowDescription, () => {
                var credits = terminal.groupCredits;

                if (credits < window.cost) {
                    ErrorMessage(window.windowName, window.windowDescription, Initialization, "Not enough money!");
                    return;
                }

                ShipWindows.networkManager?.SpawnWindow(window);

                terminal.PlayTerminalAudioServerRpc(0);

                ErrorMessage(window.windowName, window.windowDescription, Initialization, "Thanks for your purchase!");
            }, Initialization);
        };
    }

    public static void ExitApplication() => Object.Destroy(InteractiveTerminalManager.Instance.gameObject);
}