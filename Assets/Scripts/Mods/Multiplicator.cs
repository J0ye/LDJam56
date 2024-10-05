using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Multiplicator", menuName = "Multiplicator", order = 1)]
public class Multiplicator : SlotMod
{
    public override string GetType()
    {
        return "multiplicator";
    }

    public int INEEDMONEY(IEnumerable<SlotMod> mods) { return 0; }
}
