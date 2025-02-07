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
                                let isUnlocked = ShipWindows.windowManager.unlockedWindows.Contains(windowInfo.windowName.ToLower())
                                let elementAction = !isUnlocked? WindowAction(windowInfo) : WindowAlreadyUnlockedAction(windowInfo)
                                select CursorElement.Create(windowInfo.windowName, $"{windowInfo.cost}'", elementAction, _ => !isUnlocked));


        var cursorMenu = CursorMenu.Create(elements: cursorElements.ToArray());

        SwitchScreen(BoxedScreen.Create("Ship Windows", [
            cursorMenu,
        ]), cursorMenu, false);
    }

    public Action WindowAlreadyUnlockedAction(WindowInfo window) {
        return () => ErrorMessage(window.windowName, window.windowDescription, Initialization, $"{window.windowName} already unlocked!");
    }

    public Action WindowAction(WindowInfo window) {
        return () => {
            Confirm(window.windowName, window.windowDescription, () => {
                var credits = terminal.groupCredits;

                if (credits < window.cost) {
                    ErrorMessage(window.windowName, window.windowDescription, Initialization, "Not enough money!");
                    return;
                }

                //TODO: Network this.

                terminal.SyncGroupCreditsServerRpc(credits - window.cost, terminal.numberOfItemsInDropship);

                var cancelled = ShipWindows.windowManager.CreateWindow(window, out var cancelReason);

                if (cancelled) {
                    ErrorMessage(window.windowName, window.windowDescription, Initialization, cancelReason);
                    return;
                }

                Initialization();
            }, Initialization);
        };
    }
}