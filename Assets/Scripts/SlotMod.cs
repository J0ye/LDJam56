using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotModType
{
    Test
}

[CreateAssetMenu(fileName = "Mods", order = 1)]
public class SlotMod : ScriptableObject
{
    public SlotModType type;
    public string name;
    public string description;
    public Sprite image;
}
