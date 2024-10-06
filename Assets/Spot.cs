using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spot : MonoBehaviour
{
    public bool isMain = false;
    [HideInInspector]
    public bool isTaken; // Indicates if the spot is taken
    
    void Awake()
    {
        if(transform.parent.TryGetComponent<WheelSymbolManager>(out WheelSymbolManager wheelSymbolManager))
        {
            wheelSymbolManager.AddSpot(this);
        }
        else
        {
            Debug.LogError(gameObject.name + " is not on a wheel");
        }
    }
}