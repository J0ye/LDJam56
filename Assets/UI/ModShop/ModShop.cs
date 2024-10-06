using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModShop : MonoBehaviour
{
    public static ModShop instance { get; private set; }

    [SerializeField]
    ModShopElement _prefab;

    [SerializeField]
    List<SlotMod> _availableMods;

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
        GenerateRandomMods();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateRandomMods()
    {
        //Delete all existing
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < 5; i++)
        {
            var slotMod = GetRandomMod();
            var modShopElement = Instantiate(_prefab, transform);
            modShopElement.Initialize(slotMod);
        }
    }

    private SlotMod GetRandomMod()
    {
        return _availableMods[Random.Range(0, _availableMods.Count)];
    }

    //public void UpdateUI()
    //{
    //    // Delete all existing
    //    foreach (Transform child in transform)
    //    {
    //        GameObject.Destroy(child.gameObject);
    //    }

    //    // Add all back
    //    foreach (var mod in ModManager.instance.GetMods())
    //    {
    //        var modGridElement = Instantiate(_prefab, transform);
    //        modGridElement.Initialize(mod);
    //    }
    //}
}
