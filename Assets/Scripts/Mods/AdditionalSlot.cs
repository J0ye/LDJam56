using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Slot", menuName = "Slot", order = 1)]
public class AdditionalSlot : SlotMod
{
    public override string GetType()
    {
        return "slot";
    }

    public int INEEDMONEY(IEnumerable<SlotMod> mods) { return 0; }
}
