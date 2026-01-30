// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only
using System.Collections;
using ShipWindows.Api.events;
using ShipWindows.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShipWindows.SkyBox;

public class SceneListener {
    public static SceneListener? Instance { get; private set; } = null;
    private readonly AbstractSkyBox _skyBox;

    public SceneListener(AbstractSkyBox skyBox) {
        Instance = this;
        _skyBox = skyBox;

        SceneManager.sceneLoaded += (_, _) => {
            if (!StartOfRound.Instance) return;

            StartOfRound.Instance.StartCoroutine(CheckSceneStateDelayed());
        };

        SceneManager.sceneUnloaded += _ => {
            if (!StartOfRound.Instance) return;

            StartOfRound.Instance.StartCoroutine(CheckSceneStateDelayed());
        };
    }

    private IEnumerator CheckSceneStateDelayed() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        CheckSceneState();
    }

    private void CheckSceneState() {
        if (!ShipWindows.skyBox) return;

        if (SceneManager.sceneCount is not 1 || SceneManager.GetActiveScene() is not {
                name: "SampleSceneRelay",
            }) {
            ShipWindows.skyBox!.ToggleSkyBox(false);
            EventAPI.AfterSkyboxUnloaded(_skyBox);
            return;
        }

        ShipWindows.skyBox!.ToggleSkyBox(true);
        EventAPI.AfterSkyboxLoaded(_skyBox);

        if (!WindowConfig.hideSpaceProps.Value) return;
        StartOfRound.Instance.currentPlanetPrefab.transform.parent.gameObject.SetActive(false);
    }
}