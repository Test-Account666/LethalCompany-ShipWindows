namespace ShipWindows.Compatibility;

public class ShipMods {
    public static bool Enabled { get; private set; }

    private static bool Initialize() {
        if (Enabled) return false;
        Enabled = true;

        return true;
    }
}