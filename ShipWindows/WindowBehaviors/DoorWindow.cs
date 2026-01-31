// Copyright (C) 2026 TestAccount666
// SPDX-License-Identifier: LGPL-3.0-only

using System.Linq;
using ShipWindows.WindowDefinition;
using UnityEngine;

namespace ShipWindows.WindowBehaviors;

public class DoorWindow : AbstractWindow {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public GameObject leftDoorWindow;
    public GameObject rightDoorWindow;

    public Mesh leftDoorMesh;
    public Mesh rightDoorMesh;

    public Mesh originalLeftDoorMesh;
    public Mesh originalRightDoorMesh;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public override void Initialize() {
        var shipDoor = StartOfRound.Instance.shipDoorsAnimator.gameObject;

        var doorLeft = shipDoor.transform.Find("HangarDoorLeft (1)").gameObject;
        var doorRight = shipDoor.transform.Find("HangarDoorRight (1)").gameObject;

        // Fix right door materials being reversed
        var doorRightMeshRenderer = doorRight.GetComponent<MeshRenderer>();
        doorRightMeshRenderer.materials = doorRightMeshRenderer.materials.Reverse().ToArray();

        var leftDoorWindowTransform = leftDoorWindow.transform;
        ReplaceDoorSide(doorLeft, leftDoorWindowTransform, leftDoorMesh, out originalLeftDoorMesh);

        var rightDoorWindowTransform = rightDoorWindow.transform;
        ReplaceDoorSide(doorRight, rightDoorWindowTransform, rightDoorMesh, out originalRightDoorMesh);
    }

    private void OnDestroy() {
        Destroy(leftDoorWindow);
        Destroy(rightDoorWindow);

        var shipDoor = StartOfRound.Instance.shipDoorsAnimator.gameObject;

        var doorLeft = shipDoor.transform.Find("HangarDoorLeft (1)").gameObject;
        var doorRight = shipDoor.transform.Find("HangarDoorRight (1)").gameObject;

        // Undo revere fix
        var doorRightMeshRenderer = doorRight.GetComponent<MeshRenderer>();
        doorRightMeshRenderer.materials = doorRightMeshRenderer.materials.Reverse().ToArray();

        RestoreDoorSide(doorLeft, originalLeftDoorMesh);
        RestoreDoorSide(doorRight, originalRightDoorMesh);
    }

    private static void RestoreDoorSide(GameObject doorSide, Mesh doorMesh) {
        doorSide.GetComponent<MeshFilter>().mesh = doorMesh;
        doorSide.GetComponent<BoxCollider>().enabled = true;
        Destroy(doorSide.GetComponent<MeshCollider>());
    }

    private static void ReplaceDoorSide(GameObject doorSide, Transform doorWindowTransform, Mesh doorMesh, out Mesh originalMesh) {
        doorWindowTransform.parent = doorSide.transform;
        doorWindowTransform.localScale = new(1, 1, 1);
        doorWindowTransform.localPosition = new(0, 0, 0);
        doorWindowTransform.localRotation = Quaternion.Euler(0, 0, 0);

        var meshFilter = doorSide.GetComponent<MeshFilter>();

        originalMesh = meshFilter.mesh;

        meshFilter.mesh = doorMesh;
        doorSide.GetComponent<BoxCollider>().enabled = false;
        doorSide.AddComponent<MeshCollider>().sharedMesh = doorMesh;
    }
}