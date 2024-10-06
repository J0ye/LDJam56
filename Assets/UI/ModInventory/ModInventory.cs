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
    
    /// <summary>
    /// Return all mods
    /// </summary>
    /// <returns></returns>
    public List<SlotMod> GetMods() => _mods;

    public void AddMod(SlotMod mod)
    {
        _mods.Add(mod);
        instance.UpdateUI();
    }

    public void UpdateUI()
    {
        // Delete all existing
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Dictionary<SlotMod, ModInventoryElement> modTypes = new Dictionary<SlotMod, ModInventoryElement>();
        // Add all back
        foreach (var mod in GetMods())
        {
            var modGridElement = Instantiate(_modGridElementPrefab, transform);
            modGridElement.Initialize(mod);
            if(modTypes.ContainsKey(mod))
            {
                modGridElement.gameObject.SetActive(false);
                modTypes[mod].UpdateCount(1);
            }
            else
            {
                modTypes.Add(mod, modGridElement);
            }
        }
    }
}
