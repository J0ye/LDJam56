using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotModType
{
    Test
}

public abstract class SlotMod : ScriptableObject
{
    public SlotModType type;
    public string name;
    public string description;
    public Sprite image;

    public abstract int INEEDMONEY(IEnumerable<SlotMod> mods);
}
