using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RowMultiplicator", menuName = "Multiplicator", order = 1)]
public class RowMultiplicator : Multiplicator
{
    [SerializeField]
    private int multiplicator = 1;

    [SerializeField]
    private AdditionalSlot specificSymbol = null;

    public override int INEEDMONEY(int score, Dictionary<Spot, AdditionalSlot> result)
    {
        int newScore = 0;
        bool sameTwo = false;
        bool sameThree = false;
        List<string> mainSymbols = new List<string>();

        foreach (var entry in result)
        {
            if(entry.Key.isMain)
            {
                mainSymbols.Add(entry.Value.name);
            }
        }

        if(mainSymbols[0] == mainSymbols[1])
        {
            if(mainSymbols[0] == mainSymbols[2])
            {
                sameThree = true;
            }
            else
            {
                sameTwo = true;
            }
        }
        else if(mainSymbols[1] == mainSymbols[2])
        {
            sameTwo = true;            
        }

        if (result.Where(i => i.Key.isMain).Count() == 3)
        {

            
            if (specificSymbol != null && result.Where(i => i.Key.isMain && i.Value.name == specificSymbol.name).Count() == 3)
            {
                newScore = multiplicator * result.FirstOrDefault(i => i.Key.isMain).Value.reward;
            }
        }
        return newScore;
    }
}
