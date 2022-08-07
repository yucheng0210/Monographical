using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackUIManager : InventoryUIManager
{
    private BackpackManager manager;

    public override void GetManager()
    {
        manager = GetComponent<BackpackManager>();
    }

    public override void OnUsed(Item_SO item)
    {
        switch (item.itemAbility)
        {
            case Item_SO.ItemAbility.Tonic:
                BackpackManager.abilityCount = 1;
                break;
            case Item_SO.ItemAbility.AttackUp:
                BackpackManager.abilityCount = 2;
                break;
        }
        if (BackpackManager.abilityCount == item.ItemAbilityNum)
            manager.RemoveItem(item);
    }
}
