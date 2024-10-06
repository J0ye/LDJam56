using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlotMod : ScriptableObject
{
    public string name;
    public string tinyText;
    public string description;
    public Sprite image;
    public Color color = Color.gray;
    public int probability = 1;

    public abstract string GetType();
}
