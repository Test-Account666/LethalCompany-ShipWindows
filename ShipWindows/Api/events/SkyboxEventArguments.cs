using ShipWindows.SkyBox;

namespace ShipWindows.Api.events;

public struct SkyboxEventArguments(AbstractSkyBox skyBox) {
    public AbstractSkyBox skyBox = skyBox;
}