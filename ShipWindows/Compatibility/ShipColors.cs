using ShipColors.Events;

namespace ShipWindows.Compatibility;

internal static class ShipColors {
    public static bool Enabled { get; private set; }

    private static bool Initialize() {
        if (Enabled) return false;
        Enabled = true;

        return true;
    }

    public static void RefreshColors() => Subscribers.StartCustomizer();
}