using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModInventory : MonoBehaviour
{
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
        //ModManager.Get
    }
}
