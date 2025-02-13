using System.Collections;
using ShipWindows.Api.events;
using ShipWindows.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShipWindows.SkyBox;

public class SceneListener {
    private readonly AbstractSkyBox _skyBox;

    public SceneListener(AbstractSkyBox skyBox) {
        _skyBox = skyBox;

        ShipWindows.Logger.LogFatal("Starting SceneListener!");

        SceneManager.sceneLoaded += (_, _) => {
            if (!StartOfRound.Instance) return;

            ShipWindows.Logger.LogFatal("Scene loaded!");

            StartOfRound.Instance.StartCoroutine(CheckSceneStateDelayed());
        };

        SceneManager.sceneUnloaded += _ => {
            if (!StartOfRound.Instance) return;

            ShipWindows.Logger.LogFatal("Scene unloaded!");

            StartOfRound.Instance.StartCoroutine(CheckSceneStateDelayed());
        };
    }

    private IEnumerator CheckSceneStateDelayed() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        CheckSceneState();
    }

    private void CheckSceneState() {
        if (!ShipWindows.skyBox) {
            ShipWindows.Logger.LogFatal("Skybox null?!");
            return;
        }

        if (SceneManager.sceneCount is not 1 || SceneManager.GetActiveScene() is not {
                name: "SampleSceneRelay",
            }) {
            ShipWindows.skyBox!.ToggleSkyBox(false);
            EventAPI.AfterSkyboxUnloaded(_skyBox);
            ShipWindows.Logger.LogFatal("Toggled skybox off!");
            return;
        }

        ShipWindows.skyBox!.ToggleSkyBox(true);
        EventAPI.AfterSkyboxLoaded(_skyBox);
        ShipWindows.Logger.LogFatal("Toggled skybox on!");

        if (!WindowConfig.hideSpaceProps.Value) return;
        StartOfRound.Instance.currentPlanetPrefab.transform.parent.gameObject.SetActive(false);
    }
}