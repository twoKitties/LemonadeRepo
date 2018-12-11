using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LemonadeStore;
using System.Linq;

public class Roulette
{
    public int townItemGradeID;

    private Storage GetTownItem()
    {
        Storage storage = GetUngradedItem(Storage.AllStorages, out townItemGradeID);
        return storage;
    }
    private Storage GetUngradedItem(List<Storage> townItems, out int townItem)
    {
        List<Storage> copyOfTownItems = townItems.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < copyOfTownItems.Count; i++)
        {
            for (int j = 0; j < copyOfTownItems[i].Unlocked.Length; j++)
            {
                if (copyOfTownItems[i].Unlocked[j])
                    continue;
                else
                {
                    townItem = j;
                    return copyOfTownItems[i];
                }
            }
        }
        townItem = 0;
        return null;
    }

    public void CalculateReward()
    {

    }
}
