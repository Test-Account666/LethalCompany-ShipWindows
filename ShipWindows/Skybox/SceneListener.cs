using System.Collections;
using ShipWindows.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShipWindows.SkyBox;

public class SceneListener {
    public SceneListener() {
        SceneManager.sceneLoaded += (_, _) => {
            if (!StartOfRound.Instance) return;

            StartOfRound.Instance.StartCoroutine(CheckSceneStateDelayed());
        };

        SceneManager.sceneUnloaded += _ => {
            if (!StartOfRound.Instance) return;

            StartOfRound.Instance.StartCoroutine(CheckSceneStateDelayed());
        };
    }

    private static IEnumerator CheckSceneStateDelayed() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        CheckSceneState();
    }

    private static void CheckSceneState() {
        if (SceneManager.sceneCount is not 1 || SceneManager.GetActiveScene() is not {
                name: "SampleSceneRelay",
            }) {
            ShipWindows.skyBox?.ToggleSkyBox(false);
            return;
        }

        ShipWindows.skyBox?.ToggleSkyBox(true);

        if (!WindowConfig.hideSpaceProps.Value) return;
        StartOfRound.Instance.currentPlanetPrefab.transform.parent.gameObject.SetActive(false);
    }
}