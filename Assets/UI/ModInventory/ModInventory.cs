using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModInventory : MonoBehaviour
{
    public static ModInventory instance { get; private set; }

    [SerializeField]
    ModInventoryElement _modGridElementPrefab;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
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
