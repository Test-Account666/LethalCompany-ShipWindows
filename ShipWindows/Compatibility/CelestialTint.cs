namespace ShipWindows.Compatibility;

internal static class CelestialTint {
    public static bool Enabled { get; private set; }

    private static bool Initialize() {
        if (Enabled) return false;
        Enabled = true;

        return true;
    }
}