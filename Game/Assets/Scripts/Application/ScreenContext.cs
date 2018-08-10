using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScreenItem
{
    public ScreenType Type;
    public Transform Transform;
}

[CreateAssetMenu(fileName = "Screen Context", menuName = "Context/Screen context")]
public class ScreenContext : ScriptableObject
{
    public ScreenItem[] Map;
}
