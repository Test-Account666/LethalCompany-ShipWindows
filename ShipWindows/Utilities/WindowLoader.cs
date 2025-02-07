using ShipWindows.Api;

namespace ShipWindows.Utilities;

internal static class WindowLoader {
    public static void LoadWindows() {
        var windowInfos = ShipWindows.mainAssetBundle.LoadAllAssets<WindowInfo>();

        windowInfos ??= [
        ];

        foreach (var windowInfo in windowInfos) {
            if (!windowInfo) continue;

            ShipWindows.windowRegistry.RegisterWindow(windowInfo);
        }
    }
}