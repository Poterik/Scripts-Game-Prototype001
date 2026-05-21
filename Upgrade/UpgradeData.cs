using System;
using UnityEngine;

[Serializable]
public class UpgradeData
{
    public string name;
    public string description;
    public Sprite icon;
    public int bonus;

    [System.NonSerialized]
    public System.Action<UpgradeData> applyAction;
}
