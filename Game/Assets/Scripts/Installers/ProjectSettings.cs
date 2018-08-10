using System;
using UnityEngine;

[Serializable]
public class Prefabs
{
    public GameObject ScreenRoot;
}

[CreateAssetMenu(fileName = "Project Settings", menuName = "Settings/Project Settings")]
public class ProjectSettings : ScriptableObject
{
    public ScreenContext ScreenContext;
    public Prefabs Prefabs;
}
