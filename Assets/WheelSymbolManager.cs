using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WheelSymbolManager : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> spawnedSymbols = new List<GameObject>();
    [HideInInspector]
    public List<Spot> spots = new List<Spot>();

    void Start()
    {
        GenerateSymbols();
    }
    
    public void AddSpot(Spot spot)
    {
        spots.Add(spot);
    }


    // UpdateSymbols method to clear and generate new symbols
    public void UpdateSymbols()
    {
        ClearSymbols(); // Clear existing symbols
        GenerateSymbols(); // Generate new symbols
    }

    private void GenerateSymbols()
    {
        var slots = ModInventory.instance.GetMods().Where(i => i.GetType() == "slot").ToList();
        //slots = Shuffle(slots).ToList();

        foreach (AdditionalSlot mod in slots)
        {
            var newSymbol = Instantiate(mod.prefab, Vector3.zero, Quaternion.identity, transform);
            newSymbol.GetComponent<SlotItem>().Initialize(mod);
            newSymbol.SetActive(false);
            
            spawnedSymbols.Add(newSymbol);
        }
    }

    public void ClearSymbols()
    {
        foreach (GameObject symbol in spawnedSymbols)
        {
            Destroy(symbol); // Destroy each GameObject
        }
        spawnedSymbols.Clear(); // Clear the list
    }
}
