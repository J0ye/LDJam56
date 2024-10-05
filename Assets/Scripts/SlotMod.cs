using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlotMod : ScriptableObject
{
    public string name;
    public string description;
    public Sprite image;

    public abstract string GetType();
}
