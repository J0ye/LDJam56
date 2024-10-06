using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModInventory : MonoBehaviour
{
    public static ModInventory instance { get; private set; }

    [SerializeField]
    ModInventoryElement _modGridElementPrefab;

    [SerializeField]
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

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Return all mods
    /// </summary>
    /// <returns></returns>
    public List<SlotMod> GetMods() => _mods;

    public void AddMod(SlotMod mod)
    {
        _mods.Add(mod);
        ModInventory.instance.UpdateUI();
    }

    public void UpdateUI()
    {
        // Delete all existing
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Add all back
        foreach (var mod in GetMods())
        {
            var modGridElement = Instantiate(_modGridElementPrefab, transform);
            modGridElement.Initialize(mod);
        }
    }
}
