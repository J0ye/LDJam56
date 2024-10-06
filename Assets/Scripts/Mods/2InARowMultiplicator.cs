using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TwoInARowMultiplicator", menuName = "TwoInARowMultiplicator", order = 1)]
public class TwoInARowMultiplicator : Multiplicator
{
    [SerializeField]
    private int multiplicator = 1;

    [SerializeField]
    private AdditionalSlot specificSymbol = null;

    public override int INEEDMONEY(int score, Dictionary<Spot, AdditionalSlot> result)
    {
        int mult = 0;
        List<string> mainSymbols = new List<string>();

        var mains = result.Where(i => i.Key.isMain == true).Select(i => i.Value.name).ToArray();
        bool isTwoEquals = mains[0] == mains[1] || mains[1] == mains[2] || mains[0] == mains[2];
        Debug.Log($"isTwoEquals: {isTwoEquals}");
        Debug.Log($"mains: {string.Join(", ", mains)}");
        if (isTwoEquals)
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
