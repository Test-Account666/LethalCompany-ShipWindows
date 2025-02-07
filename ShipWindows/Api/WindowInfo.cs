using System.Collections.Generic;
using UnityEngine;

namespace ShipWindows.Api;

[CreateAssetMenu(menuName = "ShipWindows/WindowInfo", order = 1)]
public class WindowInfo : ScriptableObject {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public string windowName;
    public string windowDescription;
    public int cost;

    [Header("Please keep window types conform to docs.")]
    public string windowType;

    public List<string> objectsToDisable;

    public GameObject windowPrefab;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [Header("Not part of the API and initial value will be ignored.")]
    public bool alwaysUnlocked;
}