using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RowMultiplicator", menuName = "Multiplicator", order = 1)]
public class RowMultiplicator : Multiplicator
{
    public override int INEEDMONEY(Dictionary<AdditionalSlot, Spot> result)
    {
        throw new System.NotImplementedException();
    }
}
