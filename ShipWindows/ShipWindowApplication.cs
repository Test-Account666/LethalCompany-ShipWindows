using System;
using System.Collections.Generic;
using System.Linq;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;
using ShipWindows.Api;

namespace ShipWindows;

public class ShipWindowApplication : InteractiveTerminalApplication {
    public override void Initialization() {
        List<CursorElement> cursorElements = [
        ];
        cursorElements.AddRange(from windowInfo in ShipWindows.windowRegistry.windows
                                let isUnlocked = ShipWindows.windowManager.unlockedWindows.Contains(windowInfo.windowName)
                                let elementAction = !isUnlocked? WindowBuyAction(windowInfo) : WindowAlreadyUnlockedAction(windowInfo)
                                select CursorElement.Create(windowInfo.windowName, $"{windowInfo.cost}'", elementAction, _ => !isUnlocked));


        var cursorMenu = CursorMenu.Create(elements: cursorElements.ToArray());

        SwitchScreen(BoxedScreen.Create("Ship Windows", [
            cursorMenu,
        ]), cursorMenu, false);
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
}