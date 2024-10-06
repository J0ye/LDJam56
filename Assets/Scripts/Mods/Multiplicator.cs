using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Multiplicator", menuName = "Multiplicator", order = 1)]
public abstract class Multiplicator : SlotMod
{ 
    public override string GetType()
    {
        return "multiplicator";
    }

    public abstract int INEEDMONEY(int score, Dictionary<Spot, AdditionalSlot> result);
}
