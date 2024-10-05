using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModManager : MonoBehaviour
{
    public static ModManager instance { get; private set; }

    List<SlotMod> _mods = new List<SlotMod>();

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    /// <summary>
    /// Return all mods
    /// </summary>
    /// <returns></returns>
    public List<SlotMod> GetMods() => _mods;

    public void AddMod(SlotMod mod)
    {
        _mods.Add(mod);
    }
}
