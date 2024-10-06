using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RowMultiplicator", menuName = "RowMultiplicator", order = 1)]
public class RowMultiplicator : Multiplicator
{
    [SerializeField]
    private int multiplicator = 1;

    [SerializeField]
    private AdditionalSlot specificSymbol = null;

    public override int INEEDMONEY(int score, Dictionary<Spot, AdditionalSlot> result)
    {
        int mult = 0;
        List<string> mainSymbols = new List<string>();

        var mains = result.Where(i => i.Key.isMain).Select(i => i.Value.name).ToArray();
        bool isThreeEquals = mains[0] == mains[1] && mains[1] == mains[2];
        
        if (isThreeEquals)
        {
            mult = multiplicator;

            if (specificSymbol != null && mains[0] != specificSymbol.name)
            {
                mult = 0;
            }
        }
        return mult;
    }
}
