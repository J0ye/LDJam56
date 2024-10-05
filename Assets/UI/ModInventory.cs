using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModInventory : MonoBehaviour
{
    [SerializeField]
    ModGridElement _modGridElementPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMods()
    {
        // Delete all existing
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Add all back
        foreach (var mod in ModManager.instance.GetMods())
        {
            var modGridElement = Instantiate(_modGridElementPrefab, transform);
            modGridElement.Initialize(mod);
        }
    }
}
