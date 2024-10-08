using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Slot", menuName = "Slot", order = 1)]
public class AdditionalSlot : SlotMod
{
    public GameObject prefab;

    public int reward = 1;

    public override string GetType()
    {
        return "slot";
    }

    public int INEEDMONEY(int score, Spot mySpot) 
    { 
        if(mySpot.isMain) // or is gold. Gold always pays
        {
            return reward; 
        }

        return 0;
    }
}
