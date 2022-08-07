using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : InventoryUIManager
{
    private BackpackManager manager;
    private ShopManager shopManager;

    public override void GetManager()
    {
        manager = GetComponent<BackpackManager>();
        shopManager = GetComponent<ShopManager>();
    }

    public override void OnUsed(Item_SO item)
    {
        if (manager.GetMoney() >= item.ItemCost && item.ItemInShop)
        {
            manager.ReduceMoney(item.ItemCost);
            manager.AddItem(item.ItemInBackpack);
            item.ItemHeld--;
        }
        RefreshItem();
    }
}
