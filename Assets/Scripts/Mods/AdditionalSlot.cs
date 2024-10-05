using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Mods", order = 1)]
public class AdditionalSlot : SlotMod
{
    public override int INEEDMONEY(IEnumerable<SlotMod> mods)
    {
        return mods.Count();
    }
}
